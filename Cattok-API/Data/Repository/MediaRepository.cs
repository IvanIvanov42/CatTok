using Cattok_API.Data.Context;
using Cattok_API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Cattok_API.Data.Repository
{
    public class MediaRepository : IMediaRepository
    {
        private readonly MediaContext _dbContext;

        public MediaRepository(MediaContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<Media>> GetMediaListAsync(string userId)
        {
            return await _dbContext.Medias
                                   .Where(m => m.UserId == userId)
                                   .ToListAsync();
        }

        public async Task AddMediaAsync(string userId, List<Media> mediaList)
        {
            var mediaIds = mediaList.Select(m => m.Id).ToList();
            var existingMedias = await _dbContext.Medias
                                                 .Where(m => mediaIds.Contains(m.Id) && m.UserId == userId)
                                                 .ToListAsync();

            var newMedias = new List<Media>();

            foreach (var newMedia in mediaList)
            {
                var existingMedia = existingMedias.FirstOrDefault(m => m.Id == newMedia.Id);

                if (existingMedia != null)
                {
                    _dbContext.Entry(existingMedia).CurrentValues.SetValues(newMedia);
                }
                else
                {
                    newMedia.UserId = userId;
                    newMedias.Add(newMedia);
                }
            }

            if (newMedias.Count > 0)
            {
                await _dbContext.Medias.AddRangeAsync(newMedias);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
