﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Infrastructure
{
	public class MailSettings
	{
		public string? DisplayName { get; set; }
		public string? From { get; set; }
		public string? Host { get; set; }
		public string? UserName { get; set; }
		public string? Password { get; set; }
		public int Port { get; set; }
	}
}
