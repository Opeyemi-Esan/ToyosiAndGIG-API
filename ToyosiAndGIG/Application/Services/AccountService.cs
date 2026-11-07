using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Cryptography;
using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;
using ToyosiAndGIG.Infrastructure.Repository;
using static ToyosiAndGIG.Application.Dtos.HelperDtoClass;

namespace ToyosiAndGIG.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountConfirmationRepository _confirmationRepository;
        private readonly IIdentityService _identityService;
        private readonly IEmailServices _emailService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<AccountService> _logger;
        private readonly IEmployeeRepository _employeeRepository;

        private const int OtpExpiryMinutes = 10;

        public AccountService(
            IUserRepository userRepository,
            IAccountConfirmationRepository confirmationRepository,
            IIdentityService identityService,
            IEmailServices emailService,
            ILogger<AccountService> logger,
            IRefreshTokenRepository refreshTokenRepository,
            IEmployeeRepository employeeRepository
            )
        {
            _userRepository = userRepository;
            _confirmationRepository = confirmationRepository;
            _identityService = identityService;
            _emailService = emailService;
            _logger = logger;
            _refreshTokenRepository = refreshTokenRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Response<string>> SendEmailConfirmationOtpAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                    return Response<string>.Fail("No account found with this email.", HttpStatusCode.NotFound);

                if (user.EmailConfirmed)
                    return Response<string>.Fail("Email is already confirmed.", HttpStatusCode.BadRequest);

                await IssueOtpAsync(user, OtpPurpose.EmailConfirmation, "EmailConfirmation");
                return Response<string>.Success(null, "Confirmation code sent to your email.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured");
                return Response<string>.Fail("An error occured", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response<string>> ConfirmEmailAsync(ConfirmEmailDto dto)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (user == null)
                    return Response<string>.Fail("No account found with this email.", HttpStatusCode.NotFound);

                var otpRecord = await _confirmationRepository.GetLatestValidOtpAsync(user.Id, OtpPurpose.EmailConfirmation);
                if (otpRecord == null || otpRecord.OTP != dto.OTP)
                {
                    _logger.LogWarning("Invalid or expired email confirmation OTP attempt for {Email}", dto.Email);
                    return Response<string>.Fail("Invalid or expired code.", HttpStatusCode.BadRequest);
                }

                otpRecord.IsUsed = true;
                await _confirmationRepository.UpdateAsync(otpRecord);

                var result = await _identityService.ConfirmEmailAsync(user.Id);
                if (!result.IsSuccess)
                    return Response<string>.Fail(result.Message, result.StatusCode);

                _logger.LogInformation("Email confirmed for user {UserId}", user.Id);
                return Response<string>.Success(null, "Email confirmed successfully.", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occured while confirming email");
                return Response<string>.Fail("An error occured while confirming email", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(dto.Email);

                // Don't reveal whether the email exists same generic message either way,
                // otherwise this endpoint becomes a way to enumerate registered emails.
                if (user == null)
                {
                    _logger.LogInformation("Forgot-password requested for non-existent email {Email}", dto.Email);
                    return Response<string>.Success(null, "A reset code has been sent to your email.");
                }

                await IssueOtpAsync(user, OtpPurpose.PasswordReset, "PasswordReset");
                return Response<string>.Success(null, "A reset code has been sent to your email.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured sending otp");
                return Response<string>.Fail("An error occured sending otp", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response<string>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return Response<string>.Fail("Invalid request.", HttpStatusCode.BadRequest);

            var otpRecord = await _confirmationRepository.GetLatestValidOtpAsync(user.Id, OtpPurpose.PasswordReset);
            if (otpRecord == null || otpRecord.OTP != dto.OTP)
            {
                _logger.LogWarning("Invalid or expired password reset OTP attempt for {Email}", dto.Email);
                return Response<string>.Fail("Invalid or expired code.", HttpStatusCode.BadRequest);
            }

            otpRecord.IsUsed = true;
            await _confirmationRepository.UpdateAsync(otpRecord);

            var result = await _identityService.ResetPasswordAsync(user.Id, dto.NewPassword);
            if (!result.IsSuccess)
                return Response<string>.Fail(result.Message, result.StatusCode);

            // Revoke every active refresh token for this user — forces all other
            // logged-in devices/sessions to require a fresh login with the new password.
            await _refreshTokenRepository.RevokeAllActiveForUserAsync(user.Id);

            _logger.LogInformation("Password reset for user {UserId}; all active sessions revoked", user.Id);
            return Response<string>.Success(null, "Password reset successfully. You've been logged out of all other sessions.");
        }

        private async Task IssueOtpAsync(User user, OtpPurpose purpose, string purposeLabel)
        {
            // Invalidate any previous unused OTP for this purpose so only the newest one works
            await _confirmationRepository.InvalidatePendingOtpsAsync(user.Id, purpose);

            var otp = GenerateOtp();

            var confirmation = new AccountConfirmation
            {
                UserId = user.Id,
                OTP = otp,
                Purpose = purpose,
                IsUsed = false,
                ExpiryDate = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
                CreatedDate = DateTime.UtcNow
            };

            await _confirmationRepository.CreateAsync(confirmation);
            await _emailService.SendOtpEmailAsync(user.Email, otp, purposeLabel);
        }

        private static string GenerateOtp()
        {
            // Cryptographically random 6-digit OTP — avoid Random for anything security-sensitive
          var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            return otp;
        }


        // Self-service: caller updates their own basic fields only
        public async Task<Response<string>> UpdateUserAsync(Guid userId, UpdateUserDto dto, string updatedBy)
        {
            var result = await UpdateInternalAsync(userId, dto, updatedBy, isAdminAction: false);
            return result;
        }

        // Admin: resolves target by email, can also update Employee fields
        public async Task<Response<string>> UpdateUserByEmailAsync(string email, UpdateUserDto dto, string updatedBy)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return Response<string>.Fail("User not found.", HttpStatusCode.NotFound);

            return await UpdateInternalAsync(user.Id, dto, updatedBy, isAdminAction: true);
        }

        private async Task<Response<string>> UpdateInternalAsync(Guid userId, UpdateUserDto dto, string updatedBy, bool isAdminAction)
        {
            try
            {
                // AspNetUsers mutation now goes through Identity, same rhythm as DeleteAccountAsync
                var identityResult = await _identityService.UpdateUserAsync(userId, dto, updatedBy);
                if (!identityResult.IsSuccess)
                    return identityResult;

                var employee = await _employeeRepository.GetByUserIdAsync(userId);

                if (employee != null && isAdminAction)
                {
                    employee.Position = dto.Position ?? employee.Position;
                    employee.Department = dto.Department ?? employee.Department;
                    await _employeeRepository.UpdateAsync(employee);

                    _logger.LogInformation("Employee {UserId} profile updated (AspNetUsers + Employees) by {UpdatedBy}", userId, updatedBy);
                    return Response<string>.Success(null, "Employee profile updated successfully.");
                }

                _logger.LogInformation("User {UserId} profile updated (AspNetUsers only) by {UpdatedBy}", userId, updatedBy);
                return Response<string>.Success(null, "Profile updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user {UserId}", userId);
                return Response<string>.Fail("Something went wrong, try again.", HttpStatusCode.InternalServerError);
            }
        }


        public async Task<Response<string>> DeleteAccountAsync(Guid userId, string deletedBy)
        {
            var result = await _identityService.DeleteAccountAsync(userId, deletedBy);
            if (!result.IsSuccess)
                return result;

            await _refreshTokenRepository.RevokeAllActiveForUserAsync(userId);

            _logger.LogInformation("All sessions revoked following account deletion for user {UserId}", userId);
            return Response<string>.Success(null, "Account deleted successfully.");
        }
    }
}
