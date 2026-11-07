using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user, IList<string> roles);
        string GenerateRefreshToken();
    }
}
