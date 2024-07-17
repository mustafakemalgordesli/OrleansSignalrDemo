using Demo.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Demo.Signalr;
public interface ICustomerHub
{
    Task SendMessage(string message);
}

public class CustomerHub(IClusterClient client, IHubContext<AgentHub, IAgentHub> agentHub) : Hub<ICustomerHub>
{
    public async Task ConnectCustomer(Customer customer)
    {
        var conId = Context.ConnectionId;

        var management = client.GetGrain<IManagementGrain>(0);

        var grain = client.GetGrain<IAgentGrain>("xkemalrandom");

        var id = await grain.GetType();

        GrainType type = new GrainType(id);

        List<GrainId> list = await management.GetActiveGrains(type);

        IAgentGrain? minGrain = null;
        Agent? minAgent = null;

        for (int i = 0; i < list.Count; i++)
        {
            var agentGrain = client.GetGrain<IAgentGrain>(list[i]);
            var agent = await agentGrain.GetAgent();
            if (minAgent == null)
            {
                minGrain = agentGrain;
                minAgent = agent;
            }
            if(agent?.nickname != null && agent?.connectionIds.Count < minAgent?.connectionIds.Count)
            {
                minGrain = agentGrain;
                minAgent = agent;
            }
        }

        if (minAgent == null || minGrain == null) return;

        customer.agentNickname = minAgent.nickname;
        customer.connectionTime = DateTime.Now;
        customer.connectionId = conId;
        var customerGrain = client.GetGrain<ICustomerGrain>(conId);
        await customerGrain.CreateCustomer(customer);
        await minGrain.AddCustomer(customer);
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
        await agentHub.Clients.Clients(agent.connectionIds).SendMessage(message, customer.id);
    }
}
