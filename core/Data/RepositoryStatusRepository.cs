using DotnetStatus.Core.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data
{
    public class RepositoryStatusRepository : IRepositoryStatusRepository
    {
        private readonly IMongoClient _client;
        private readonly DatabaseConfiguration _config;

        public RepositoryStatusRepository(IOptions<DatabaseConfiguration> options, IMongoClient mongoClient)
        {
            _client = mongoClient;
            _config = options.Value;
        }

        public async Task SaveAsync(RepositoryStatusResult repoStatus, CancellationToken cancellationToken)
        {
            var collection = GetCollection();
            var filter = Builders<RepositoryStatusResult>.Filter.Eq("_id", repoStatus.Id);
            var result = await collection.FindOneAndReplaceAsync(filter, repoStatus);

            if (result == null)
                await collection.InsertOneAsync(repoStatus, new InsertOneOptions(), cancellationToken);
        }

        public async Task<RepositoryStatusResult> GetAsync(string id, CancellationToken cancellationToken)
        {
            var collection = GetCollection();
            var filter = Builders<RepositoryStatusResult>.Filter.Eq("_id", id);
            var cursor = await collection.FindAsync(filter);
            var result = await cursor.FirstOrDefaultAsync();
            return result;
        }

        private IMongoCollection<RepositoryStatusResult> GetCollection()
        {
            var db = _client.GetDatabase(_config.DatabaseName);
            var collection = db.GetCollection<RepositoryStatusResult>(nameof(RepositoryStatusResult).ToLower());
            return collection;
        }
    }
}
