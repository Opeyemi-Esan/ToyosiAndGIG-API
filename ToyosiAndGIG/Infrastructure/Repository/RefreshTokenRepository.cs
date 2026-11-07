using Microsoft.EntityFrameworkCore;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Infrastructure.Data;

namespace ToyosiAndGIG.Infrastructure.Repository
{
    public class RefreshTokenRepository:IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> CreateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
            => await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);

        public async Task UpdateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllActiveForUserAsync(Guid userId)
        {
            // Used on logout / password reset — kills every active session for this user
            var activeTokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in activeTokens)
                token.IsRevoked = true;

            if (activeTokens.Count > 0)
                await _context.SaveChangesAsync();
        }
    }
}
