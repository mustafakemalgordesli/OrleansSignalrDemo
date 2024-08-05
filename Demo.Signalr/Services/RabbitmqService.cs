using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Demo.Signalr.Services;

public class RabbitmqService : IRabbitmqService
{
    private readonly RabbitMqOptions _options;
    public RabbitmqService(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public IConnection GetRabbitMQConnection()
    {
        ConnectionFactory connectionFactory = new ConnectionFactory()
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            Port = _options.Port
        };

        return connectionFactory.CreateConnection();
    }
}

public class RabbitMqOptions
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; }
    public int Port { get; set; }
}