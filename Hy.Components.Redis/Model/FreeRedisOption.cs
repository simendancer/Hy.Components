using System;
using System.Collections.Generic;
using System.Text;

namespace Hy.Components.Redis.Model
{
    /// <summary>
    /// FreeRedis配置选项
    /// </summary>
    public class FreeRedisOption
    {
        /// <summary>
        /// Host
        /// </summary>
        public string RedisHost { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int RedisPort { get; set; } = 6379;

        /// <summary>
        /// 密码
        /// </summary>
        public string RedisPassword { get; set; }

        /// <summary>
        /// 同步超时
        /// </summary>
        public int SyncTimeout { get; set; } = 5000;

        /// <summary>
        /// 连接超时
        /// </summary>
        public int ConnectTimeout { get; set; } = 15000;

        /// <summary>
        /// Key前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 默认访问第几个
        /// </summary>
        public int DefaultIndex { get; set; } = 0;

        /// <summary>
        /// 池大小
        /// </summary>
        public int Poolsize { get; set; } = 5;

        /// <summary>
        /// 异步管道
        /// </summary>
        public bool asyncPipeline { get; set; } = true;

        /// <summary>
        /// 是否启用客户端缓存（6.0及以上支持）
        /// </summary>
        public bool UseClientSideCache { get; set; }

        /// <summary>
        /// 客户端缓存Key筛选条件
        /// </summary>
        public Func<string,bool> ClientSideCacheKeyFilter { get; set; }
    }
}
