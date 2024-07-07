using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace backend.Dao.Repository
{
        public class ResponseCacheService : IResponseCacheService
        {
        //IConnectionMultiplexer để kết nối với Redis
        public readonly IConnectionMultiplexer connectionMultiplexer;
            public ResponseCacheService(IConnectionMultiplexer _connectionMultiplexer)
            {
                connectionMultiplexer = _connectionMultiplexer; 
            }

        public async Task<string> GetCacheResponseAsync(string cache)
            {
                if (string.IsNullOrWhiteSpace(cache))
                {
                    return null;
                }
                //kết nối với Redis
                var redisDb = connectionMultiplexer.GetDatabase();
            //lấy dữ liệu
                var cachedResponse = await redisDb.StringGetAsync(cache);

                return cachedResponse;
            }

        public async Task RemoveCache(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentNullException(nameof(pattern));
            }
            var redisDb = connectionMultiplexer.GetDatabase();
           
            var endpoint = connectionMultiplexer.GetEndPoints().FirstOrDefault();
            if(endpoint != null)
            {
                var server = connectionMultiplexer.GetServer(endpoint);
                var keys = server.Keys(pattern: pattern);
                foreach (var key in keys)
                {
                    await redisDb.KeyDeleteAsync(key);
                }
            }
            

        }

        public async Task SetCacheResponseAsync(string cache, object response, TimeSpan timeOut)
            {
               if(response == null)
                {
                    return;
                }
                var redisDb = connectionMultiplexer.GetDatabase();
                var serializedResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }) ;
         
               await  redisDb.StringSetAsync(cache, serializedResponse, timeOut);                  
            }
    }
}
