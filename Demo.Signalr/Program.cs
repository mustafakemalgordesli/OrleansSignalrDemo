using Orleans.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR().AddStackExchangeRedis("127.0.0.1:6379", options => {
    options.Configuration.ChannelPrefix = RedisChannel.Literal("MyApp");
});

builder.Host.UseOrleansClient(client =>
{
    client
    .UseMongoDBClient("mongodb://docker:mongopw@localhost:49153")
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

app.UseHttpsRedirection();

app.Run();

