using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FiapCloudGames.Shared.RabbitMQ;

public interface IRabbitMQPublisher
{
    Task PublishAsync<T>(string exchange, string routingKey, T message) where T : class;
}

public class RabbitMQPublisher : IRabbitMQPublisher
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQPublisher(string host, string username, string password)
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = host,
            UserName = username,
            Password = password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            RequestedHeartbeat = TimeSpan.FromSeconds(60)
        };
    }

    private void EnsureConnection()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T message) where T : class
    {
        try
        {
            EnsureConnection();

            _channel.ExchangeDeclare(
                exchange: exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.ContentType = "application/json";
            properties.DeliveryMode = 2;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body
            );

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao publicar mensagem no RabbitMQ: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
