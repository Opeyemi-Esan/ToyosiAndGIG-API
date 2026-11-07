using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Interface
{
    public interface IAboutService
    {
        Task<Response<AboutInfo>> GetAboutIfoAsync(AboutInfo about);
        Task<Response<AboutInfo>> UploadAboutIfoAsync(AboutInfo about);
        Task<Response<AboutInfo>> UpdateAboutIfoAsync(AboutInfo about);
        Task<Response<string>> DeleteAboutInfo();
    }
}
