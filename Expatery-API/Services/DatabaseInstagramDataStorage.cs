using Expatery_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Expatery_API.Services
{
    public class DatabaseInstagramDataStorage : IInstagramDataStorage
    {
        private readonly InstagramDataStorageDbContext dbContext;

        public DatabaseInstagramDataStorage(InstagramDataStorageDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Media>> GetMediaListAsync()
        {
            return await dbContext.Medias.ToListAsync();
        }

        public async Task AddMediaAsync(List<Media> mediaList)
        {
            foreach (var media in mediaList)
            {
                await dbContext.Medias.AddAsync(media);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}