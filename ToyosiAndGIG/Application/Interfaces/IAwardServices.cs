using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IAwardServices
    {
        Task<Response<List<Award>>> GetAllAwardAsync();
        Task<Response<Award>> GetAwardAsync(int id);
        Task<Response<Award>> CreateAwardAsync(Award award);
        Task<Response<Award>> UpdateAwardAsync(Award award);
        Task<Response<string>> DeleteAwardAsync(int id);
    }
}
