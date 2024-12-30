using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace eCommerceApp.Infrastructure.Services.RedisCache
{
    public class RedisCacheOptions : IOptions<RedisCacheOptions>
    {
        public string InstanceName { get; set; }
        public string Configuration { get; set; }
        public string ReadServer { get; set; }
        public string WriteServer { get; set; }
        public ConfigurationOptions ConfigurationOptions { get; set; }
        public string ConnectionString { get; internal set; }

        RedisCacheOptions IOptions<RedisCacheOptions>.Value
        {
            get { return this; }
        }
    }
}
