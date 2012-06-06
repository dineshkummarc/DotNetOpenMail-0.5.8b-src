using System;
using System.IO;
using DotNetOpenMail; // change to tested dir

using NUnit.Framework;
using log4net;

namespace DotNetOpenMailTests {

	[TestFixture]
	public class ExampleTests {

		private static readonly ILog log = LogManager.GetLogger(typeof(ExampleTests));

		public ExampleTests() {
		}

		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void TestOne() 
		{
			EmailMessage emailmessage=new EmailMessage();
	
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
	
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());	
	
			emailmessage.Subject="Missed you";
	
			emailmessage.TextPart=new TextAttachment("Just checking where "+
				"you were last night.\r\nSend me a note!\r\n\r\n-Charles");
		
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>"+
				"<p>Just checking up on where you were last night.</p>\r\n"+
				"<p>Send me a note!</p>\r\n\r\n"+
				"<p>-Charles</p></body></html>");
		
			SmtpServer smtpserver=TestAddressHelper.GetSmtpServer();
			//smtpserver.CaptureSmtpConversation=true;

			try 
			{
				emailmessage.Send(smtpserver);		
				
			}
			finally 
			{
				//log.Debug(smtpserver.GetSmtpConversation());
				//Assert.IsNotNull(smtpserver.GetSmtpConversation());
				//smtpserver.CaptureSmtpConversation=false;	
			}
		}
		[Test]
		public void TestTwo() 
		{
			EmailMessage emailmessage=new EmailMessage();
	
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
	
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());	
	
			emailmessage.Subject="A photo of hawthornes";
	
			emailmessage.TextPart=new TextAttachment("This photo requires a better email reader.");		
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>"+
				"<p>Note to self: look at this photo again in 30 years.</p>"+
				"<p><img src=\"cid:hawthornes\" alt=\"Hawthorne bush\"/></p>"+
				"<p>-Marcel</p>");
			
			FileInfo relatedfileinfo=new FileInfo(@"..\..\TestFiles\grover.jpg");
	
			FileAttachment relatedfileattachment=new FileAttachment(relatedfileinfo,"hawthornes");
	
			relatedfileattachment.ContentType="image/jpeg";
			
			emailmessage.AddRelatedAttachment(relatedfileattachment);
			
			SmtpServer smtpserver=TestAddressHelper.GetSmtpServer();
			emailmessage.Send(smtpserver);
			//Assert.IsNull(smtpserver.GetSmtpConversation());
		}

		[Test]
		public void TestThree() 
		{
			EmailMessage emailmessage=new EmailMessage();
	
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
	
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
	
			emailmessage.Subject="Something has come up";
	
			emailmessage.TextPart=new TextAttachment("I regret that something has "+
				"come up unexpectedly,\r\n"+
				"and I must postpone our meeting.\r\n\r\n"+	
				"Please read the 20 pages of my thoughts on this in the attached\r\n"+
				"PDF file.\r\n\r\n-Marcel");
		
			emailmessage.HtmlPart=new HtmlAttachment("<p>I regret that something "+
				"has come up unexpectedly,\r\n"+
				"and I must postpone our meeting.</p>\r\n"+	
				"<p>Please read the 20 pages of my thoughts on this in the attached\r\n"+
				"PDF file.</p>\r\n<p>-Marcel</p>");
			
			FileAttachment fileattachment=new FileAttachment(new FileInfo(@"..\..\TestFiles\grover.jpg"));
	
			fileattachment.ContentType="image/jpeg";
			
			emailmessage.AddMixedAttachment(fileattachment);
		
			emailmessage.Send(TestAddressHelper.GetSmtpServer());
	
		}

	}
}
