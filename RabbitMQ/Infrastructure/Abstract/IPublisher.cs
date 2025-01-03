﻿using EmailService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Infrastructure.Abstract
{
	public interface IPublisher
	{
		Task Publish(string queueName, string message);

		Task PublishMail<T>(MailDTO<T> mailDTO) where T : class;
	}
}
