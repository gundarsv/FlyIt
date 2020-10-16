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

        public Task<List<News>> GetNewsAsync();

        public Task<News> GetNewsByIdAsync(int id);

        public Task<News> GetNewsByTitleAsync(string title);

        public Task<News> RemoveNewsAsync(News news);
    }
}