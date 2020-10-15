using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface INewsService
    {
        public Task<Result<NewsDTO>> AddNews(string title, string imageurl, string body, int airportId, ClaimsPrincipal user);
    }
}