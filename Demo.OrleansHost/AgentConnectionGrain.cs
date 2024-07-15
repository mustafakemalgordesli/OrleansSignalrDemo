using Demo.Contracts;

namespace Demo.OrleansHost;

public class AgentConnectionGrain : Grain, IAgentConnectionGrain
{
    private readonly IPersistentState<AgentConnection> _state;

    public AgentConnectionGrain(
        [PersistentState("agentConnection", "MongoStorage")] IPersistentState<AgentConnection> state)
    {
        _state = state;
    }

    public async Task CreateAgentConnection(AgentConnection agentConnection)
    {
        _state.State = agentConnection;
        await _state.WriteStateAsync();
    }

    public async Task ClearState()
    {
        await _state.ClearStateAsync();
    }

    public Task<AgentConnection> GetAgentConnection()
    {
        return Task.FromResult(_state.State);
    }
}
