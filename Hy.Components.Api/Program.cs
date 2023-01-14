using Hy.Components.Api.Cache;
using Hy.Components.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

//ע��Redis����
builder.Services.AddRedisService(builder.Configuration);

//��ѡ��ע��ͻ��˻������ʵ���ࡣ ���ʵ���кܶ࣬�������һ���ע����롣�ڴ�����ֱ��ʵ�����ಢ����IServiceProviderҲһ����
builder.Services.AddSingleton<ClientSideDemoOneCache>();
builder.Services.AddSingleton<ClientSideDemoTwoCache>();

//����WebApplication
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/health");

app.Run();
