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
        public static void AddRedisService(this IServiceCollection services,IConfiguration configuration)
        {
            var clientCacheKeyFilter = ClientSideCacheKeyBuilder.Build(services.BuildServiceProvider()); //构造过滤条件
            var option = GetRedisOption(configuration,clientCacheKeyFilter); //组装Redis初始配置
            services.AddSingleton(c => new FreeRedisService(option)); //FreeRedis注入为单例
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="clientSideCacheKeyFilter"></param>
        /// <returns></returns>
        static FreeRedisOption GetRedisOption(IConfiguration configuration,Func<string,bool> clientSideCacheKeyFilter = null)
        {
            return new FreeRedisOption() {
                RedisHost = configuration.GetSection("Redis:RedisHost").Value,
                RedisPassword = configuration.GetSection("Redis:RedisPassword").Value,
                RedisPort = Convert.ToInt32(configuration.GetSection("Redis:RedisPort").Value),
                SyncTimeout = 5000,
                ConnectTimeout = 15000,
                DefaultIndex = 0,
                Poolsize = 5,
                UseClientSideCache = clientSideCacheKeyFilter != null,
                ClientSideCacheKeyFilter = clientSideCacheKeyFilter
            };
        }
    }
}
