using Demo.Signalr;
using Demo.Signalr.BackgroundServices;
using Demo.Signalr.Services;
using Orleans.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR().AddStackExchangeRedis("127.0.0.1:7000", options => {
    options.Configuration.ChannelPrefix = RedisChannel.Literal("MyApp");
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:5500")
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });
});

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IRabbitmqService, RabbitmqService>();
builder.Services.AddSingleton<IGroupManager, GroupManager>();
builder.Services.AddHostedService<MatchingBackgroundService>();

builder.Host.UseOrleansClient(client =>
{
    client
    .UseMongoDBClient("mongodb://localhost:27017")
    .UseMongoDBClustering(options =>
    {
        options.DatabaseName = "ClusterDB";
    })
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "dev";
        options.ServiceId = "SignalRService";
    });
});

var app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();

app.MapHub<AgentHub>("/agenthub");

app.MapHub<CustomerHub>("/customerhub");

app.Run();

