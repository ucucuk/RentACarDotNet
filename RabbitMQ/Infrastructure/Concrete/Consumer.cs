using EmailService.Application.Abstarct;
using EmailService.Application.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Application.Abstract;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Infrastructure.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Infrastructure.Concrete
{
	public class Consumer : IConsumer
	{
		private readonly IRabbitMQService _rabbitMQService;
		private readonly IMailService _mailService;
		public Consumer(IRabbitMQService rabbitMQService, IMailService mailService)
		{
			_rabbitMQService = rabbitMQService;
			_mailService = mailService;
		}

		public async Task Consume(string queueName)
		{
			using (var connection = await _rabbitMQService.GetRabbitMQConnection())
			{
				using (var channel = await connection.CreateChannelAsync())
				{
					await channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

					Console.WriteLine(" [*] Waiting for messages.");

					var consumer = new AsyncEventingBasicConsumer(channel);
					consumer.ReceivedAsync += (model, ea) =>
					{
						var body = ea.Body.ToArray();
						var message = Encoding.UTF8.GetString(body);
						Console.WriteLine($" [x] Received {message}");
						return Task.CompletedTask;
					};

					await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

				}
			}
		}

		public async Task ConsumeMailFromRabbitMQ()
		{
			var connection = await _rabbitMQService.GetRabbitMQConnection();
			var channel = await connection.CreateChannelAsync();

			await channel.QueueDeclareAsync("mail", durable: false, exclusive: false, autoDelete: false, arguments: null);

			Console.WriteLine(" [*] Waiting for messages.");

			var consumer = new AsyncEventingBasicConsumer(channel);
			consumer.ReceivedAsync += (model, ea) =>
			{
				var body = ea.Body.ToArray();
				string message = Encoding.UTF8.GetString(body);

				var MailDTO = JsonConvert.DeserializeObject<MailDTO<object>>(message);
				_mailService.SendMailFromRabbitMQ(MailDTO);

				Console.WriteLine($" [x] Received {message}");

				return Task.CompletedTask;
			};
			Console.WriteLine("dinlemeye devam");
			await channel.BasicConsumeAsync("mail", autoAck: true, consumer: consumer);

		}


	}
}
