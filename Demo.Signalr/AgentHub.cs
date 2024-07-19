using Demo.Contracts;
using Microsoft.AspNetCore.SignalR;
using Pipelines.Sockets.Unofficial.Arenas;

namespace Demo.Signalr;

public interface IAgentHub
{
    Task SendMessage(string message, string conId);
    Task SendMessage(string message);
}

public class AgentHub(IClusterClient client, IHubContext<CustomerHub, ICustomerHub> customerHub) : Hub<IAgentHub>
{
    public async Task ConnectAgent(Agent agent) 
    {
        var conId = Context.ConnectionId;
        var agentConnectionGrain = client.GetGrain<IAgentConnectionGrain>(conId); 

        var agentGrain = client.GetGrain<IAgentGrain>(agent.nickname);

        if(agentGrain == null)
        {
            await Clients.Client(conId).SendMessage("grain null");
            return;
        }
        
        var existAgent = await agentGrain.GetAgent();

        if (existAgent.nickname != null)
        {
            await agentConnectionGrain.CreateAgentConnection(new() { connectionId = conId, nickname = agent.nickname });
            await agentGrain.AddConnectionId(conId);
            return;
        }

        agent.connectionIds.Add(conId);

        await agentGrain.CreateAgent(agent); 
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var conId = Context.ConnectionId;
        var agentConnectionGrain = client.GetGrain<IAgentConnectionGrain>(conId);
        var agentConnection = await agentConnectionGrain.GetAgentConnection();

        if (agentConnection != null)
        {
            var agentGrain = client.GetGrain<IAgentGrain>(agentConnection.nickname);
            var agent = await agentGrain.GetAgent();
            
            if(agent != null)
            {
                await agentGrain.RemoveConnectionId(conId);
                await agentConnectionGrain.ClearState();
            }
        }
    }

    public async Task ReceiveMessage(string message, string connectionId)
    {
        var customerGrain = client.GetGrain<ICustomerGrain>(connectionId);
        var customer = await customerGrain.GetCustomer();
        await customerHub.Clients.Client(customer.connectionId).SendMessage(message);
    }
}
