using Hy.Components.Redis.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hy.Components.Redis
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// ServiceInject
        /// </summary>
        /// <param name="services"></param>
        public static void AddRedisService(this IServiceCollection services, IConfiguration configuration)
        {
            var clientCacheKeyFilter = ClientSideCacheKeyBuilder.Build(services.BuildServiceProvider());
            var option = GetRedisOption(configuration, clientCacheKeyFilter);
            services.AddSingleton(c => new FreeRedisService(option));
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="clientSideCacheKeyFilter"></param>
        /// <returns></returns>
        static FreeRedisOption GetRedisOption(IConfiguration configuration, Func<string, bool> clientSideCacheKeyFilter = null)
        {
            return new FreeRedisOption()
            {
                RedisHost = configuration.GetSection("Redis:RedisHost").Value,
                RedisPassword = configuration.GetSection("Redis:RedisPassword").Value,
                RedisPort = Convert.ToInt32(configuration.GetSection("Redis:RedisPort").Value),
                SyncTimeout = 5000,
                ConnectTimeout = 15000,
                DefaultIndex = 0,
                Poolsize = 5,
                UseClientSideCache = true,
                ClientSideCacheKeyFilter = clientSideCacheKeyFilter
            };
        }
    }
}
