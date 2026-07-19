using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FirstProjectAPI.Dtos;
using FirstProjectAPI.Services;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/auth/register
        // Inscription publique : le rôle est toujours forcé à "Apprenant" côté service.
        // Pour créer un Formateur ou un Admin, voir /api/auth/register-staff (réservé Admin).
        [HttpPost("register")]
        public ActionResult<AuthResponseDto> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = _authService.Register(dto);
                return StatusCode(201, result);
            }
            catch (EmailAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // POST api/auth/register-staff
        // Réservé aux Admins : permet de créer un compte Formateur ou Admin.
        [Authorize(Roles = "Admin")]
        [HttpPost("register-staff")]
        public ActionResult<AuthResponseDto> RegisterStaff([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = _authService.RegisterStaff(dto);
                return StatusCode(201, result);
            }
            catch (EmailAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (InvalidStaffRoleException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/auth/login
        [HttpPost("login")]
        public ActionResult<AuthResponseDto> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = _authService.Login(dto);
                return Ok(result);
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // GET api/auth/me
        // Renvoie les infos de l'utilisateur connecté (à partir du token JWT).
        [Authorize]
        [HttpGet("me")]
        public ActionResult Me()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var id = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

            return Ok(new { id, name = email, role });
        }
    }
}
