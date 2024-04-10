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
            await _dbContext.Medias.AddRangeAsync(mediaList);
            await _dbContext.SaveChangesAsync();
        }
    }
}
