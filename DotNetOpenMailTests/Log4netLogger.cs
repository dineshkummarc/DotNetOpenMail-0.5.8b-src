using System;

using DotNetOpenMail.Logging;

using log4net;

namespace DotNetOpenMailTests
{
	/// <summary>
	/// Summary description for Log4netLogger.
	/// </summary>
	public class Log4netLogger
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Log4netLogger));

		public Log4netLogger()
		{
		}

		public void Log(LogMessage message) 
		{
			log.Debug(message.Sender.GetType().Name+" ["+DateTime.Now.ToShortTimeString()+"]: "+message.Message);
		}

	}
}
