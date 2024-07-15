namespace Demo.Contracts;

[GenerateSerializer]
public class Customer
{
    [Id(0)]
    public Guid id { get; set; } = new Guid();
    [Id(1)]
    public string firstname { get; set; }
    [Id(2)]
    public string lastname { get; set; }
    [Id(3)]
    public string phoneNumber { get; set; }
    [Id(4)]
    public Guid agentId { get; set; }
    [Id(5)]
    public DateTime connectionTime { get; set; }
}
