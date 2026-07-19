using FirstProjectAPI.Dtos;

namespace FirstProjectAPI.Services
{
    public interface IAuthService
    {
        AuthResponseDto Register(RegisterDto dto);
        AuthResponseDto RegisterStaff(RegisterDto dto);
        AuthResponseDto Login(LoginDto dto);
    }
}
