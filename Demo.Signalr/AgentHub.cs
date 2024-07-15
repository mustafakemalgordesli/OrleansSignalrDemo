using Demo.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Demo.Signalr;

public interface IAgentHub
{
    Task ConnectAgent(Agent agent);
}

public class AgentHub(IClusterClient client) : Hub<IAgentHub>
{
    public async Task ConnectAgent(Agent agent) 
    {
        var agentConnectionGrain = client.GetGrain<IAgentConnectionGrain>(Context.ConnectionId); 

        var agentGrain = client.GetGrain<IAgentGrain>(agent.nickname);
        
        var existAgent = await agentGrain.GetAgent();

        if (existAgent != null)
        {
            await agentConnectionGrain.CreateAgentConnection(new() { connectionId = Context.ConnectionId, nickname = agent.nickname });
            await agentGrain.AddConnectionId(Context.ConnectionId);
            return;
        }

        await agentGrain.CreateAgent(agent); 
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var agentConnectionGrain = client.GetGrain<IAgentConnectionGrain>(Context.ConnectionId);
        var agentConnection = await agentConnectionGrain.GetAgentConnection();

        if (agentConnection != null)
        {
            var agentGrain = client.GetGrain<IAgentGrain>(agentConnection.nickname);
            var agent = await agentGrain.GetAgent();
            
            if(agent != null)
            {
                await agentGrain.RemoveConnectionId(Context.ConnectionId);
                await agentConnectionGrain.ClearState();
            }
        }
    }
}
