using Hy.Components.Api.Cache;
using Hy.Components.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

//注入Redis服务
builder.Services.AddRedisService(builder.Configuration);

//可选：注入客户端缓存具体实现类。 如果实现有很多，这里会有一大堆注入代码。在代码中直接实例化类并传入IServiceProvider也一样的
builder.Services.AddSingleton<ClientSideDemoOneCache>();
builder.Services.AddSingleton<ClientSideDemoTwoCache>();

//构建WebApplication
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/health");

app.Run();
