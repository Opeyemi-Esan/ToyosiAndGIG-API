using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Interface
{
    public interface IMemberService
    {
        Task<Response<List<Member>>> GetAllMembersAsync();
        Task<Response<Member>> GetMemberAsync(int id);
        Task<Response<Member>> CreateMemberAsync(Member member);
        Task<Response<Member>> UpdateMemberAsync(Member member);
        Task<Response<string>> DeleteMemberAsync(int id);
    }
}
