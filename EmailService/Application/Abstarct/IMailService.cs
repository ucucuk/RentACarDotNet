using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Application.Abstarct
{
	public interface IMailService
	{
		void SendMail(string subject, string body, string mail);

	}
}
