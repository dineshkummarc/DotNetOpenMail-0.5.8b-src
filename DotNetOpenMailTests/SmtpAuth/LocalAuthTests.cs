using System;
using System.Net;

using DotNetOpenMail;
using DotNetOpenMail.SmtpAuth;
using DotNetOpenMail.Encoding;

using NUnit.Framework;
using log4net;
using NMock;

namespace DotNetOpenMailTests.SmtpAuth {

	[TestFixture]
	public class LocalAuthTests {

		private static readonly ILog log = LogManager.GetLogger(typeof(LocalAuthTests));

		public LocalAuthTests() {
		}

		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}

		#region GetTestHtmlAndTextMessage
		private EmailMessage GetTestHtmlAndTextMessage() 
		{
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.Subject="EmailMessageTests Test HTML and Text";
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>\r\nis the<br>\r\n<strong>HTML</strong><br>\r\npart.</body></html>");

			emailmessage.AddCustomHeader("X-MyHeader1", "my header number one");
			emailmessage.AddCustomHeader("X-MyHeader2", "my header number two");

			return emailmessage;
		}
		#endregion
	
		/*
		 * Test for this exchange:
		 * S: 220 esmtp.example.com ESMTP
		 * C: ehlo client.example.com
		 * S: 250-esmtp.example.com
		 * S: 250-PIPELINING
		 * S: 250-8BITMIME
		 * S: 250-SIZE 255555555
		 * S: 250 AUTH LOGIN PLAIN CRAM-MD5
		 * C: auth login
		 * S: 334 VXNlcm5hbWU6
		 * C: avlsdkfj
		 * S: 334 UGFzc3dvcmQ6
		 * C: lkajsdfvlj
		 * S: 535 authentication failed (#5.7.1)
		 */

		#region TestSimpleSmtpNegotiationWithAuth
		[Test]
		public void TestSimpleSmtpNegotiationWithAuth()
		{


			SmtpServer smtpserver=new SmtpServer("localhost");
			smtpserver.SmtpAuthToken=new SmtpAuthToken("test", "test");			
			
			
			Base64Encoder encoder=Base64Encoder.GetInstance();
			String base64Username=encoder.EncodeString(smtpserver.SmtpAuthToken.UserName, System.Text.Encoding.ASCII );
			String base64Password=encoder.EncodeString(smtpserver.SmtpAuthToken.Password, System.Text.Encoding.ASCII );

			EmailMessage emailMessage=GetTestHtmlAndTextMessage();

			IMock mockSmtpProxy = new DynamicMock(typeof(ISmtpProxy));
			mockSmtpProxy.ExpectAndReturn("Open", new SmtpResponse(220, "welcome to the mock object server"), null);

			EhloSmtpResponse ehloResponse=new EhloSmtpResponse();
			ehloResponse.AddAvailableAuthType("login");
			
			ehloResponse.Message="OK";
			ehloResponse.ResponseCode=250;
			mockSmtpProxy.ExpectAndReturn("Ehlo",ehloResponse , Dns.GetHostName());
			mockSmtpProxy.ExpectAndReturn("Auth", new SmtpResponse(334, encoder.EncodeString("Username:", System.Text.Encoding.ASCII)), "login");
			mockSmtpProxy.ExpectAndReturn("SendString", new SmtpResponse(334, encoder.EncodeString("Password:",System.Text.Encoding.ASCII)), base64Username);
			mockSmtpProxy.ExpectAndReturn("SendString", new SmtpResponse(235, "Hooray, Authenticated"), base64Password);
			mockSmtpProxy.ExpectAndReturn("MailFrom", new SmtpResponse(250, "mail from"), emailMessage.FromAddress);
			foreach (EmailAddress rcpttoaddr in emailMessage.ToAddresses) 
			{
				mockSmtpProxy.ExpectAndReturn("RcptTo", new SmtpResponse(250, "receipt to"), rcpttoaddr);
			}
			mockSmtpProxy.ExpectAndReturn("Data", new SmtpResponse(354, "data open"), null);
			mockSmtpProxy.ExpectAndReturn("WriteData", new SmtpResponse(250, "data"), emailMessage.ToDataString());
			
			mockSmtpProxy.ExpectAndReturn("Quit", new SmtpResponse(221, "quit"), null);
			mockSmtpProxy.Expect("Close", null);

			ISmtpProxy smtpProxy= (ISmtpProxy) mockSmtpProxy.MockInstance;
			

			smtpserver.OverrideSmtpProxy(smtpProxy);
			
			try 
			{
				emailMessage.Send(smtpserver);
			}
			catch (SmtpException ex)
			{
				log.Debug("Exception was "+ex.Message);
				log.Debug(ex.StackTrace);
				throw ex;
			}

		}
		#endregion

		#region TestForcedLoginSmtpNegotiationWithAuth
		[Test]
		public void TestForcedLoginSmtpNegotiationWithAuth()
		{


			SmtpServer smtpserver=new SmtpServer("localhost");
			smtpserver.SmtpAuthToken=new LoginAuthToken("test", "testtest");
			
			
			Base64Encoder encoder=Base64Encoder.GetInstance();
			String base64Username=encoder.EncodeString(smtpserver.SmtpAuthToken.UserName, System.Text.Encoding.ASCII );
			String base64Password=encoder.EncodeString(smtpserver.SmtpAuthToken.Password, System.Text.Encoding.ASCII );

			EmailMessage emailMessage=GetTestHtmlAndTextMessage();

			IMock mockSmtpProxy = new DynamicMock(typeof(ISmtpProxy));
			mockSmtpProxy.ExpectAndReturn("Open", new SmtpResponse(220, "welcome to the mock object server"), null);

			EhloSmtpResponse ehloResponse=new EhloSmtpResponse();
			ehloResponse.AddAvailableAuthType("login");
			
			ehloResponse.Message="OK";
			ehloResponse.ResponseCode=250;
			mockSmtpProxy.ExpectAndReturn("Ehlo",ehloResponse , Dns.GetHostName());
			
			mockSmtpProxy.ExpectAndReturn("Auth", new SmtpResponse(334, encoder.EncodeString("Username:", System.Text.Encoding.ASCII)), "login");
			mockSmtpProxy.ExpectAndReturn("SendString", new SmtpResponse(334, encoder.EncodeString("Password:",System.Text.Encoding.ASCII)), base64Username);
			mockSmtpProxy.ExpectAndReturn("SendString", new SmtpResponse(235, "Hooray, Authenticated"), base64Password);
			mockSmtpProxy.ExpectAndReturn("MailFrom", new SmtpResponse(250, "mail from"), emailMessage.FromAddress);
			foreach (EmailAddress rcpttoaddr in emailMessage.ToAddresses) 
			{
				mockSmtpProxy.ExpectAndReturn("RcptTo", new SmtpResponse(250, "receipt to"), rcpttoaddr);
			}
			mockSmtpProxy.ExpectAndReturn("Data", new SmtpResponse(354, "data open"), null);
			mockSmtpProxy.ExpectAndReturn("WriteData", new SmtpResponse(250, "data"), emailMessage.ToDataString());
			
			mockSmtpProxy.ExpectAndReturn("Quit", new SmtpResponse(221, "quit"), null);
			mockSmtpProxy.Expect("Close", null);

			ISmtpProxy smtpProxy= (ISmtpProxy) mockSmtpProxy.MockInstance;
			

			smtpserver.OverrideSmtpProxy(smtpProxy);
			
			try 
			{
				emailMessage.Send(smtpserver);
			}
			catch (SmtpException ex)
			{
				log.Debug("Exception was "+ex.Message);
				log.Debug(ex.StackTrace);
				throw ex;
			}

		}
		#endregion

		#region TestFailedSmtpNegotiationWithAuth
		[Test]
		public void TestFailedSmtpNegotiationWithAuth()
		{


			SmtpServer smtpserver=new SmtpServer("localhost");
			smtpserver.SmtpAuthToken=new SmtpAuthToken("test", "test");			
			
			
			Base64Encoder encoder=Base64Encoder.GetInstance();
			String base64Username=encoder.EncodeString(smtpserver.SmtpAuthToken.UserName, System.Text.Encoding.ASCII );
			String base64Password=encoder.EncodeString(smtpserver.SmtpAuthToken.Password, System.Text.Encoding.ASCII );

			EmailMessage emailMessage=GetTestHtmlAndTextMessage();

			IMock mockSmtpProxy = new DynamicMock(typeof(ISmtpProxy));
			mockSmtpProxy.ExpectAndReturn("Open", new SmtpResponse(220, "welcome to the mock object server"), null);

			EhloSmtpResponse ehloResponse=new EhloSmtpResponse();
			ehloResponse.AddAvailableAuthType("login");
			
			ehloResponse.Message="OK";
			ehloResponse.ResponseCode=250;
			mockSmtpProxy.ExpectAndReturn("Ehlo", ehloResponse , Dns.GetHostName());
			mockSmtpProxy.ExpectAndReturn("Auth", new SmtpResponse(554, "Unrecognized auth type"), "login");

			ISmtpProxy smtpProxy= (ISmtpProxy) mockSmtpProxy.MockInstance;

			smtpserver.OverrideSmtpProxy(smtpProxy);

			/*
			mockSmtpProxy.ExpectAndReturn("SendString", new SmtpResponse(554, "Invalid UserName"), base64Username);
			mockSmtpProxy.ExpectAndReturn("SendString", new SmtpResponse(554, "Invalid Password"), base64Password);
			mockSmtpProxy.ExpectAndReturn("MailFrom", new SmtpResponse(250, "mail from"), emailMessage.FromAddress);
			foreach (EmailAddress rcpttoaddr in emailMessage.ToAddresses) 
			{
				mockSmtpProxy.ExpectAndReturn("RcptTo", new SmtpResponse(250, "receipt to"), rcpttoaddr);
			}
			mockSmtpProxy.ExpectAndReturn("Data", new SmtpResponse(354, "data open"), null);
			mockSmtpProxy.ExpectAndReturn("WriteData", new SmtpResponse(250, "data"), emailMessage.ToDataString());
			
			mockSmtpProxy.ExpectAndReturn("Quit", new SmtpResponse(221, "quit"), null);
			mockSmtpProxy.Expect("Close", null);
			*/

			try 
			{
				emailMessage.Send(smtpserver);
				Assert.Fail("The auth type is wrong");
			}
			catch (SmtpException ex)
			{
				log.Debug("ERROR CODE IS "+554);
				Assert.AreEqual(554,ex.ErrorCode);
			}



		}
		#endregion

		#region TestSmtpNegotiationWithAuthTimeout
		[Test]
		[Ignore("Not completed yet")]
		public void TestSmtpNegotiationWithAuthTimeout()
		{


			SmtpServer smtpserver=new SmtpServer("localhost");
			smtpserver.SmtpAuthToken=new LoginAuthToken("test", "test");
			
			
			Base64Encoder encoder=Base64Encoder.GetInstance();
			String base64Username=encoder.EncodeString(smtpserver.SmtpAuthToken.UserName, System.Text.Encoding.ASCII );
			String base64Password=encoder.EncodeString(smtpserver.SmtpAuthToken.Password, System.Text.Encoding.ASCII );

			EmailMessage emailMessage=GetTestHtmlAndTextMessage();

			/*
			IMock mockSmtpProxy = new DynamicMock(typeof(ISmtpProxy));
			
			mockSmtpProxy.ExpectAndReturn("Open", new SmtpResponse(220, "welcome to the mock object server"), null);
			
			EhloSmtpResponse ehloResponse=new EhloSmtpResponse();
			ehloResponse.AddAvailableAuthType("login");
			
			ehloResponse.Message="OK";
			ehloResponse.ResponseCode=250;
			mockSmtpProxy.ExpectAndReturn("Ehlo",ehloResponse , Dns.GetHostName());
			
			mockSmtpProxy.ExpectAndReturn("Auth", new SmtpResponse(334, encoder.EncodeString("Username:", System.Text.Encoding.ASCII)), "login");
			mockSmtpProxy.ExpectAndReturn("SendString", new SmtpResponse(334, encoder.EncodeString("Password:",System.Text.Encoding.ASCII)), base64Username);
			mockSmtpProxy.ExpectAndReturn("SendString", new SmtpResponse(235, "Hooray, Authenticated"), base64Password);
			mockSmtpProxy.ExpectAndReturn("MailFrom", new SmtpResponse(250, "mail from"), emailMessage.FromAddress);
			
			foreach (EmailAddress rcpttoaddr in emailMessage.ToAddresses) 
			{
				mockSmtpProxy.ExpectAndReturn("RcptTo", new SmtpResponse(250, "receipt to"), rcpttoaddr);
			}
			mockSmtpProxy.ExpectAndReturn("Data", new SmtpResponse(354, "data open"), null);
			mockSmtpProxy.ExpectAndReturn("WriteData", new SmtpResponse(250, "data"), emailMessage.ToDataString());
			
			mockSmtpProxy.ExpectAndReturn("Quit", new SmtpResponse(221, "quit"), null);
			mockSmtpProxy.Expect("Close", null);

			ISmtpProxy smtpProxy= (ISmtpProxy) mockSmtpProxy.MockInstance;
			*/
			
			smtpserver.OverrideSmtpProxy(new MockedSlowProxy());
			try 
			{
				emailMessage.Send(smtpserver);
				Assert.Fail("This should have thrown an SmtpException indicating a timeout.");
			}
			catch (SmtpException ex)
			{
				log.Debug("Exception was "+ex.Message);
				log.Debug(ex.StackTrace);
				//throw ex;
			}

		}
		#endregion



	}
}

