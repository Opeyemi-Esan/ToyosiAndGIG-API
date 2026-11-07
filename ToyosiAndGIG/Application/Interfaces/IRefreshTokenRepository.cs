using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateAsync(RefreshToken token);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task UpdateAsync(RefreshToken token);
        Task RevokeAllActiveForUserAsync(Guid userId);
    }
}
