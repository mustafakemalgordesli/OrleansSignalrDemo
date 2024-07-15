using Demo.Contracts;
using Microsoft.AspNetCore.SignalR;
using Pipelines.Sockets.Unofficial.Arenas;

namespace Demo.Signalr;

public class AgentHub(IClusterClient client) : Hub
{
    public async Task ConnectAgent(Agent agent) 
    {
        var conId = Context.ConnectionId;
        var agentConnectionGrain = client.GetGrain<IAgentConnectionGrain>(conId); 

        var agentGrain = client.GetGrain<IAgentGrain>(agent.nickname);

        if(agentGrain == null)
        {
            await Clients.Client(conId).SendAsync("Newuser", "hatalı");
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

    public async Task GetMinCustomer()
    {
        var conId = Context.ConnectionId;

        var management = client.GetGrain<IManagementGrain>(0);

        var id = await client.GetGrain<IAgentGrain>("kemal").GetType();

        GrainType type = new GrainType(id);

        List<GrainId> list = await management.GetActiveGrains(type);

        await Clients.Client(conId).SendAsync("Newuser", list.Count);

        for (int i = 0; i < list.Count; i++)
        {
            var agentGrain = client.GetGrain<IAgentGrain>(list[i]);
            var agent = await agentGrain.GetAgent();
            await Clients.Client(Context.ConnectionId).SendAsync("Newuser", agent.nickname + " " + agent.connectionIds.Count);
        }
    }
}
