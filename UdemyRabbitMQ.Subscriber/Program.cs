using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://iilgwgrl:ezp3WGQ8F8xoF4bqnhlsTWMKz8PMwmJV@baboon.rmq.cloudamqp.com/iilgwgrl");

            using var connection = factory.CreateConnection(); // open connection

            var channel = connection.CreateModel(); // created channel
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            /*
             x-match --> 
             all ise --> mutlaka bütün keyler eşit oladuğu koşulda kuyruğa eklenicek.             
             */
            headers.Add("x-match", "all");


            channel.QueueBind(queueName, "header-exchange", string.Empty, headers);

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar dinleniyor...");

            // rabbitmq subscriber mesaj gönderidiğinde event çalışır.
            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);

                Console.WriteLine($"Gelen Mesaj : {message}");

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
