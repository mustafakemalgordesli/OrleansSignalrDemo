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
    Task<MatchingStatus> MatchingStatus();
    Task ClearState();
}

[GenerateSerializer]
public class MatchingStatus
{
    [Id(0)]
    public bool IsOnline {  get; set; }
    [Id(1)]
    public int CustomerCount { get; set; }
}
