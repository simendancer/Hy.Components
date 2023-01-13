using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;

namespace Hy.Components.Redis
{
    /// <summary>
    /// Redis6.0客户端缓存实现基类
    /// </summary>
    public abstract class ClienSideCacheBase
    {
        /// <summary>
        /// RedisService
        /// </summary>
        private static FreeRedisService _redisService;

        /// <summary>
        /// 获取RedisKey
        /// </summary>
        /// <returns></returns>
        protected abstract string GetRedisKey();

        /// <summary>
        /// 设置客户端缓存Key过滤条件
        /// </summary>
        /// <returns></returns>
        public abstract Expression<Func<string,bool>> SetCacheKeyFilter();

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private ClienSideCacheBase() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="redisService"></param>
        public ClienSideCacheBase(IServiceProvider redisService)
        {
            Console.WriteLine("带入参构造函数");
            _redisService = redisService.GetService<FreeRedisService>();
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            return _redisService.Instance.Get<T>(GetRedisKey());
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Set<T>(T data)
        {
            _redisService.Instance.Set(GetRedisKey(),data);
            return true;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool Set<T>(T data,int seconds)
        {
            _redisService.Instance.Set(GetRedisKey(),data,TimeSpan.FromSeconds(seconds));
            return true;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="expired"></param>
        /// <returns></returns>
        public bool Set<T>(T data,TimeSpan expired)
        {
            _redisService.Instance.Set(GetRedisKey(),data,expired);
            return true;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="expiredAt"></param>
        /// <returns></returns>
        public bool Set<T>(T data,DateTime expiredAt)
        {
            _redisService.Instance.Set(GetRedisKey(),data,TimeSpan.FromSeconds(expiredAt.Subtract(DateTime.Now).TotalSeconds));
            return true;
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <returns></returns>
        public bool SetExpire(int seconds)
        {
            return _redisService.Instance.Expire(GetRedisKey(),TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <returns></returns>
        public bool SetExpire(TimeSpan expired)
        {
            return _redisService.Instance.Expire(GetRedisKey(),expired);
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <returns></returns>
        public bool SetExpireAt(DateTime expiredTime)
        {
            return _redisService.Instance.ExpireAt(GetRedisKey(),expiredTime);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <returns></returns>
        public long Remove()
        {
            return _redisService.Instance.Del(GetRedisKey());
        }

        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            return _redisService.Instance.Exists(GetRedisKey());
        }
    }
}
