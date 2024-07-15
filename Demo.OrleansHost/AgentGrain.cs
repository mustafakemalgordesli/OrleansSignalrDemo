using Demo.Contracts;
using Orleans.Providers;
using System.Collections.Generic;

namespace Demo.OrleansHost;

[StorageProvider(ProviderName = "MongoStorage")]
public class AgentGrain : Grain, IAgentGrain
{
    private readonly IPersistentState<Agent> _state;
    private readonly GrainType _grainType;
    private IManagementGrain _management;
    public AgentGrain(
        [PersistentState("agent", "MongoStorage")] IPersistentState<Agent> state, IGrainContext grainContext, IManagementGrain management)
    {
        _state = state;
        _grainType = grainContext.GrainId.Type;
        _management = management;
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
        if (_state.State.connectionIds != null && _state.State.connectionIds.Contains(connectionId))
        {
            _state.State.connectionIds.Remove(connectionId);
        }
        await _state.WriteStateAsync();
    }

    public Task<IdSpan> GetType()
    {
        return Task.FromResult(_grainType.Value);   
    }
}