using FlyIt.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
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

        public Task<List<News>> GetNewsAsync()
        {
            return context.News.ToListAsync();
        }

        public Task<News> GetNewsByIdAsync(int id)
        {
            return context.News.SingleOrDefaultAsync(news => news.Id == id);
        }

        public async Task<News> GetNewsByTitle(string Title)
        {
            return await context.News.SingleOrDefaultAsync(n => n.Title == Title);
        }

        public Task<News> GetNewsByTitleAsync(string title)
        {
            throw new NotImplementedException();
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
    }
}