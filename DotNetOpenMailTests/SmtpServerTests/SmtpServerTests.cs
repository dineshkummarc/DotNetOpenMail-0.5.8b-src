using System;
using System.Net;

using DotNetOpenMail; 

using NUnit.Framework;
using log4net;
using NMock;

namespace DotNetOpenMailTests.SmtpServerTests {

	[TestFixture]
	public class SmtpServerTests {

		private static readonly ILog log = LogManager.GetLogger(typeof(SmtpServerTests));

		public SmtpServerTests() {
		}

		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}
		
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
	

		[Test]
		public void TestSimpleSmtpNegotiation()
		{
			SmtpServer smtpserver=new SmtpServer("localhost");
			EmailMessage emailMessage=GetTestHtmlAndTextMessage();

			IMock mockSmtpProxy = new DynamicMock(typeof(ISmtpProxy));
			mockSmtpProxy.ExpectAndReturn("Open", new SmtpResponse(220, "welcome to the mock object server"), null);
			mockSmtpProxy.ExpectAndReturn("Helo", new SmtpResponse(250, "helo"), Dns.GetHostName());
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

			
			emailMessage.Send(smtpserver);


		}


	}
}
