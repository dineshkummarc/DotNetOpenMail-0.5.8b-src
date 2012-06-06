using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

using log4net;

namespace DotNetOpenMail
{
	/// <summary>
	/// Perform the low-level interaction with the external 
	/// SMTP server
	/// </summary>
	public class GenericSmtpNegotiator : TcpClient, ISmtpProxy
	{
		private bool _isConnected=false;

		private static readonly ILog log = LogManager.GetLogger(typeof(GenericSmtpNegotiator));

		private IPEndPoint _ipEndPoint;
		private int _timeout=30000; // default is 30 seconds

		/// <summary>
		/// Create an instance of the GenericSmtpNegotiator
		/// </summary>
		/// <param name="ipEndPoint"></param>
		public GenericSmtpNegotiator(IPEndPoint ipEndPoint)
		{
			this._ipEndPoint=ipEndPoint;
			this.ReceiveTimeout=_timeout;
		}

		#region Open
		/// <summary>
		/// Connect to the server and return the initial 
		/// welcome string. Throw a MailException if we 
		/// can't connect.
		/// </summary>
		/// <returns></returns>
		public SmtpResponse Open() 
		{
			try {
				Connect(_ipEndPoint);
			}
			catch (Exception ex)
			{
				throw new MailException("Could not connect to "+_ipEndPoint.Address+":"+_ipEndPoint.Port, ex);
			} 
			_isConnected=true;
	
			return ReadSmtpResponse();
		}
		#endregion

		#region Helo
		/// <summary>
		/// Send the HELO string.
		/// Throw a MailException if we can't connect.
		/// </summary>
		/// <returns>the SMTP response</returns>
		public SmtpResponse Helo(String localHostName) 
		{
			if (!_isConnected) 
			{
				throw new MailException("The connection is closed.");
			}
			String message = "HELO "+localHostName+SmtpProxy.ENDOFLINE;
			//LogDebug("SENDING: "+message);
				
			Write(message);

			return ReadSmtpResponse();
		}
		#endregion

		#region MailFrom
		/// <summary>
		/// Send the MAIL FROM command
		/// Throw a MailException if we can't connect.
		/// </summary>
		/// <param name="mailfrom">The Envelope-From address</param>
		/// <returns>the SMTP response</returns>
		public SmtpResponse MailFrom(EmailAddress mailfrom) 
		{
			if (!_isConnected) 
			{
				throw new MailException("The connection is closed.");
			}
			String message = "MAIL FROM: <"+mailfrom.Email+">"+SmtpProxy.ENDOFLINE;
			LogDebug("SENDING: "+message);

			Write(message);
			return ReadSmtpResponse();
		}
		#endregion

		#region RcptTo
		/// <summary>
		/// Send the MAIL FROM command
		/// Throw a MailException if we can't connect.
		/// </summary>
		/// <param name="rcpttoaddress">A recipient's address</param>
		/// <returns>the SMTP response</returns>
		public SmtpResponse RcptTo(EmailAddress rcpttoaddress) 
		{
			if (!_isConnected) 
			{
				throw new MailException("The connection is closed.");
			}

			String message= "RCPT TO: <"+rcpttoaddress.Email+">"+SmtpProxy.ENDOFLINE;
			LogDebug("SENDING: "+message);
			Write(message);
			return ReadSmtpResponse();
		}
		#endregion

		#region Data
		/// <summary>
		/// Send the DATA string (without the data)
		/// Throw a MailException if we can't connect.
		/// </summary>
		/// <returns>the SMTP response</returns>
		public SmtpResponse Data() 
		{
			if (!_isConnected) 
			{
				throw new MailException("The connection is closed.");
			}
			String message = "DATA"+SmtpProxy.ENDOFLINE;
			//LogDebug("SENDING: "+message);
				
			Write(message);

			return ReadSmtpResponse();
		}
		#endregion

		#region WriteData
		/// <summary>
		/// Send the message content string
		/// Throw a MailException if we can't 
		/// connect.
		/// </summary>
		/// <returns>the SMTP response</returns>
		public SmtpResponse WriteData(String message) 
		{
			if (!_isConnected) 
			{
				throw new MailException("The connection is closed.");
			}
			
			StringReader reader=new StringReader(message);
			String line=null;

			while ((line=reader.ReadLine())!=null) 				
			{
				// checking for dot at the beginning of the
				// line (RFC821 sec. 4.5.2)
					
				if (line.Length > 0 && line[0]=='.') 
				{
					Write("."+line+SmtpProxy.ENDOFLINE);
				} 
				else 
				{
					Write(line+SmtpProxy.ENDOFLINE);
				}
			}
			Write(SmtpProxy.ENDOFLINE+"."+SmtpProxy.ENDOFLINE);

			return ReadSmtpResponse();
		}
		#endregion

		#region ReadSmtpResponse
		private SmtpResponse ReadSmtpResponse() 
		{
			String response=ReadResponse();
			String responseCodeStr=response.Substring(0, 3);
			String responseMessage="";
			if (response.Length > 4) 
			{				
				responseMessage=response.Substring(4);
			}
			try 
			{
				int responseCode=Convert.ToInt32(responseCodeStr);
				return new SmtpResponse(responseCode, responseMessage);
			}
			catch 
			{
				throw new MailException("Could not understand response from server: "+response);
			}
			
		}
		#endregion

		#region Write
		/// <summary>
		/// Write a string to the current connection.
		/// </summary>
		/// <param name="message"></param>
		private void Write(string message)
		{
			try 
			{
				System.Text.ASCIIEncoding en = new System.Text.ASCIIEncoding() ;
				byte[] WriteBuffer = new byte[1024] ;
				WriteBuffer = en.GetBytes(message) ;
	
				NetworkStream stream = GetStream() ;
				stream.Write(WriteBuffer,0,WriteBuffer.Length);
			}
			catch (Exception ex)
			{
				LogError(ex.Message);
				throw new MailException("Error while sending data to the server: "+ex.Message, ex);
			}

		}
		#endregion

		#region Quit
		/// <summary>
		/// Send the QUIT command
		/// Throw a MailException if we can't connect.
		/// </summary>
		/// <returns>the SMTP response</returns>
		public SmtpResponse Quit() 
		{
			if (!_isConnected) 
			{
				throw new MailException("The connection is closed.");
			}
			Write("QUIT"+SmtpProxy.ENDOFLINE);
			return ReadSmtpResponse();
		}
		#endregion

		#region ReadResponse
		private string ReadResponse()
		{
			try 
			{
				System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
				byte []serverbuff = new Byte[1024];
				NetworkStream stream = GetStream();
				int count = stream.Read( serverbuff, 0, 1024 ); 
				if (count == 0)
				{
					return "";
				}
				return enc.GetString( serverbuff, 0, count ); 
			}
			catch (Exception ex)
			{
				LogError(ex.Message);
				throw new MailException("Error while receiving data from server: "+ex.Message, ex);
			}
		}
		#endregion

		#region LogError
		private void LogError(String message) 
		{
			log.Error(message);
		}
		#endregion

		#region LogDebug
		private void LogDebug(String message) 
		{
			log.Debug(message);
		}
		#endregion

	}
}
