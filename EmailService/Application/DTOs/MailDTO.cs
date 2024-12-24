using System.Text.Json;

namespace EmailService.Application.DTOs
{
	public class MailDTO<T> 
	{
		public string Mail { get; set; }
		public string? Subject { get; set; }

		public string? Text { get; set; }
		public T? Body { get; set; }

		public MailDTO()
		{
		}

		public MailDTO(string mail, string? subject, string? text, T? body)
		{
			Mail = mail;
			Subject = subject;
			Body = body;
			Text = text;
		}
	}
}
