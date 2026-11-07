using Microsoft.EntityFrameworkCore;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Infrastructure.Data;
using ToyosiAndGIG.Infrastructure.Interfaces;

namespace ToyosiAndGIG.Infrastructure.Repository
{
    public class AccountConfirmationRepository:IAccountConfirmationRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountConfirmationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AccountConfirmation> CreateAsync(AccountConfirmation confirmation)
        {
            _context.AccountConfirmations.Add(confirmation);
            await _context.SaveChangesAsync();
            return confirmation;
        }

        public async Task<AccountConfirmation> GetLatestValidOtpAsync(Guid userId, OtpPurpose purpose)
        {
            var validOtp = await _context.AccountConfirmations
                .Where(a => a.UserId == userId
                         && a.Purpose == purpose
                         && !a.IsUsed
                         && a.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(a => a.CreatedDate)
                .FirstOrDefaultAsync();

            return validOtp;
        }

        public async Task<AccountConfirmation> UpdateAsync(AccountConfirmation confirmation)
        {
            _context.AccountConfirmations.Update(confirmation);
            await _context.SaveChangesAsync();
            return confirmation;
        }

        public async Task InvalidatePendingOtpsAsync(Guid userId, OtpPurpose purpose)
        {
            // Prevents a stale, previously-issued OTP from still being valid after a new one is sent
            var pending = await _context.AccountConfirmations
                .Where(a => a.UserId == userId && a.Purpose == purpose && !a.IsUsed)
                .ToListAsync();

            foreach (var otp in pending)
                otp.IsUsed = true;

            if (pending.Count > 0)
                await _context.SaveChangesAsync();
        }

    }
}
