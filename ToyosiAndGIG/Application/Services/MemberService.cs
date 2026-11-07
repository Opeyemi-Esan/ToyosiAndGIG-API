

using ToyosiAndGIG.Application.Interface;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Services
{
    public class MemberService : IMemberService
    {
        public Task<Response<Member>> CreateMemberAsync(Member member)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> DeleteMemberAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<List<Member>>> GetAllMembersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Response<Member>> GetMemberAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<Member>> UpdateMemberAsync(Member member)
        {
            throw new NotImplementedException();
        }
    }
}
