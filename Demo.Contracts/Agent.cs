namespace Demo.Contracts;

[Serializable]
public class Agent
{
    public Guid id { get; set; } = new Guid();
    public string nickname { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public List<Customer> customers { get; set; }
    public List<string> connectionIds { get; set; }
}
