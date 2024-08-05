
using Demo.Contracts;
using Demo.Signalr.Services;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Demo.Signalr.BackgroundServices;

public class MatchingBackgroundService : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    IHubContext<AgentHub, IAgentHub> agentHub;
    IHubContext<CustomerHub, ICustomerHub> customerHub;
    IClusterClient client;
    IGroupManager groupManager {  get; set; }
    public MatchingBackgroundService(IRabbitmqService rabbitmqService, IHubContext<AgentHub, IAgentHub> agentHub, IHubContext<CustomerHub, ICustomerHub> customerHub, IClusterClient client, IGroupManager groupManager)
    {
        this.client = client;
        this.agentHub = agentHub;
        this.customerHub = customerHub;
        _connection = rabbitmqService.GetRabbitMQConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "matching_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        //_channel.QueueDeclare(queue: "unmatching_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        this.groupManager = groupManager;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var conId = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {conId}");

            var customerGrain = client.GetGrain<ICustomerGrain>(conId);

            Customer customer = await customerGrain.GetCustomer();

            var management = client.GetGrain<IManagementGrain>(0);

            var grain = client.GetGrain<IAgentGrain>("defaultuser");

            var id = await grain.GetType();

            GrainType type = new GrainType(id);

            List<GrainId> list = await management.GetActiveGrains(type);

            IAgentGrain? minGrain = null;
            Agent? minAgent = null;

            for (int i = 0; i < list.Count; i++)
            {
                var agentGrain = client.GetGrain<IAgentGrain>(list[i]);
                var agent = await agentGrain.GetAgent();
                if (minAgent == null && agent.nickname != null)
                {
                    minGrain = agentGrain;
                    minAgent = agent;
                }
                if (agent?.nickname != null && agent?.connectionIds.Count < minAgent?.connectionIds.Count)
                {
                    minGrain = agentGrain;
                    minAgent = agent;
                }
            }

            if (minAgent == null || minGrain == null) return;

            string groupName = Guid.NewGuid().ToString();



            await groupManager.AddToGroupAsync(conId, groupName, nameof(CustomerHub));
            
            foreach (var item in minAgent.connectionIds)
            {
                await groupManager.AddToGroupAsync(item, groupName, nameof(AgentHub));
            }

            customer.agentNickname = minAgent.nickname;
            customer.groupName = groupName;
            await customerGrain.UpdateAgent(customer);
            await minGrain.AddCustomer(customer);

            await groupManager.SendMessageToGroupAsync(groupName, groupName + " bağlanıldı");
        };

        _channel.BasicConsume(queue: "matching_queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Close();
        _connection.Close();
        return base.StopAsync(cancellationToken);
    }
}
