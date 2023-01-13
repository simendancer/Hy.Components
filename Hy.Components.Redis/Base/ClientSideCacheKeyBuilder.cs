using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Hy.Components.Redis
{
    /// <summary>
    /// 构建Redis客户端缓存Key条件
    /// </summary>
    public class ClientSideCacheKeyBuilder
    {
        /// <summary>
        /// 具体业务缓存实现所在项目的程序集
        /// </summary>
        const string DefaultDllName = "Hy.Components.Api";

        /// <summary>
        /// 构建表达式树
        /// </summary>
        /// <param name="serviceProvider">serviceProvider</param>
        /// <param name="dllName">当前类所在的项目dll名</param>
        /// <returns></returns>
        public static Func<string,bool> Build(IServiceProvider serviceProvider,string dllName = DefaultDllName)
        {
            Expression<Func<string,bool>> expression = o => false;
            var baseClass = typeof(ClienSideCacheBase);
            Assembly ass = Assembly.LoadFrom($"{AppDomain.CurrentDomain.BaseDirectory}{dllName}.dll");
            Type[] types = ass.GetTypes();
            foreach (Type item in types) {
                //忽略接口、枚举、带Obsolete标签的类
                if (item.IsInterface || item.IsEnum || item.GetCustomAttribute(typeof(ObsoleteAttribute)) != null)
                    continue;
                //判读基类
                if (item != null && item.BaseType == baseClass) {
                    var instance = (ClienSideCacheBase)Activator.CreateInstance(item,serviceProvider); //这里参数带入IServiceProvider纯粹为了实例化不报错
                    var expr = instance.SetCacheKeyFilter();
                    expression = expression.Or(expr);
                }
            }
            return expression.Compile();
        }
    }
}
