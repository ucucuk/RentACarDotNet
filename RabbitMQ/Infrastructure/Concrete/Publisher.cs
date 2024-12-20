using RabbitMQ.Application.Abstract;
using RabbitMQ.Application.Concrete;
using RabbitMQ.Client;
using RabbitMQ.Infrastructure.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Infrastructure.Concrete
{
	public class Publisher : IPublisher
	{
		private readonly IRabbitMQService _rabbitMQService;
		public Publisher(IRabbitMQService rabbitMQService)
		{
			_rabbitMQService = rabbitMQService;
		}

		public async Task Publish(string queueName, string message)
		{
			using (var connection = await _rabbitMQService.GetRabbitMQConnection())
			{
				using (var channel = await connection.CreateChannelAsync())
				{
					await channel.QueueDeclareAsync(queueName, false, false, false, null);

					await channel.BasicPublishAsync("", queueName, Encoding.UTF8.GetBytes(message));

					//Console.WriteLine("{0} queue'su üzerine, \"{1}\" mesajı yazıldı.", queueName, message);
					//Console.WriteLine(" PUBLISH BİTTİ");
				}
			}
		}
	}
}
