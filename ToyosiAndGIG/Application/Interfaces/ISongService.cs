using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Interface
{
    public interface ISongService
    {
        Task<Response<List<Song>>> GetAllSongsAsync();
        Task<Response<Song>> GetSongAsync(int id);
        Task<Response<Song>> UploadSongAsync(Song song);
        Task<Response<Song>> UpdateSongAsync(Song song);
        Task<Response<string>> DeleteSongAsync(int id);
    }
}
