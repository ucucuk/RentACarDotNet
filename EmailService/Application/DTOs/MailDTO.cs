using System.Text.Json;

namespace EmailService.Application.DTOs
{
	public class MailDTO<T> 
	{
		public string Mail { get; set; }
		public string? Subject { get; set; }
		public T? body { get; set; }

		public MailDTO()
		{
		}

		public MailDTO(string mail , string? subject, T? body)
		{
			Mail = mail;
			Subject = subject;
			this.body = body;
		}
	}
}
