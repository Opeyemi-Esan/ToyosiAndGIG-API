using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Infrastructure.Interface
{
    public interface IMemberRepository
    {
        Task<List<Member>> GetAllMembersAsync();
        Task<Member> GetMemberAsync(int id);
        Task<Member> CreateMemberAsync(Member member);
        Task<Member> UpdateMemberAsync(Member member);
        Task DeleteMemberAsync(int id);
    }
}
