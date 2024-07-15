namespace Demo.Contracts;

public interface IAgentConnectionGrain : IGrainWithStringKey
{
    Task CreateAgentConnection(AgentConnection agentConnection);
    Task<AgentConnection> GetAgentConnection();
    Task ClearState();
}
