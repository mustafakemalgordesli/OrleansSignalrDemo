namespace Demo.Contracts;

[GenerateSerializer]
public class AgentConnection
{
    [Id(0)]
    public string connectionId { get; set; }
    [Id(1)]
    public string nickname { set; get; }
}
