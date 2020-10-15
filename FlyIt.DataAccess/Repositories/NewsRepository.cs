using FlyIt.DataAccess.Entities;
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
    }
}