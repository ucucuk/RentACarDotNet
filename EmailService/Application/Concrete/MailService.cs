using EmailService.Application.Abstarct;
using EmailService.Application.DTOs;
using EmailService.Infrastructure;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;

namespace EmailService.Application.Concrete
{
	public class MailService : IMailService
	{
		private readonly MailSettings _mailSettings;

		public MailService(IOptions<MailSettings> mailSettings)
		{
			_mailSettings = mailSettings.Value;
		}

		public void SendMail<T>(MailDTO<T> mailDTO) where T : class
		{
			var email = new MimeMessage();

			email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
			email.To.Add(MailboxAddress.Parse(mailDTO.Mail));
			email.Subject = mailDTO.Subject;
			email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{ Text = JsonConvert.SerializeObject(mailDTO.body) };
			;
			using (var client = new SmtpClient())
			{

				client.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
				client.Authenticate(_mailSettings.UserName, _mailSettings.Password);
				client.Send(email);
				client.Disconnect(true);
			}
		}

		public void SendMailFromRabbitMQ<T>(MailDTO<T> mailDTO) where T : class
		{
			if(mailDTO.Mail == "")
				throw new FileNotFoundException("You should login with your email.");

			var email = new MimeMessage();

			email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
			email.To.Add(MailboxAddress.Parse(mailDTO.Mail));
			email.Subject = mailDTO.Subject;
			email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{ Text = JsonConvert.SerializeObject(mailDTO.body, Formatting.Indented) };

			using (var client = new SmtpClient())
			{

				client.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
				client.Authenticate(_mailSettings.UserName, _mailSettings.Password);
				client.Send(email);
				client.Disconnect(true);
			}
		}


	}
}
