using Microsoft.Win32;
using System.Net;
using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Application.Models;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;
using ToyosiAndGIG.Infrastructure.Repository;

namespace ToyosiAndGIG.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private const int RefreshTokenExpiryDays = 7;

        public AuthService(IIdentityService identityService, IUserRepository userRepository, ITokenService tokenService, ILogger<AuthService> logger, IRefreshTokenRepository refreshTokenRepository)
        {
            _identityService = identityService;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var request = new CreateUserRequest
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    UserName = registerDto.Username, // optional field from RegisterDto, falls back to Email if blank
                    PhoneNumber = registerDto.PhoneNumber,
                    Password = registerDto.Password,
                    Role = Roles.Customer, // default role for public self-registration
                    IsAdminAction = false
                };

                var identityResult = await _identityService.RegisterAsync(request);

                if (identityResult.StatusCode != HttpStatusCode.Created)
                {
                    return new Response<AuthResponseDto>
                    {
                        Data = null,
                        StatusCode = identityResult.StatusCode,
                        Message = identityResult.Message
                    };
                }

                var user = await _userRepository.GetUserByIdAsync(identityResult.Data);
                var authResponse = await IssueTokensAsync(user);
                //var roles = await _identityService.GetRolesAsync(user.Id);
                //var (token, expiresAt) = _tokenService.GenerateToken(user, roles);

                //var authResponse = new AuthResponseDto
                //{
                //    Token = token,
                //    ExpiresAt = expiresAt,
                //    UserId = user.Id,
                //    Email = user.Email
                //};

                return new Response<AuthResponseDto>
                {
                    Data = authResponse,
                    StatusCode = HttpStatusCode.Created,
                    Message = "User registered successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the registration");
                return new Response<AuthResponseDto>
                {
                    Data = null,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"An error occurred while processing the registration"
                };
            }

        }

        public async Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var credResult = await _identityService.ValidateCredentialsAsync(loginDto.Email, loginDto.Password);

                if (credResult.StatusCode != HttpStatusCode.OK)
                {
                    return new Response<AuthResponseDto>
                    {
                        Data= null,
                        StatusCode = credResult.StatusCode,
                        Message = credResult.Message
                    };
                }


                var user = await _userRepository.GetUserByIdAsync(credResult.Data);

                if (!user.EmailConfirmed)
                {
                    return Response<AuthResponseDto>.Fail("Please confirm your email before logging in.", HttpStatusCode.Unauthorized);
                }

                var roles = await _identityService.GetRolesAsync(user.Id);
                var (token, expiresAt) = _tokenService.GenerateToken(user, roles);

                var authResponse = new AuthResponseDto
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    UserId = user.Id,
                    Email = user.Email
                };

                return new Response<AuthResponseDto>
                {
                    Data = authResponse,
                    StatusCode = HttpStatusCode.OK,
                    Message = "User logged in successfully"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the login");
                return new Response<AuthResponseDto>
                {
                    Data = null,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"An error occurred while processing the login"
                };
            }
        }


        public async Task<Response<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);

            if (storedToken == null)
            {
                _logger.LogWarning("Refresh token not found: {TokenPrefix}...", dto.RefreshToken[..Math.Min(10, dto.RefreshToken.Length)]);
                return Response<AuthResponseDto>.Fail("Invalid refresh token.", HttpStatusCode.Unauthorized);
            }

            if (!storedToken.IsActive)
            {
                // Reuse of a revoked/expired token is a strong signal of token theft —
                // revoke every active session for this user as a precaution.
                _logger.LogWarning("Inactive/expired refresh token reuse attempt for user {UserId}", storedToken.UserId);
                await _refreshTokenRepository.RevokeAllActiveForUserAsync(storedToken.UserId);
                return Response<AuthResponseDto>.Fail("Refresh token is no longer valid. Please log in again.", HttpStatusCode.Unauthorized);
            }

            var user = await _userRepository.GetUserByIdAsync(storedToken.UserId);
            if (user == null)
                return Response<AuthResponseDto>.Fail("User not found.", HttpStatusCode.NotFound);

            // Rotate: revoke the old refresh token, issue a brand new one
            storedToken.IsRevoked = true;
            var newRefreshTokenValue = _tokenService.GenerateRefreshToken();
            storedToken.ReplacedByToken = newRefreshTokenValue;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            var authResponse = await IssueTokensAsync(user, newRefreshTokenValue);

            _logger.LogInformation("Refresh token rotated for user {UserId}", user.Id);
            return Response<AuthResponseDto>.Success(authResponse, "Token refreshed");
        }

        public async Task<Response<string>> LogoutAsync(RefreshTokenDto dto)
        {
            try
            {
                var storedToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);
                if (storedToken == null)
                    return Response<string>.Success(null, "Logged out."); // don't leak whether token existed

                storedToken.IsRevoked = true;
                await _refreshTokenRepository.UpdateAsync(storedToken);

                _logger.LogInformation("User {UserId} logged out, refresh token revoked", storedToken.UserId);
                return Response<string>.Success(null, "Logged out.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occured while loging out");
                return Response<string>.Fail("An error occured while loging out", HttpStatusCode.InternalServerError);
            }
        }

        private async Task<AuthResponseDto> IssueTokensAsync(User user, string existingRefreshTokenValue = null)
        {
            var roles = await _identityService.GetRolesAsync(user.Id);
            var (accessToken, expiresAt) = _tokenService.GenerateToken(user, roles);

            var refreshTokenValue = existingRefreshTokenValue ?? _tokenService.GenerateRefreshToken();

            // Only persist a new row if this isn't already the rotated replacement
            // being handed off from RefreshTokenAsync (which updates the old row itself)
            if (existingRefreshTokenValue == null)
            {
                var refreshToken = new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshTokenValue,
                    ExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays),
                    CreatedDate = DateTime.UtcNow,
                    IsRevoked = false
                };
                await _refreshTokenRepository.CreateAsync(refreshToken);
            }
            else
            {
                var newToken = new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshTokenValue,
                    ExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays),
                    CreatedDate = DateTime.UtcNow,
                    IsRevoked = false
                };
                await _refreshTokenRepository.CreateAsync(newToken);
            }

            return new AuthResponseDto
            {
                Token = accessToken,
                ExpiresAt = expiresAt,
                RefreshToken = refreshTokenValue,
                UserId = user.Id,
                Email = user.Email
            };
        }
    }
}
