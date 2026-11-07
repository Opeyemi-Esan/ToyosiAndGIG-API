using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Interface
{
    public interface IUserService
    {
        Task<Response<List<User>>> GetAllAsync();
        Task<Response<User>> GetUserByIdAsync(Guid id);
        Task<Response<User>> GetUserByEmailAsync(string email);
        Task<Response<User>> CreateUserAsync(User user);
        Task<Response<User>> UpdateUserAsync(User user);
        Task<Response<string>> DeleteUserAsync(Guid id);
    }
}
