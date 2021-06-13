using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UdemyRabbitMq.WaterMark.WebUI.Services
{
    public class RabbitMqClientService:IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWaterMark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";

        private readonly ILogger<RabbitMqClientService> _logger;

        public RabbitMqClientService(ConnectionFactory connectionFactory, ILogger<RabbitMqClientService> logger)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);

            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, null);

            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingWaterMark);

            _logger.LogInformation("RabbitMq il bağlantı kuruldu.");

            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMq ile bağlantı koptu !");
        }
    }
}
