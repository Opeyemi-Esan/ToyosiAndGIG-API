using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Domain.Response;
using static ToyosiAndGIG.Application.Dtos.HelperDtoClass;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Response<string>> SendEmailConfirmationOtpAsync(string email);
        Task<Response<string>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
        Task<Response<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<Response<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<Response<string>> UpdateUserAsync(Guid userId, UpdateUserDto dto, string updatedBy);
        Task<Response<string>> UpdateUserByEmailAsync(string email, UpdateUserDto dto, string updatedBy);
        Task<Response<string>> DeleteAccountAsync(Guid userId, string deletedBy);
    }
}
