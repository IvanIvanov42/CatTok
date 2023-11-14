using Expatery_API.Models;

namespace Expatery_API.Services
{
    public class DatabaseInstagramDataStorage : IInstagramDataStorage
    {
        private readonly InstagramDataStorageDbContext dbContext;

        public DatabaseInstagramDataStorage(InstagramDataStorageDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public string GetLatestTimestamp()
        {
            var latestTimestampEntity = dbContext.InstagramTimeStamps.FirstOrDefault();
            return latestTimestampEntity?.LatestTimeStamp ?? DateTime.MinValue.ToString();
        }

        public void UpdateLatestTimestamp(string newTimestamp)
        {
            var latestTimestampEntity = dbContext.InstagramTimeStamps.FirstOrDefault();
            if (latestTimestampEntity == null)
            {
                latestTimestampEntity = new InstagramTimeStamp { LatestTimeStamp = newTimestamp };
                dbContext.InstagramTimeStamps.Add(latestTimestampEntity);
            }
            else
            {
                latestTimestampEntity.LatestTimeStamp = newTimestamp;
            }

            dbContext.SaveChanges();
        }
    }
}
