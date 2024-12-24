using EmailService.Application.DTOs;
using RabbitMQ.Application.Abstract;
using RabbitMQ.Application.Concrete;
using RabbitMQ.Client;
using RabbitMQ.Infrastructure.Abstract;
using RedisEntegrationBusinessDotNetCore.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQ.Infrastructure.Concrete
{
	public class Publisher : IPublisher
	{
		private readonly IRabbitMQService _rabbitMQService;
		private readonly IConsumer _consumer;
		public Publisher(IRabbitMQService rabbitMQService, IConsumer consumer)
		{
			_rabbitMQService = rabbitMQService;
			_consumer = consumer;
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

		public async Task PublishMail<T>(MailDTO<T> mailDto) where T : class
		{

			//mailDto.Mail = await _redisCacheService.GetValueAsync("usermail");
			//mailDto.Subject = await _redisCacheService.GetValueAsync("username");
			if(mailDto.Mail == "")
				mailDto.Mail = "ulascucuk@gmail.com";
			
			if (mailDto.Subject == "")
				mailDto.Subject = "denememail";

			using (var connection = await _rabbitMQService.GetRabbitMQConnection())
			{
				using (var channel = await connection.CreateChannelAsync())
				{
					await channel.QueueDeclareAsync("mail", false, false, false, null);

					await channel.BasicPublishAsync("", "mail", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mailDto)));

				}
			}
			_consumer.ConsumeMailFromRabbitMQ();
		}
	}
}
