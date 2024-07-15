using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using System.Net;

var siloPort = int.Parse(args[0]);
var gatewayPort = int.Parse(args[1]);

var host = Host.CreateDefaultBuilder(args)
    .UseOrleans(silo =>
    {
        silo.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "SignalRService";
        })
        .UseMongoDBClient("mongodb://docker:mongopw@localhost:49153")
        .AddMongoDBGrainStorage(name: "MongoStorage", options =>
        {
            options.DatabaseName = "OrleansDemoDB";
        })
        .ConfigureEndpoints(IPAddress.Loopback, siloPort, gatewayPort)
        .UseDashboard()
        .UseMongoDBClustering(options =>
        {
            options.DatabaseName = "ClusterDB";
        });
    })
    .Build();

await host.RunAsync();
