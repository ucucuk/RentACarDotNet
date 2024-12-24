using EmailService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailService.Application.DTOs;

namespace EmailService.Application.Abstarct
{
	public interface IMailService
	{
		void SendMail<T>(MailDTO<T> mailDTO) where T : class;
		void SendMailFromRabbitMQ<T>(MailDTO<T> mailDTO) where T : class;
	}
}
