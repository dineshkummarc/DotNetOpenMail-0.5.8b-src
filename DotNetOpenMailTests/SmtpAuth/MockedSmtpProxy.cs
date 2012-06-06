using System;

using DotNetOpenMail;
using DotNetOpenMail.SmtpAuth;
using DotNetOpenMail.Encoding;

namespace DotNetOpenMailTests.SmtpAuth
{
	/// <summary>
	/// Summary description for MockedSmtpProxy.
	/// </summary>
	public abstract class MockedSmtpProxy : ISmtpProxy
	{
		public MockedSmtpProxy()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public virtual SmtpResponse Open()
		{
			throw new ApplicationException("Open Not implemented yet");
		}

		public SmtpResponse Helo(String localHostName) 
		{
			throw new ApplicationException("Helo Not implemented yet");
		}


		public virtual SmtpResponse Auth(String localHostName) 
		{
			Base64Encoder encoder=Base64Encoder.GetInstance();
			return new SmtpResponse(334, encoder.EncodeString("Username:", System.Text.Encoding.ASCII));
		}

		public virtual EhloSmtpResponse Ehlo(String localHostName) 
		{
			EhloSmtpResponse ehloResponse=new EhloSmtpResponse();
			ehloResponse.AddAvailableAuthType("login");			
			ehloResponse.Message="OK";
			ehloResponse.ResponseCode=250;
			return ehloResponse;
		}

		public SmtpResponse MailFrom(EmailAddress rcptto) 
		{
			throw new ApplicationException("MailFrom Not implemented yet");
		}

		public SmtpResponse RcptTo(EmailAddress rcptto) 
		{
			throw new ApplicationException("RcptTo Not implemented yet");
		}

		public SmtpResponse Data() 
		{
			throw new ApplicationException("Data Not implemented yet");
		}

		public SmtpResponse WriteData(String str) 
		{
			throw new ApplicationException("WriteData Not implemented yet");
		}

		public SmtpResponse Quit() 
		{
			throw new ApplicationException("Quit Not implemented yet");
		}

		public void Close() 
		{
			// do nothing
		}

		public SmtpResponse SendString(String str) 
		{
			throw new ApplicationException("SendString Not implemented yet");
		}

		public bool CaptureSmtpConversation {
			get
			{
				return false;
			} 
			set
			{
				// IGNORE
			}
		}

	}
}
