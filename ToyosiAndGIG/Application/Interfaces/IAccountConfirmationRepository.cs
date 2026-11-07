using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IAccountConfirmationRepository
    {
        Task<AccountConfirmation> CreateAsync(AccountConfirmation confirmation);
        Task<AccountConfirmation> GetLatestValidOtpAsync(Guid userId, OtpPurpose purpose);
        Task<AccountConfirmation> UpdateAsync(AccountConfirmation confirmation);
        Task InvalidatePendingOtpsAsync(Guid userId, OtpPurpose purpose);
    }
}
