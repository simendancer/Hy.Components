using FreeRedis;
using Hy.Components.Redis.Model;
using Newtonsoft.Json;
using System;

namespace Hy.Components.Redis
{
    public class FreeRedisService
    {
        /// <summary>
        /// RedisClient
        /// </summary>
        private static RedisClient _redisClient;

        /// <summary>
        /// 初始化配置
        /// </summary>
        private FreeRedisOption _redisOption;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FreeRedisService(FreeRedisOption redisOption)
        {
            if (redisOption == null) {
                throw new NullReferenceException("初始化配置为空");
            }
            _redisOption = redisOption;
            InitRedisClient();
        }

        /// <summary>
        /// 懒加载Redis客户端
        /// </summary>
        private readonly static Lazy<RedisClient> redisClientLazy = new Lazy<RedisClient>(() => {
            var r = _redisClient;
            r.Serialize = obj => JsonConvert.SerializeObject(obj);
            r.Deserialize = (json,type) => JsonConvert.DeserializeObject(json,type);
            r.Notice += (s,e) => Console.WriteLine(e.Log);
            return r;
        });

        private static readonly object obj = new object();

        /// <summary>
        /// 初始化Redis
        /// </summary>
        /// <returns></returns>
        bool InitRedisClient()
        {
            if (_redisClient == null) {
                lock (obj) {
                    if (_redisClient == null) {
                        _redisClient = new RedisClient($"{_redisOption.RedisHost}:{_redisOption.RedisPort},password={_redisOption.RedisPassword},defaultDatabase={_redisOption.DefaultIndex},poolsize={_redisOption.Poolsize},ssl=false,writeBuffer=10240,prefix={_redisOption.Prefix},asyncPipeline={_redisOption.asyncPipeline},connectTimeout={_redisOption.ConnectTimeout},abortConnect=false");
                        //设置客户端缓存
                        if (_redisOption.UseClientSideCache) {
                            if (_redisOption.ClientSideCacheKeyFilter == null) {
                                throw new NullReferenceException("如果开启客户端缓存，必须设置客户端缓存Key过滤条件");
                            }
                            _redisClient.UseClientSideCaching(new ClientSideCachingOptions() {
                                Capacity = 0,  //本地缓存的容量，0不限制
                                KeyFilter = _redisOption.ClientSideCacheKeyFilter,  //过滤哪些键能被本地缓存
                                CheckExpired = (key,dt) => DateTime.Now.Subtract(dt) > TimeSpan.FromSeconds(3)  //检查长期未使用的缓存
                            });
                        }
                        return true;
                    }
                }
            }
            return _redisClient != null;
        }

        /// <summary>
        /// 获取Client实例
        /// </summary>
        public RedisClient Instance {
            get {
                if (InitRedisClient()) {
                    return redisClientLazy.Value;
                }
                throw new NullReferenceException("Redis不可用");
            }
        }
    }
}
