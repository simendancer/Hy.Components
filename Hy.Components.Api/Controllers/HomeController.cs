using FreeRedis;
using Hy.Components.Api.Cache;
using Hy.Components.Redis;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace Hy.Components.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public HomeController(ILogger<HomeController> logger,IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 测试Redis
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("redis")]
        public string TestRedis()
        {
            Stopwatch sw = new Stopwatch();

            //构造一个比较大的String
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 10; i++) {
                sb.Append("DemoString");
            }

            //普通缓存 set，这里直接使用FreeRedis默认API即可
            var redisService = _serviceProvider.GetService<FreeRedisService>();
            var key = "NormalCacheKey";
            redisService.Instance.Set(key,$"0000000{sb.ToString()}",60);

            //普通缓存后续多次请求
            sw.Start();
            for (int i = 0; i < 3; i++) {
                redisService.Instance.Get(key);
            }
            sw.Stop();
            Console.WriteLine($"普通缓存：{sw.ElapsedMilliseconds}");


            //客户端缓存1 set
            ClientSideDemoOneCache cacheOne = new ClientSideDemoOneCache(_serviceProvider);
            cacheOne.Set($"111111!{sb.ToString()}",60);

            Thread.Sleep(200);

            //客户端缓存1 多次get
            sw.Restart();
            for (int i = 0; i < 3; i++) {
                cacheOne.Get<string>();
            }
            sw.Stop();
            Console.WriteLine($"客户端缓存1 多次get：{sw.ElapsedMilliseconds}");


            //客户端缓存2 set
            ClientSideDemoTwoCache cacheTwo = new ClientSideDemoTwoCache(_serviceProvider);
            cacheTwo.Set($"222222!{sb.ToString()}",60);

            Thread.Sleep(200);

            //客户端缓存2 多次get
            sw.Restart();
            for (int i = 0; i < 3; i++) {
                cacheTwo.Get<string>(); //第二次从客户端获取
            }
            sw.Stop();
            Console.WriteLine($"客户端缓存2 多次get：{sw.ElapsedMilliseconds}");


            //客户端缓存1 从容器获取实例的方式
            var cacheOneInject = _serviceProvider.GetService<ClientSideDemoOneCache>();
            var value = cacheOne.Get<string>();

            return value;
        }

        #region 可通过启动不同端口的Api，分别调用以下接口对同一个Key进行操作，测试客户端缓存是否生效以及是否及时同步

        /// <summary>
        /// 测试get
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("getvalue")]
        public string TestGetValue()
        {
            ClientSideDemoOneCache cacheOne = new ClientSideDemoOneCache(_serviceProvider);
            var value = cacheOne.Get<string>();
            return value ?? "缓存空了";
        }

        /// <summary>
        /// 测试set
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet, Route("setvalue")]
        public string TestSetValue([FromQuery] string value)
        {
            ClientSideDemoOneCache cacheOne = new ClientSideDemoOneCache(_serviceProvider);
            cacheOne.Set(value);
            return "OK";
        }

        /// <summary>
        /// 测试del
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("delvalue")]
        public string TestDelValue()
        {
            ClientSideDemoOneCache cacheOne = new ClientSideDemoOneCache(_serviceProvider);
            cacheOne.Remove();
            return "OK";
        }

        #endregion
    }
}