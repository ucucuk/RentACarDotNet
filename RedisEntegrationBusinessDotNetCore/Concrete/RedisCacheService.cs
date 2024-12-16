using Newtonsoft.Json;
using RedisEntegrationBusinessDotNetCore.Abstract;
using StackExchange.Redis;
using System.Text;

namespace RedisEntegrationBusinessDotNetCore.Concrete
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _cache;

        public RedisCacheService(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
            _cache = redisConnection.GetDatabase();
        }

        public async Task<bool> Clear(string key)
        {
            bool _isKeyExist = _cache.KeyExists(key);
            if (_isKeyExist == true)
            {
                await _cache.KeyDeleteAsync(key);
                return true;
            }
            return false;
        }

        public void ClearAll()
        {
            var redisEndpoints = _redisConnection.GetEndPoints(true);
            foreach (var redisEndpoint in redisEndpoints)
            {
                var redisServer = _redisConnection.GetServer(redisEndpoint);
                redisServer.FlushAllDatabases();
            }
        }

        //public async Task<string> GetValueAsync(string key)
        //{
        //    return await _cache.StringGetAsync(key);
        //}

        public async Task<string> GetValueAsync(string key)
        {
            var value = await _cache.StringGetAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject(value).ToString();
            }
            return default;
        }

        public async Task<bool> SetValueAsync<T>(string key, T value)
        {
            //TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return await _cache.StringSetAsync(key, JsonConvert.SerializeObject(value), TimeSpan.FromHours(1));
        }
    }
}
