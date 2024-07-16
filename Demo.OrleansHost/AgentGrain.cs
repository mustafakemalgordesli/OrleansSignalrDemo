using Demo.Contracts;
using MongoDB.Driver.Core.Connections;
using Orleans.Providers;
using System.Collections.Generic;

namespace Demo.OrleansHost;

[StorageProvider(ProviderName = "MongoStorage")]
public class AgentGrain : Grain, IAgentGrain
{
    private readonly IPersistentState<Agent> _state;
    private readonly GrainType _grainType;
    public AgentGrain(
        [PersistentState("agent", "MongoStorage")] IPersistentState<Agent> state, IGrainContext grainContext)
    {
        _state = state;
        _grainType = grainContext.GrainId.Type;
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

    public async Task AddCustomer(Customer customer)
    {
        _state.State.customers.Add(customer);
        await _state.WriteStateAsync();
    }

    public async Task RemoveCustomer(string conId)
    {
        if (_state.State.customers != null)
        {
            var customer = _state.State.customers.FirstOrDefault(x => x.connectionId == conId);
            _state.State.customers.Remove(customer);
        }
        await _state.WriteStateAsync();
    }
}