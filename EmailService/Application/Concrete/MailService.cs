using EmailService.Application.Abstarct;
using EmailService.Infrastructure;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Application.Concrete
{
	public class MailService : IMailService
	{
		private readonly MailSettings _mailSettings;

		public MailService(IOptions<MailSettings> mailSettings)
		{
			_mailSettings = mailSettings.Value;
		}

		public void SendMail(string subject, string body, string mail)
		{
			var email = new MimeMessage();

			email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
			email.To.Add(MailboxAddress.Parse(mail));
			email.Subject = subject;
			email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{ Text = body };

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
