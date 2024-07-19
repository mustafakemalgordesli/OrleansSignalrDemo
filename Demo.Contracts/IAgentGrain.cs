namespace Demo.Contracts;


public interface IAgentGrain : IGrainWithStringKey
{
    Task CreateAgent(Agent agent);
    Task<Agent> GetAgent();
    Task AddConnectionId(string connectionId);
    Task RemoveConnectionId(string connectionId);
    Task<IdSpan> GetType();
    Task AddCustomer(Customer customer);
    Task RemoveCustomer(string conId);
    Task ClearState();
}
