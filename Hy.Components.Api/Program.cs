using Hy.Components.Api.Cache;
using Hy.Components.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

//ע��Redis����
builder.Services.AddRedisService(builder.Configuration);

//ע��Redisҵ��ʵ��
builder.Services.AddSingleton<ClientSideDemoOneCache>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/health");

app.Run();
