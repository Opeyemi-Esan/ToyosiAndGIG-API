using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Response<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
        Task<Response<string>> LogoutAsync(RefreshTokenDto dto);
    }
}
