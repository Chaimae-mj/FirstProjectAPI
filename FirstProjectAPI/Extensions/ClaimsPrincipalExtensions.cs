using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FirstProjectAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Récupère l'Id de l'utilisateur connecté depuis les claims du token JWT.
        /// Cherche d'abord "sub", puis retombe sur ClaimTypes.NameIdentifier
        /// (utile si le token a été émis différemment par le passé).
        /// </summary>
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(JwtRegisteredClaimNames.Sub)
                        ?? user.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new UnauthorizedAccessException("Utilisateur non identifié dans le token.");

            return int.Parse(claim.Value);
        }

        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}