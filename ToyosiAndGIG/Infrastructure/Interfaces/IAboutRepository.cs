using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Infrastructure.Interface
{
    public interface IAboutRepository
    {
        Task<AboutInfo> GetAboutIfoAsync(AboutInfo about);
        Task<AboutInfo> UploadAboutIfoAsync(AboutInfo about);
        Task<AboutInfo> UpdateAboutIfoAsync(AboutInfo about);
        Task DeleteAboutInfo();
    }
}
