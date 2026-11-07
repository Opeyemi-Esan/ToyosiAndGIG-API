using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Security.Claims;
using ToyosiAndGIG.Application;
using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Application.Models;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly Helper _helper;
        private readonly ILogger<IdentityService> _logger;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        public IdentityService(UserManager<User> userManager, SignInManager<User> signInManager, Helper helper, ILogger<IdentityService> logger, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _helper = helper;
            _logger = logger;
            _roleManager = roleManager;
        }

        //public async Task<Response<Guid>> RegisterAsync(RegisterDto register)
        //{
        //    try
        //    {
        //        //if (string.IsNullOrWhiteSpace(register.Role) || Roles.IsPubliclyAssignable(register.Role) == false)
        //        //{
        //        //    _logger.LogWarning("Registration attempted with invalid or disallowed role {Role} for {Email}", register.Role, register.Email);
        //        //    return Response<Guid>.Fail("Invalid role selected.", HttpStatusCode.BadRequest);
        //        //}

        //        var existing = await _userManager.FindByEmailAsync(register.Email);
        //        if (existing != null)
        //        {
        //            return new Response<Guid>
        //            {
        //                Data = Guid.Empty,
        //                StatusCode = HttpStatusCode.Conflict,
        //                Message = "User with this email already exists"
        //            };
        //        }

        //        //var userEmail = _helper.GetUserEmailFromToken();

        //        var user = new User
        //        {
        //            Id = Guid.NewGuid(),
        //            FirstName = register.FirstName,
        //            LastName = register.LastName,
        //            Email = register.Email,
        //            UserName = register.Username,
        //            PhoneNumber = register.PhoneNumber,
        //            CreatedDate = DateTime.UtcNow,
        //        };

        //        var result = await _userManager.CreateAsync(user, register.Password);
        //        if (!result.Succeeded)
        //        {
        //            _logger.LogInformation("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        //            return Response<Guid>.Fail("User creation failed, Try again", HttpStatusCode.InternalServerError);
        //        }

        //        //var roleResult = await _userManager.AddToRoleAsync(user, register.Role);
        //        //if (!roleResult.Succeeded)
        //        //{
        //        //    // User was created but role assignment failed — clean up to avoid an orphaned roleless account
        //        //    await _userManager.DeleteAsync(user);
        //        //    var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
        //        //    _logger.LogError("Role assignment failed for {Email}, user rolled back: {Errors}", register.Email, errors);
        //        //    return Response<Guid>.Fail("Failed to assign role. Please try again.", HttpStatusCode.InternalServerError);
        //        //}

        //        return new Response<Guid>
        //        {
        //            Data = user.Id,
        //            StatusCode = HttpStatusCode.Created,
        //            Message = "User Created Successfully"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while registering a user.");
        //        return Response<Guid>.Fail("An error occurred while registering a user.", HttpStatusCode.InternalServerError);
        //    }


        //}


        public async Task<Response<Guid>> RegisterAsync(CreateUserRequest request)
        {
            var roleIsValid = request.IsAdminAction
                ? Roles.IsAdminAssignable(request.Role)
                : Roles.IsPubliclyAssignable(request.Role);

            if (string.IsNullOrWhiteSpace(request.Role) || !roleIsValid)
            {
                _logger.LogWarning("Registration attempted with invalid or disallowed role {Role} for {Email}", request.Role, request.Email);
                return Response<Guid>.Fail("Invalid role selected.", HttpStatusCode.BadRequest);
            }

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
                return Response<Guid>.Fail("Email already registered.", HttpStatusCode.Conflict);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = string.IsNullOrWhiteSpace(request.UserName) ? request.Email : request.UserName,
                PhoneNumber = request.PhoneNumber,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Registration failed for {Email}: {Errors}", request.Email, errors);
                return Response<Guid>.Fail(errors, HttpStatusCode.BadRequest);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                _logger.LogError("Role assignment failed for {Email}, user rolled back: {Errors}", request.Email, errors);
                return Response<Guid>.Fail("Failed to assign role. Please try again.", HttpStatusCode.InternalServerError);
            }

            _logger.LogInformation("User {UserId} registered with role {Role}", user.Id, request.Role);
            return Response<Guid>.Success(user.Id, "User created", HttpStatusCode.Created);
        }

        public async Task<Response<Guid>> ValidateCredentialsAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Response<Guid>.Fail("Invalid email or password.", HttpStatusCode.Unauthorized);

                var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

                if (result.IsLockedOut)
                    return Response<Guid>.Fail("Account locked due to multiple failed attempts.", HttpStatusCode.Forbidden);

                if (!result.Succeeded)
                    return Response<Guid>.Fail("Invalid email or password.", HttpStatusCode.Unauthorized);

                return Response<Guid>.Success(user.Id, "Credentials valid", HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while Validating credentials");
                return Response<Guid>.Fail("An error occured while Validating credentials", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<IList<string>> GetRolesAsync(Guid userId)
        { 
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user == null ? new List<string>() : await _userManager.GetRolesAsync(user);
            
        }

        public async Task<Response<List<string>>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles
                        .AsNoTracking()
                        .Select(r => r.Name!)
                        .ToListAsync();
            return Response<List<string>>.Success(Roles.AdminAssignable.ToList(), "Roles retrieved successfully.", HttpStatusCode.OK);
        }

        public async Task<Response<string>> ConfirmEmailAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Response<string>.Fail("User not found.", HttpStatusCode.NotFound);

            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? Response<string>.Success(null, "Email confirmed.")
                : Response<string>.Fail(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task<Response<string>> ResetPasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Response<string>.Fail("User not found.", HttpStatusCode.NotFound);

            // RemovePassword/AddPassword avoids needing a reset token, since we already
            // verified identity via the OTP — mirrors the pattern from your WebForms flow
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, newPassword);

            return result.Succeeded
                ? Response<string>.Success(null, "Password reset.")
                : Response<string>.Fail(string.Join("; ", result.Errors.Select(e => e.Description)));
        }


        public async Task<Response<string>> DeleteAccountAsync(Guid userId, string deletedBy)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Response<string>.Fail("User not found.", HttpStatusCode.NotFound);

            var originalEmail = user.Email; // capture BEFORE mutating it

            var tombstonePrefix = $"deleted_{user.Id}_";
            user.Email = tombstonePrefix + originalEmail;
            user.UserName = tombstonePrefix + user.UserName;
            user.NormalizedEmail = user.Email.ToUpperInvariant();
            user.NormalizedUserName = user.UserName.ToUpperInvariant();

            user.IsDeleted = true;
            user.DeletedDate = DateTime.UtcNow;
            user.DeletedBy = deletedBy; // the acting admin's email, or "self"

            await _userManager.RemovePasswordAsync(user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogError("Account deletion failed for user {UserId}: {Errors}", userId, errors);
                return Response<string>.Fail(errors, HttpStatusCode.InternalServerError);
            }

            _logger.LogInformation("Account soft-deleted for user {UserId} by {DeletedBy}", userId, deletedBy);
            return Response<string>.Success(null, "Account deleted successfully.");
        }

        public Task<Response<string>> UpdateUserAsync(Guid userId, UpdateUserDto dto, string updatedBy)
        {
            try
            {
                var user = _userManager.FindByIdAsync(userId.ToString()).Result;
                if (user == null)
                {
                    return Task.FromResult(Response<string>.Fail("User not found.", HttpStatusCode.NotFound));
                }

                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.PhoneNumber = dto.PhoneNumber;
                user.LastUpdatedDate = DateTime.UtcNow;
                user.LastUpdatedBy = updatedBy;
                var result = _userManager.UpdateAsync(user).Result;

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    _logger.LogError("User update failed for user {UserId}: {Errors}", userId, errors);
                    return Task.FromResult(Response<string>.Fail(errors, HttpStatusCode.InternalServerError));
                }
                return Task.FromResult(Response<string>.Success(null, "User updated successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user {UserId}", userId);
                return Task.FromResult(Response<string>.Fail("An error occurred while updating the user.", HttpStatusCode.InternalServerError));
            }
        }
    }
}
