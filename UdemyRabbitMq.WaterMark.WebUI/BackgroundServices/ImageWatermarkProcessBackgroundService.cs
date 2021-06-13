using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UdemyRabbitMq.WaterMark.WebUI.Services;

namespace UdemyRabbitMq.WaterMark.WebUI.BackgroundServices
{
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMqClientService _rabbitMqClientService;
        private readonly ILogger<ImageWatermarkProcessBackgroundService> _logger;
        private RabbitMQ.Client.IModel _channel;
        public ImageWatermarkProcessBackgroundService(RabbitMqClientService rabbitMqClientService, ILogger<ImageWatermarkProcessBackgroundService> logger)
        {
            _rabbitMqClientService = rabbitMqClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMqClientService.Connect();

            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(queue: RabbitMqClientService.QueueName, autoAck: false, consumer: consumer);

            consumer.Received += Consumer_Received;


            return Task.CompletedTask;


        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {

            Task.Delay(5000).Wait();

            try
            {
                var imageCreatedEvent = System.Text.Json.JsonSerializer.Deserialize<productImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", imageCreatedEvent.ImageName);

                var siteName = "www.hac.com.tr";

                using var img = Image.FromFile(path);

                using var graphic = Graphics.FromImage(img);

                var font = new Font(FontFamily.GenericMonospace, 32, FontStyle.Bold, GraphicsUnit.Pixel);

                var textSize = graphic.MeasureString(siteName, font);

                var color = Color.FromArgb(128, 255, 255, 255);

                var brush = new SolidBrush(color);

                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height + 30));


                //draw
                graphic.DrawString(siteName, font, brush, position);

                // save
                img.Save("wwwroot/Images/watermarks/" + imageCreatedEvent.ImageName);

                img.Dispose();
                graphic.Dispose();

                // Başarılı bir şekilde işlerse bilgi verilir.
                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception : " + ex.Message);
                throw;
            }

            return Task.CompletedTask;

           


        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
