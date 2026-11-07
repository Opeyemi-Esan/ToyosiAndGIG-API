using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Domain.Entities;
using static ToyosiAndGIG.Application.Dtos.HelperDtoClass;

namespace ToyosiAndGIG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("AuthPolicy")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IIdentityService _identityService;

        public AccountController(IAccountService accountService, IIdentityService identityService)
        {
            _accountService = accountService;
            _identityService = identityService;
        }

        [HttpPost("send-confirmation")]
        public async Task<IActionResult> SendConfirmation([FromBody] ResendOtpDto resendOtpDto)
        {
            var response = await _accountService.SendEmailConfirmationOtpAsync(resendOtpDto.Email);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            var response = await _accountService.ConfirmEmailAsync(confirmEmailDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var response = await _accountService.ForgotPasswordAsync(forgotPasswordDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = await _accountService.ResetPasswordAsync(resetPasswordDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize(Roles = $"{Roles.Owner}, {Roles.Manager}")]
        [HttpPost("get-userroles")]
        public async Task<IActionResult> GetRoles()
        {
            var response = await _identityService.GetAllRolesAsync();
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateUserDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid token." });

            var updatedBy = User.FindFirstValue(ClaimTypes.Email) ?? "self";
            var response = await _accountService.UpdateUserAsync(userId, dto, updatedBy);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("update-profile/{email}")]
        [Authorize(Roles = $"{Roles.Manager},{Roles.Owner}")]
        public async Task<IActionResult> UpdateProfileByAdmin(string email, UpdateUserDto dto)
        {
            var updatedBy = User.FindFirstValue(ClaimTypes.Email);
            var response = await _accountService.UpdateUserByEmailAsync(email, dto, updatedBy);
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid token." });

            var response = await _accountService.DeleteAccountAsync(userId, deletedBy: "self");
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("delete-account/{userId:guid}")]
        [Authorize(Roles = $"{Roles.Manager},{Roles.Owner}")]
        public async Task<IActionResult> DeleteAccountByAdmin(Guid userId)
        {
            var adminEmail = User.FindFirstValue(ClaimTypes.Email);
            var response = await _accountService.DeleteAccountAsync(userId, deletedBy: adminEmail);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
