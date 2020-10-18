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

        public Task<Result<NewsDTO>> DeleteNews(int id, ClaimsPrincipal claims);

        public Task<Result<List<NewsDTO>>> GetNews(ClaimsPrincipal claims, int airportId);

        public Task<Result<NewsDTO>> UpdateNews(int id, string Title, string Imageurl, string Body, ClaimsPrincipal claims);
    }
}