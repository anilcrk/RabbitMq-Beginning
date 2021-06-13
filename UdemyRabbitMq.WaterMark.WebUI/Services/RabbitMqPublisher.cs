using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UdemyRabbitMq.WaterMark.WebUI.Services
{
    public class RabbitMqPublisher
    {
        private readonly RabbitMqClientService _rabbitMqClientService;

        public RabbitMqPublisher(RabbitMqClientService rabbitMqClientService)
        {
            _rabbitMqClientService = rabbitMqClientService;
        }

        public void Publish(productImageCreatedEvent productImageCreatedEvent)
        {
            // bağlantı kuruldu.
            var channel = _rabbitMqClientService.Connect();

            var bodyString = System.Text.Json.JsonSerializer.Serialize(productImageCreatedEvent);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: RabbitMqClientService.ExchangeName, routingKey: RabbitMqClientService.RoutingWaterMark, basicProperties: properties, body: bodyByte);
        }
    }
}
