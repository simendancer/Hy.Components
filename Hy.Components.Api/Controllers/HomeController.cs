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
        /// ����Redis
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("redis")]
        public string TestRedis()
        {
            Stopwatch sw = new Stopwatch();

            //����һ���Ƚϴ��String
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 1000; i++) {
                sb.Append("DemoString");
            }

            //��ͨ���棬ֱ��ʹ��FreeRedisĬ��API����
            var redisService = _serviceProvider.GetService<FreeRedisService>();
            var key = "NormalCacheKey";
            redisService.Instance.Set(key,$"0000000{sb.ToString()}",60);

            sw.Start();
            for (int i = 0; i < 100; i++) {
                redisService.Instance.Get(key);
            }
            sw.Stop();
            Console.WriteLine($"��ͨ����ѭ��100�Σ�{sw.ElapsedMilliseconds}");



            //�ͻ��˻���1
            ClientSideDemoOneCache cacheOne = new ClientSideDemoOneCache(_serviceProvider);
            cacheOne.Set($"111111!{sb.ToString()}",60);

            sw.Restart();
            var valueFromServer1 = cacheOne.Get<string>(); //�״λ�ӷ���˻�ȡ
            sw.Stop();
            Console.WriteLine($"one��һ�Σ�{sw.ElapsedMilliseconds}");

            Thread.Sleep(1000);

            sw.Restart();
            for (int i = 0; i < 100; i++) {
                cacheOne.Get<string>(); //�ڶ��δӿͻ��˻�ȡ
            }
            sw.Stop();
            Console.WriteLine($"oneѭ��100�Σ�{sw.ElapsedMilliseconds}");



            //�ͻ��˻���2
            ClientSideDemoTwoCache cacheTwo = new ClientSideDemoTwoCache(_serviceProvider);
            cacheTwo.Set($"222222!{sb.ToString()}",60);

            sw.Restart();
            var valueFromServer2 = cacheTwo.Get<string>(); //�״λ�ӷ���˻�ȡ
            sw.Stop();
            Console.WriteLine($"two��һ�Σ�{sw.ElapsedMilliseconds}");

            Thread.Sleep(1000);

            sw.Restart();
            for (int i = 0; i < 100; i++) {
                cacheTwo.Get<string>(); //�ڶ��δӿͻ��˻�ȡ
            }
            sw.Stop();
            Console.WriteLine($"twoѭ��100�Σ�{sw.ElapsedMilliseconds}");



            //�ͻ��˻���1 ��������ȡ�ķ�ʽ
            var cacheOneInject = _serviceProvider.GetService<ClientSideDemoOneCache>();
            var value = cacheOne.Get<string>();

            return value;
        }
    }
}