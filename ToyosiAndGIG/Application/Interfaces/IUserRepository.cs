using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
    }

}
