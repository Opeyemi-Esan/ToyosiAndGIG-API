using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Infrastructure.Interfaces;

namespace ToyosiAndGIG.Infrastructure.Repository
{
    public class AwardRepository : IAwardRepository
    {
        public Task<Award> CreateAwardAsync(Award award)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAwardAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Award>> GetAllAwardAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Award> GetAwardAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Award> UpdateAwardAsync(Award award)
        {
            throw new NotImplementedException();
        }
    }
}
