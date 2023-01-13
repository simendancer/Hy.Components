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

        [HttpGet, Route("get")]
        public IEnumerable<string> Get()
        {
            return new List<string>() { "Ok","Fail" };
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
            for (int i = 0; i < 1000; i++) {
                sb.Append("DemoString");
            }

            //普通缓存，直接使用FreeRedis默认API即可
            var redisService = _serviceProvider.GetService<FreeRedisService>();
            var key = "NormalCacheKey";
            redisService.Instance.Set(key,$"0000000{sb.ToString()}",60);

            sw.Start();
            for (int i = 0; i < 100; i++) {
                redisService.Instance.Get(key);
            }
            sw.Stop();
            Console.WriteLine($"普通缓存循环100次：{sw.ElapsedMilliseconds}");



            //客户端缓存1
            ClientSideDemoOneCache cacheOne = new ClientSideDemoOneCache(_serviceProvider);
            cacheOne.Set($"111111!{sb.ToString()}",60);

            sw.Restart();
            var valueFromServer1 = cacheOne.Get<string>(); //首次会从服务端获取
            sw.Stop();
            Console.WriteLine($"one第一次：{sw.ElapsedMilliseconds}");

            Thread.Sleep(1000);

            sw.Restart();
            for (int i = 0; i < 100; i++) {
                cacheOne.Get<string>(); //第二次从客户端获取
            }
            sw.Stop();
            Console.WriteLine($"one循环100次：{sw.ElapsedMilliseconds}");



            //客户端缓存2
            ClientSideDemoTwoCache cacheTwo = new ClientSideDemoTwoCache(_serviceProvider);
            cacheTwo.Set($"222222!{sb.ToString()}",60);

            sw.Restart();
            var valueFromServer2 = cacheTwo.Get<string>(); //首次会从服务端获取
            sw.Stop();
            Console.WriteLine($"two第一次：{sw.ElapsedMilliseconds}");

            Thread.Sleep(1000);

            sw.Restart();
            for (int i = 0; i < 100; i++) {
                cacheTwo.Get<string>(); //第二次从客户端获取
            }
            sw.Stop();
            Console.WriteLine($"two循环100次：{sw.ElapsedMilliseconds}");



            //客户端缓存1 从容器获取的方式
            var cacheOneInject = _serviceProvider.GetService<ClientSideDemoOneCache>();
            var value = cacheOne.Get<string>();

            return value;
        }
    }
}