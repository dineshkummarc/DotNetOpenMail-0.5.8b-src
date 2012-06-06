using System;
using System.Net.Sockets;

using DotNetOpenMail;
using DotNetOpenMail.SmtpAuth;

namespace DotNetOpenMailTests.SmtpAuth
{
	/// <summary>
	/// Summary description for MockedSlowServer.
	/// </summary>
	public class MockedSlowProxy : MockedSmtpProxy
	{
		public MockedSlowProxy()
		{
		}

		public override SmtpResponse Open()
		{
			return new SmtpResponse(220, "Welcome to the Slow Server");			
		}

		public override SmtpResponse Auth(String localHostName) 
		{
			//int timeout=10000;
			//System.Threading.Thread.Sleep(timeout);
			throw new SocketException(10060);
			//throw new ApplicationException("Waited "+timeout+" millisseconds");
		}

		
	}
}
