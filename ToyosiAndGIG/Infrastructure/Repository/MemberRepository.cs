using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Infrastructure.Interface;

namespace ToyosiAndGIG.Infrastructure.Repository
{
    public class MemberRepository : IMemberRepository
    {
        public Task<Member> CreateMemberAsync(Member member)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMemberAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Member>> GetAllMembersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetMemberAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Member> UpdateMemberAsync(Member member)
        {
            throw new NotImplementedException();
        }
    }
}
