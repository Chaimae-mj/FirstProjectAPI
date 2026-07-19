using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FirstProjectAPI.Dtos;
using FirstProjectAPI.Models;

namespace FirstProjectAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IServices _service;
        private readonly IConfiguration _config;

        public AuthService(IServices service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        public AuthResponseDto Register(RegisterDto dto)
        {
            if (_service.GetUserByEmail(dto.Email) != null)
                throw new EmailAlreadyExistsException();

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                // Toute personne peut s'inscrire comme Apprenant.
                // Un rôle Formateur/Admin ne doit être attribué que par un Admin
                // (voir AuthController.Register : on ignore dto.Role pour les non-admins).
                Role = Role.Apprenant
            };

            _service.Add(user);

            return BuildAuthResponse(user);
        }

        // Réservé aux Admins (vérifié par [Authorize(Roles = "Admin")] sur le controller).
        // Permet de créer un compte Formateur ou Admin avec un rôle explicite.
        public AuthResponseDto RegisterStaff(RegisterDto dto)
        {
            if (dto.Role == null || dto.Role == Role.Apprenant)
                throw new InvalidStaffRoleException();

            if (_service.GetUserByEmail(dto.Email) != null)
                throw new EmailAlreadyExistsException();

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = dto.Role.Value
            };

            _service.Add(user);

            return BuildAuthResponse(user);
        }

        public AuthResponseDto Login(LoginDto dto)
        {
            var user = _service.GetUserByEmail(dto.Email);
            if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
                throw new InvalidCredentialsException();

            return BuildAuthResponse(user);
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection["Key"]!;
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiresMinutes = int.Parse(jwtSection["ExpiresMinutes"] ?? "120");

            var claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(JwtRegisteredClaimNames.Email, user.Email),
    new Claim(ClaimTypes.Name, user.Name),
    new Claim(ClaimTypes.Role, user.Role.ToString()),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};


            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
