using Demo.Contracts;

namespace Demo.OrleansHost;

public class CustomerGrain : Grain, ICustomerGrain
{
    private readonly IPersistentState<Customer> _state;

    public CustomerGrain(
        [PersistentState("customer", "MongoStorage")] IPersistentState<Customer> state)
    {
        _state = state;
    }

    public async Task ClearState()
    {
        await _state.ClearStateAsync();
    }

    public async Task CreateCustomer(Customer customer)
    {
        _state.State = customer;
        await _state.WriteStateAsync();
    }

    public Task<Customer> GetCustomer()
    {
        return Task.FromResult(_state.State);
    }
}
