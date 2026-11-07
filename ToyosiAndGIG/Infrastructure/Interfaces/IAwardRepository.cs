using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Infrastructure.Interfaces
{
    public interface IAwardRepository
    {
        Task<List<Award>> GetAllAwardAsync();
        Task<Award> GetAwardAsync(int id);
        Task<Award> CreateAwardAsync(Award award);
        Task<Award> UpdateAwardAsync(Award award);
        Task DeleteAwardAsync(int id);
    }
}
