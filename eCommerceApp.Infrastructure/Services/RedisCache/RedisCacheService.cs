using eCommerceApp.Application.Services.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace eCommerceApp.Infrastructure.Services.RedisCache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public async Task<bool> CreateNewSessionAsync(string userId, string sessionId)
        {
            var db = _redis.GetDatabase();

            var serverKeysResult = await db.ExecuteAsync("KEYS", $"session:{userId}:*");

            if (serverKeysResult.IsNull)
            {
                return false;
            }

            var serverKeys = (RedisResult[])serverKeysResult;

            if (serverKeys.Length > 0)
            {
                foreach (var serverKey in serverKeys)
                {
                    var key = serverKey.ToString();
                    await db.StringSetAsync(key, "false");
                }
            }

            var newSessionKey = $"session:{userId}:{sessionId}";
            bool isSessionCreated = await db.StringSetAsync(newSessionKey, "true", TimeSpan.FromMinutes(36000));

            return isSessionCreated;
        }

        public async Task<bool> DeleteSessionAsync(string sessionId)
        {
            return await _database.KeyDeleteAsync(sessionId);
        }

        public async Task<T?> GetSessionAsync<T>(string sessionId)
        {
            string? sessionJson = await _database.StringGetAsync(sessionId);
            return sessionJson != null ? JsonSerializer.Deserialize<T>(sessionJson) : default;
        }
    }
}
