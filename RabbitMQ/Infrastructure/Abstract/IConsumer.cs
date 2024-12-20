using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Infrastructure.Abstract
{
	public interface IConsumer
	{
		Task Consume(string queueName);
	}
}
