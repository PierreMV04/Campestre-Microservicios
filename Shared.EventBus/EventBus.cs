using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;

namespace Shared.EventBus;

/// <summary>
/// Interfaz para el bus de eventos
/// </summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;
    void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;
}


public class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ConcurrentDictionary<string, List<Func<IEvent, Task>>> _handlers = new();

    private const string ExchangeName = "hotel.events";

    public RabbitMqEventBus(string hostName = "localhost")
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Fanout,
            durable: true
        );
    }

    // =====================
    //      PUBLISH
    // =====================
    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var eventName = typeof(TEvent).Name;
        Console.WriteLine(
            $"[EVENT PUBLISHED] {eventName} | ID: {@event.EventId} | {DateTime.UtcNow}"
        );
        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(@event)
        );

        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: string.Empty,
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }

    // =====================
    //      SUBSCRIBE
    // =====================
    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        var eventName = typeof(TEvent).Name;

        var queueName = $"{eventName}.queue";

        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        _channel.QueueBind(
            queue: queueName,
            exchange: ExchangeName,
            routingKey: string.Empty
        );

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var @event = JsonSerializer.Deserialize<TEvent>(message);

                await handler(@event!);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch
            {
                _channel.BasicNack(ea.DeliveryTag, false, true);
                throw;
            }
        };


        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer
        );
    }

    public void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        var eventName = typeof(TEvent).Name;
        _handlers.TryRemove(eventName, out _);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}


/// <summary>
/// Logger simple para cuando no hay DI
/// </summary>
public interface ILogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogError(Exception ex, string message, params object[] args);
}

public class ConsoleLogger<T> : ILogger<T>
{
    public void LogInformation(string message, params object[] args)
    {
        Console.WriteLine($"[INFO] [{typeof(T).Name}] {string.Format(message, args)}");
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        Console.WriteLine($"[ERROR] [{typeof(T).Name}] {string.Format(message, args)}: {ex.Message}");
    }
}
