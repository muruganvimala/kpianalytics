using System;
using System.IO;
using System.Net;
using BusinessIntelligence_API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;

namespace BusinessIntelligence_API.Repository
{
	public class LogService : ILogServiceRepository
	{
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly JTSContext _context;
		public LogService(JTSContext jTSContext ,IWebHostEnvironment hostingEnvironment)
		{
			_context = jTSContext;
			_hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
		}

		public async Task InsertApiLog(BiApiCallLog biApiCallLog, int statusCode)
		{
			biApiCallLog.Timestamp = DateTime.Now;
			biApiCallLog.Environment = _hostingEnvironment.EnvironmentName;
			biApiCallLog.ServerName = Dns.GetHostName();
			biApiCallLog.Username = Environment.UserName;
			biApiCallLog.StatusCode = statusCode;
			await _context.BiApiCallLogs.AddAsync(biApiCallLog);
			await _context.SaveChangesAsync();
		}

		public void InsertLog(string strValue, string strColor = "")
		{
			try
			{
				string StrServerPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Logs");
				if (!Directory.Exists(StrServerPath))
				{
					Directory.CreateDirectory(StrServerPath);
				}

				string LogName = DateTime.Now.Date.ToString("ddMMyyyy") + ".htm";
				string Header = "";

				if (!File.Exists(Path.Combine(StrServerPath, LogName)))
				{
					using (StreamWriter objwriter = new StreamWriter(Path.Combine(StrServerPath, LogName), true))
					{
						Header = "<Table width=100% Border=1 BorderColor=black Cellspacing=0>";
						Header += "<tr><th align=center width=100% colspan=3><Font Face=Verdana Size=2><B>WebAPI - BI</th></tr>";
						Header += "<tr><td align=center width=10%><Font Face=Verdana Size=2><B>Date</td><td align=center width=10%><Font Face=Verdana Size=2><B>Time</td><td align=center><B><Font Face=Verdana Size=2>Message</td></tr>";
						objwriter.Write(Header);
					}
				}

				using (StreamWriter objwriter1 = new StreamWriter(Path.Combine(StrServerPath, LogName), true))
				{
					if (strColor != "")
						objwriter1.Write("<tr><td><Font Face=Verdana Size=2>" + DateTime.Now.Date.ToString("dd.MM.yyyy") + "</TD><TD><Font Face=Verdana Size=2>" + DateTime.Now.ToLongTimeString() + "</TD><TD><Font Face=Verdana Size=2 color=" + strColor + ">Error: " + strValue + "</TD></TR>");
					else
						objwriter1.Write("<tr><td><Font Face=Verdana Size=2>" + DateTime.Now.Date.ToString("dd.MM.yyyy") + "</TD><TD><Font Face=Verdana Size=2>" + DateTime.Now.ToLongTimeString() + "</TD><TD><Font Face=Verdana Size=2>" + strValue + "</TD></TR>");
				}
			}
			catch
			{
				// Handle exceptions appropriately
			}
		}
	}
}
