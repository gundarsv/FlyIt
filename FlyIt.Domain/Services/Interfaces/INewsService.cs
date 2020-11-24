using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public interface INewsService
    {
        public Task<Result<NewsDTO>> AddNews(string title, string imageurl, string imageName, string body, int airportId, ClaimsPrincipal user);

        public Task<Result<NewsDTO>> DeleteNews(int id, ClaimsPrincipal claims);

        public Task<Result<List<NewsDTO>>> GetNewsByAirportId(int airportId);

        public Task<Result<List<NewsDTO>>> GetNewsByAirportIata(string iata);

        public Task<Result<NewsDTO>> UpdateNews(int id, string Title, string Imageurl, string imageName, string Body, ClaimsPrincipal claims);
    }
}