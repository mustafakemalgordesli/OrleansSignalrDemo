using Demo.Signalr;
using Microsoft.AspNetCore.SignalR;

public interface IGroupManager
{
    Task AddToGroupAsync(string connectionId, string groupName, string hubName);
    Task RemoveFromGroupAsync(string connectionId, string groupName, string hubName);
    Task SendMessageToGroupAsync(string groupName, string message);
}

public class GroupManager : IGroupManager
{
    private readonly IHubContext<AgentHub, IAgentHub> _agentHub;
    private readonly IHubContext<CustomerHub, ICustomerHub> _customerHub;

    public GroupManager(IHubContext<AgentHub, IAgentHub> agentHub, IHubContext<CustomerHub, ICustomerHub> customerHub)
    {
        _customerHub = customerHub;
        _agentHub = agentHub;
    }

    public async Task AddToGroupAsync(string connectionId, string groupName, string hubName)
    {
        if (hubName == nameof(CustomerHub))
        {
            await _customerHub.Groups.AddToGroupAsync(connectionId, groupName);
        }
        else if (hubName == nameof(AgentHub))
        {
            await _agentHub.Groups.AddToGroupAsync(connectionId, groupName);
        }
    }

    public async Task RemoveFromGroupAsync(string connectionId, string groupName, string hubName)
    {
        if (hubName == nameof(CustomerHub))
        {
            await _customerHub.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }
        else if (hubName == nameof(AgentHub))
        {
            await _agentHub.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }
    }

    public async Task SendMessageToGroupAsync(string groupName, string message)
    {
        await _agentHub.Clients.Group(groupName).SendMessage(message);
        await _customerHub.Clients.Group(groupName).SendMessage(message);
    }
}