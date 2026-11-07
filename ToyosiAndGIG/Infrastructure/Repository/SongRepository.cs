using Microsoft.EntityFrameworkCore;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Infrastructure.Data;
using ToyosiAndGIG.Infrastructure.Interface;

namespace ToyosiAndGIG.Infrastructure.Repository
{
    public class SongRepository : ISongRepository
    {
        private ApplicationDbContext _context;
        public SongRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<List<Song>> GetAllSongsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Song> GetSongAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Song> UpdateSongAsync(Song song)
        {
            throw new NotImplementedException();
        }

        public Task<Song> UploadSongAsync(Song song)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSongAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
