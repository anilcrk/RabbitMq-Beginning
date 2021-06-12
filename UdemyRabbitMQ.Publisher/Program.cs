using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

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

            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

            Random rnd = new Random();

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {

                // RabbitMq mesajlar byte dizi olarak gider bu sebeple her türlü tipte veri gönderilir.
                
                LogNames log1 = (LogNames)rnd.Next(1, 5);
                LogNames log2 = (LogNames)rnd.Next(1, 5);
                LogNames log3 = (LogNames)rnd.Next(1, 5);

                var routeKey = $"{log1}.{log2}.{log3}";
                string message = $"Log-type : {log1}-{log2}-{log3}"; // created message
                var messageBody = Encoding.UTF8.GetBytes(message); //message Convert to byte array


                /*
                 Direk kuyruğa mesajı gönderdiğimiz için exchange string.empty gönderildi.
                 Default exchange kullanmak için routingKey property sine mutlaka kuyruk ismi verilir.
                 */
                channel.BasicPublish("logs-topic", routeKey, null, messageBody); ;

                Console.WriteLine($"Log gönderildi. Mesaj : {message}");
            });



            Console.ReadLine();
        }
    }
}
