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
        /// ����Redis
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("redis")]
        public string TestRedis()
        {
            Stopwatch sw = new Stopwatch();

            //����һ���Ƚϴ��String
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 10; i++) {
                sb.Append("DemoString");
            }

            //��ͨ���� set������ֱ��ʹ��FreeRedisĬ��API����
            var redisService = _serviceProvider.GetService<FreeRedisService>();
            var key = "NormalCacheKey";
            redisService.Instance.Set(key,$"0000000{sb.ToString()}",60);

            //��ͨ��������������
            sw.Start();
            for (int i = 0; i < 3; i++) {
                redisService.Instance.Get(key);
            }
            sw.Stop();
            Console.WriteLine($"��ͨ���棺{sw.ElapsedMilliseconds}");


            //�ͻ��˻���1 set
            ClientSideDemoOneCache cacheOne = new ClientSideDemoOneCache(_serviceProvider);
            cacheOne.Set($"111111!{sb.ToString()}",60);

            Thread.Sleep(200);

            //�ͻ��˻���1 ���get
            sw.Restart();
            for (int i = 0; i < 3; i++) {
                cacheOne.Get<string>();
            }
            sw.Stop();
            Console.WriteLine($"�ͻ��˻���1 ���get��{sw.ElapsedMilliseconds}");


            //�ͻ��˻���2 set
            ClientSideDemoTwoCache cacheTwo = new ClientSideDemoTwoCache(_serviceProvider);
            cacheTwo.Set($"222222!{sb.ToString()}",60);

            Thread.Sleep(200);

            //�ͻ��˻���2 ���get
            sw.Restart();
            for (int i = 0; i < 3; i++) {
                cacheTwo.Get<string>(); //�ڶ��δӿͻ��˻�ȡ
            }
            sw.Stop();
            Console.WriteLine($"�ͻ��˻���2 ���get��{sw.ElapsedMilliseconds}");


            //�ͻ��˻���1 ��������ȡʵ���ķ�ʽ
            var cacheOneInject = _serviceProvider.GetService<ClientSideDemoOneCache>();
            var value = cacheOne.Get<string>();

            return value;
        }

        #region ��ͨ��������ͬ�˿ڵ�Api���ֱ�������½ӿڶ�ͬһ��Key���в��������Կͻ��˻����Ƿ���Ч�Լ��Ƿ�ʱͬ��

        /// <summary>
        /// ����get
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("getvalue")]
        public string TestGetValue()
        {
            ClientSideDemoOneCache cacheOne = new ClientSideDemoOneCache(_serviceProvider);
            var value = cacheOne.Get<string>();
            return value ?? "�������";
        }

        /// <summary>
        /// ����set
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
        /// ����del
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