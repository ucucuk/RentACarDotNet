using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Application.Abstract
{
	public interface IRabbitMQService
	{
		Task<IConnection> GetRabbitMQConnection();
	}
}
