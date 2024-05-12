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

        public async Task<List<Media>> GetMediaListAsync()
        {
            return await _dbContext.Medias.ToListAsync();
        }

        public async Task AddMediaAsync(List<Media> mediaList)
        {
            var mediaIds = mediaList.Select(m => m.Id);
            var existingMedias = await _dbContext.Medias
                                                 .Where(m => mediaIds.Contains(m.Id))
                                                 .ToListAsync();

            foreach (var newMedia in mediaList)
            {
                var existingMedia = existingMedias.FirstOrDefault(m => m.Id == newMedia.Id);

                if (existingMedia != null)
                {
                    _dbContext.Entry(existingMedia).CurrentValues.SetValues(newMedia);
                }
                else
                {
                    await _dbContext.Medias.AddAsync(newMedia);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
