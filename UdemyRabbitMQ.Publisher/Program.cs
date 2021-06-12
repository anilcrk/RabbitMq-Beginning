using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using UdemyRabbitMQ.Shared;

namespace UdemyRabbitMQ.Publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://iilgwgrl:ezp3WGQ8F8xoF4bqnhlsTWMKz8PMwmJV@baboon.rmq.cloudamqp.com/iilgwgrl");

            using var connection = factory.CreateConnection(); // open connection

            var channel = connection.CreateModel(); // created channel

            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            Dictionary<string, object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            //created property
            var propoerties = channel.CreateBasicProperties();
            propoerties.Headers = headers;
            propoerties.Persistent = true; // mesajlar kalıcı hale gelir.


                var product = new Product
                {
                    Id = 1,
                    Stock = 2,
                    Name = "Test Product",
                    Price = 1500
                };

            var productJsonString = JsonSerializer.Serialize(product);

            channel.BasicPublish("header-exchange", string.Empty, propoerties, Encoding.UTF8.GetBytes(productJsonString));



            Console.WriteLine("Mesajınınız gönderildi !");

            Console.ReadLine();
        }
    }
}
