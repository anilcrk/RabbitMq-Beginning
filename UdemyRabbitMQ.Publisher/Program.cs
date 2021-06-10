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

            /*
             * durable --> false ise rabbitmq de oluşan kuyruklar memoryde tutulur.
            true ise kuyruklar fiziksel olarak kayıt edilir restrat atılsa bile tutulur.
            */

            /*
             excslusive true ise bu kuyruğa sadece bu kadal üzerinden bağlanılır.
             farklı bir kanal üzerinden bağlanılacak için false yapıyoruz.
             */

            /*
             autodelete : Bütün subcriber lar giderse kutruk silinir.
             otomatik silinmemesi için false yapıyoruz.
             */

            //channel.QueueDeclare("hello-queue", true, false, false);

            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var routeKey = $"route-{x}";
                var queueName = $"direct-queue-{x}";

                // created queue
                channel.QueueDeclare(queueName, true, false);

                //binding
                channel.QueueBind(queueName, "logs-direct", routeKey);
            });

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log = (LogNames)new Random().Next(1, 5);

                // RabbitMq mesajlar byte dizi olarak gider bu sebeple her türlü tipte veri gönderilir.
                string message = $"Log-type : {log}"; // created message

                var messageBody = Encoding.UTF8.GetBytes(message); //message Convert to byte array


                // created root
                var routeKey = $"route-{log}";

                /*
                 Direk kuyruğa mesajı gönderdiğimiz için exchange string.empty gönderildi.
                 Default exchange kullanmak için routingKey property sine mutlaka kuyruk ismi verilir.
                 */
                channel.BasicPublish("logs-direct", routeKey, null, messageBody); ;

                Console.WriteLine($"Log gönderildi. Mesaj : {message}");
            });



            Console.ReadLine();
        }
    }
}
