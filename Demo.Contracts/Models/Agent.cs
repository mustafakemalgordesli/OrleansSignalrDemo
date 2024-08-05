namespace Demo.Contracts;

[GenerateSerializer]
public class Agent

{
    [Id(0)]
    public Guid id { get; set; } = new Guid();
    [Id(1)]
    public string nickname { get; set; }
    [Id(2)]
    public string firstname { get; set; }
    [Id(3)]
    public string lastname { get; set; }
    [Id(4)]
    public IsStatus IsStatus { get; set; } = IsStatus.Online;
    [Id(5)]
    public List<Customer> customers { get; set; } = new();
    [Id(6)]
    public List<string> connectionIds { get; set; } = new();
}

public enum IsStatus
{
    Online= 1,
    Break=2,
    Offline=3,
}