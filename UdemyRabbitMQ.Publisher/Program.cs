using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{
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

            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                // RabbitMq mesajlar byte dizi olarak gider bu sebeple her türlü tipte veri gönderilir.
                string message = $"Log {x}"; // created message

                var messageBody = Encoding.UTF8.GetBytes(message); //message Convert to byte array

                /*
                 Direk kuyruğa mesajı gönderdiğimiz için exchange string.empty gönderildi.
                 Default exchange kullanmak için routingKey property sine mutlaka kuyruk ismi verilir.
                 */
                channel.BasicPublish("logs-fanout", "", null, messageBody); ;

                Console.Write($"Mesaj gönderildi. Mesaj : {message}");
            });

           

            Console.ReadLine();
        }
    }
}
