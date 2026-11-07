using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Models;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IIdentityService
    {
        //Task<Response<Guid>> RegisterAsync(RegisterDto register);
        Task<Response<Guid>> RegisterAsync(CreateUserRequest request);
        Task<Response<Guid>> ValidateCredentialsAsync(string email, string password);
        Task<IList<string>> GetRolesAsync(Guid userId);
        Task<Response<string>> ConfirmEmailAsync(Guid userId);
        Task<Response<string>> ResetPasswordAsync(Guid userId, string newPassword);
        Task<Response<List<string>>> GetAllRolesAsync();
        Task<Response<string>> UpdateUserAsync(Guid userId, UpdateUserDto dto, string updatedBy);
        Task<Response<string>> DeleteAccountAsync(Guid userId, string deletedBy);
    }
}
