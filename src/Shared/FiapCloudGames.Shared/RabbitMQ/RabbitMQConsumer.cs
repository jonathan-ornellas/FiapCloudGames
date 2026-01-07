using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FiapCloudGames.Shared.RabbitMQ;

public interface IRabbitMQConsumer
{
    Task ConsumeAsync<T>(
        string queue,
        string exchange,
        string routingKey,
        Func<T, Task> handler,
        CancellationToken cancellationToken = default
    ) where T : class;
}

public class RabbitMQConsumer : IRabbitMQConsumer
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQConsumer(string host, string username, string password)
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

    public async Task ConsumeAsync<T>(
        string queue,
        string exchange,
        string routingKey,
        Func<T, Task> handler,
        CancellationToken cancellationToken = default
    ) where T : class
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

            _channel.QueueDeclare(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            _channel.QueueBind(
                queue: queue,
                exchange: exchange,
                routingKey: routingKey
            );

            _channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(json);

                    if (message != null)
                    {
                        await handler(message);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: queue,
                autoAck: false,
                consumerTag: $"{queue}-consumer",
                consumer: consumer
            );

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao consumir mensagens do RabbitMQ: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
