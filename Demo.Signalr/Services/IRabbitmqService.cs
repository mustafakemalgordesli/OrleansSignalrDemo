using RabbitMQ.Client;

namespace Demo.Signalr.Services;

public interface IRabbitmqService
{
    IConnection GetRabbitMQConnection();
}


