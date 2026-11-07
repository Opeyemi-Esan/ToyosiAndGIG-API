using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Infrastructure.Interface
{
    public interface ISongRepository
    {
        Task<List<Song>> GetAllSongsAsync();
        Task<Song> GetSongAsync(int id);
        Task<Song> UploadSongAsync(Song song);
        Task<Song> UpdateSongAsync(Song song);
        Task DeleteSongAsync(int id);
    }
}
