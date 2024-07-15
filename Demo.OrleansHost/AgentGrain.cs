using Demo.Contracts;
using Orleans.Providers;

namespace Demo.OrleansHost;

[StorageProvider(ProviderName = "MongoStorage")]
public class AgentGrain : Grain, IAgentGrain
{
    private readonly IPersistentState<Agent> _state;

    public AgentGrain(
        [PersistentState("agent", "MongoStorage")] IPersistentState<Agent> state)
    {
        _state = state;
    }

    public async Task AddConnectionId(string connectionId)
    {
        _state.State.connectionIds.Add(connectionId);
        await _state.WriteStateAsync();
    }

    public async Task CreateAgent(Agent agent)
    {
        _state.State = agent;
        await _state.WriteStateAsync();
    }

    public async Task<Agent> GetAgent()
    {
        return _state.State;
    }

    public async Task RemoveConnectionId(string connectionId)
    {
        _state.State.connectionIds.Remove(connectionId);
        await _state.WriteStateAsync();
    }
}