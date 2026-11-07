using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Services
{
    public class AwardService : IAwardServices
    {
        public Task<Response<Award>> CreateAwardAsync(Award award)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> DeleteAwardAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<List<Award>>> GetAllAwardAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Response<Award>> GetAwardAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<Award>> UpdateAwardAsync(Award award)
        {
            throw new NotImplementedException();
        }
    }
}
