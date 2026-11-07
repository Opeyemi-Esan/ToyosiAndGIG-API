using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Interfaces;

namespace ToyosiAndGIG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("AuthPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        [EnableRateLimiting("RefreshPolicy")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenDto dto)
        {
            var response = await _authService.LogoutAsync(dto);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
