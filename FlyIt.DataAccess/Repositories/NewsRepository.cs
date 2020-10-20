using FlyIt.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.DataAccess.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly FlyItContext context;

        public NewsRepository(FlyItContext context)
        {
            this.context = context;
        }

        public async Task<News> AddNewsAsync(News news)
        {
            var entityEntry = await context.News.AddAsync(news);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return entityEntry.Entity;
        }

        public Task<List<News>> GetNewsByAirportIdAsync(int airportId)
        {
            return context.News.Where(n => n.AirportId == airportId).ToListAsync();
        }

        public Task<News> GetNewsByIdAsync(int id)
        {
            return context.News.SingleOrDefaultAsync(news => news.Id == id);
        }

        public async Task<News> RemoveNewsAsync(News news)
        {
            var removedNews = context.News.Remove(news);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return removedNews.Entity;
        }

        public async Task<News> UpdateNewsAsync(News news)
        {
            var newsToUpdate = await context.News.FirstOrDefaultAsync(n => n.Id == news.Id);

            context.Entry(newsToUpdate).CurrentValues.SetValues(news);

            var result = await context.SaveChangesAsync();

            if (result < 1)
            {
                return null;
            }

            return await context.News.FirstOrDefaultAsync(n => n.Id == news.Id);
        }
    }
}