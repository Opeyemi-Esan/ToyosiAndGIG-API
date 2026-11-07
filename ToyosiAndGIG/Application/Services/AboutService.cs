using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToyosiAndGIG.Application.Interface;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;
using ToyosiAndGIG.Infrastructure.Data;

namespace ToyosiAndGIG.Application.Services
{
    public class AboutService : IAboutService
    {
        public Task<Response<string>> DeleteAboutInfo()
        {
            throw new NotImplementedException();
        }

        public Task<Response<AboutInfo>> GetAboutIfoAsync(AboutInfo about)
        {
            throw new NotImplementedException();
        }

        public Task<Response<AboutInfo>> UpdateAboutIfoAsync(AboutInfo about)
        {
            throw new NotImplementedException();
        }

        public Task<Response<AboutInfo>> UploadAboutIfoAsync(AboutInfo about)
        {
            throw new NotImplementedException();
        }
    }
}
