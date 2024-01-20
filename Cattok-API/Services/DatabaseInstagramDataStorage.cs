using Cattok_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Cattok_API.Services
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

        public async Task<string> GetLatestTimestamp()
        {
            var latestTimestampEntity = await dbContext.InstagramTimeStamps.FirstOrDefaultAsync();
            return latestTimestampEntity?.LatestTimeStamp
                   ?? DateTime.MinValue.ToString();
        }

        public async Task UpdateLatestTimestamp(string newTimestamp)
        {
            var latestTimestampEntity = await dbContext.InstagramTimeStamps.FirstOrDefaultAsync();
            if (latestTimestampEntity == null)
            {
                latestTimestampEntity = new InstagramTimeStamp { LatestTimeStamp = newTimestamp };
                await dbContext.InstagramTimeStamps.AddAsync(latestTimestampEntity);
            }
            else
            {
                latestTimestampEntity.LatestTimeStamp = newTimestamp;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
