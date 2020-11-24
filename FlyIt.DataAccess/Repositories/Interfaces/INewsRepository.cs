using FlyIt.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public interface INewsRepository
    {
        public Task<News> AddNewsAsync(News news);

        public Task<News> GetNewsByIdAsync(int id);

        public Task<List<News>> GetNewsByAirportIdAsync(int airportId);

        public Task<News> RemoveNewsAsync(News news);

        public Task<News> UpdateNewsAsync(News news);
    }
}