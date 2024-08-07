﻿using Cattok_API.Data.Models;

namespace Cattok_API.Data.Repository
{
    public interface IMediaRepository
    {
        Task<List<Media>> GetMediaListAsync();
        Task AddMediaAsync(List<Media> mediaList);
    }
}
