using Demo.Signalr;
using Orleans.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR().AddStackExchangeRedis("127.0.0.1:6379", options => {
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

app.UseCors();

app.UseHttpsRedirection();

app.MapHub<AgentHub>("/agenthub");

app.Run();

