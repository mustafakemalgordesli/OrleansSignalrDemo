namespace Demo.Contracts;

public interface IAgentConnectionGrain : IGrainWithStringKey
{
    public Task CreateAgentConnection(AgentConnection agentConnection);
    public Task<AgentConnection> GetAgentConnection();
    public Task ClearState();
}
