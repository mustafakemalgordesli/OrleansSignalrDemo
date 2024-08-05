using Demo.Contracts;
using Demo.Signalr.Services;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using System.Text;

namespace Demo.Signalr;
public interface ICustomerHub
{
    Task SendMessage(string message);
}

public class CustomerHub : Hub<ICustomerHub>
{
    private IRabbitmqService rabbitmqService;
    IClusterClient client;
    IHubContext<AgentHub, IAgentHub> agentHub;
    public CustomerHub(IClusterClient client, IHubContext<AgentHub, IAgentHub> agentHub, IRabbitmqService rabbitmqService)
    {
        this.agentHub = agentHub;
        this.client = client;
        this.rabbitmqService = rabbitmqService;
        //_channel.QueueDeclare(queue: "matching_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public async Task ConnectCustomer(Customer customer)
    {
        var conId = Context.ConnectionId;

        customer.connectionTime = DateTime.Now;
        customer.connectionId = conId;
        var customerGrain = client.GetGrain<ICustomerGrain>(conId);
        await customerGrain.CreateCustomer(customer);

        var queueName = "matching_queue";
        using (var connection = rabbitmqService.GetRabbitMQConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, false, false, false, null);

                channel.BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(conId));

                Console.WriteLine("{0} queue'su üzerine, \"{1}\" mesajı yazıldı.", queueName, conId);
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var conId = Context.ConnectionId;
        var customerGrain = client.GetGrain<ICustomerGrain>(conId);
        var customer = await customerGrain.GetCustomer();
        await client.GetGrain<IAgentGrain>(customer.agentNickname).RemoveCustomer(conId);
        await customerGrain.ClearState();
        return;
    }

    public async Task ReceiveMessage(string message)
    {
        var conId = Context.ConnectionId;
        var customerGrain = client.GetGrain<ICustomerGrain>(conId);
        var customer = await customerGrain.GetCustomer();
        var agent = await client.GetGrain<IAgentGrain>(customer.agentNickname).GetAgent();
        await agentHub.Clients.Clients(agent.connectionIds).SendMessage(message, customer.connectionId);
    }
}
