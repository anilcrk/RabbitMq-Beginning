using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
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

            //var randomQueueName = channel.QueueDeclare().QueueName; // random queue name
            var randomQueueName = "log-database-save-queue"; // random queue name

            channel.QueueDeclare(randomQueueName, true, false, false);
            channel.QueueBind(randomQueueName, "logs-fanout", "", null);

            // açıklamar publisher içinde
            //channel.QueueDeclare("hello-queue", true, false, false);

            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);

            /*
             autoAck --> true verilirse rabbitmq subscriber'a bir mesaj gönderildiğinde mesaj başarılı veya başarısız da işlense siler.
             
             */
            channel.BasicConsume(randomQueueName, false, consumer);

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
