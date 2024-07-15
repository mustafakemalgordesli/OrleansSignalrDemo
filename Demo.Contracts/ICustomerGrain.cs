namespace Demo.Contracts;

public interface ICustomerGrain : IGrainWithStringKey
{
    Task CreateCustomer(Customer customer);
    Task<Customer> GetCustomer();
    Task ClearState();
}
