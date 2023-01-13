using Hy.Components.Redis;
using System.Linq.Expressions;

namespace Hy.Components.Api.Cache
{
    /// <summary>
    /// 实现客户端缓存Demo
    /// </summary>
    public class ClientSideDemoOneCache : ClienSideCacheBase
    {
        /// <summary>
        /// 注入Redis构造函数
        /// </summary>
        /// <param name="redisService"></param>
        public ClientSideDemoOneCache(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// 设置Key过滤规则
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<string,bool>> SetCacheKeyFilter()
        {
            return o => o == GetRedisKey();
        }

        /// <summary>
        /// 获取缓存的Key
        /// </summary>
        /// <returns></returns>
        protected override string GetRedisKey()
        {
            return "DemoOneRedisKey";
        }
    }
}
