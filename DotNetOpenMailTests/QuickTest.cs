/*
 * Copyright (c) 2005 Mike Bridge <mike@bridgecanada.com>
 * 
 * Permission is hereby granted, free of charge, to any 
 * person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the 
 * Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, 
 * distribute, sublicense, and/or sell copies of the 
 * Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following 
 * conditions:
 *
 * The above copyright notice and this permission notice 
 * shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
 * ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT 
 * SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
 * ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN 
 * ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.IO;

using DotNetOpenMail; 
using DotNetOpenMail.Utils; 

using NUnit.Framework;
using log4net;

namespace DotNetOpenMailTests 
{

	[TestFixture]
	public class QuickTest 
	{


		SmtpServer _smtpserver=null;

		private static readonly ILog log = LogManager.GetLogger(typeof(QuickTest));

		public QuickTest() 
		{

		}

		[SetUp]
		public void SetUp() 
		{
			_smtpserver=TestAddressHelper.GetSmtpServer();
		}

		[TearDown]
		public void TearDown() 
		{
		}

		[Test]
		public void TestSend() 
		{
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			//emailmessage.FromAddress=new EmailAddress("mike@bridgecanada.com", "Bridge, Mike");
			//emailmessage.FromAddress=new EmailAddress("mike@bridgecanada.com", "<Mike Bridge>");

			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.Subject="EmailMessageTests Test HTML and Text";
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>\r\nis the<br>\r\n<strong>HTML</strong><br>\r\npart.</body></html>");

			emailmessage.BodyText="This is a test";

			emailmessage.AddCustomHeader("X-MyHeader1", "my header number one");
			//emailmessage.AddCustomHeader("Sender", TestAddressHelper.GetToAddress().ToString());

			Log4netLogger logger=new Log4netLogger();
			_smtpserver.LogSmtpWrite+=(new SmtpServer.LogHandler(logger.Log));
			_smtpserver.LogSmtpReceive+=(new SmtpServer.LogHandler(logger.Log));
			_smtpserver.LogSmtpCompleted+=(new SmtpServer.LogHandler(logger.Log));
			
			emailmessage.Send(_smtpserver);

		

		}

	
	}
}
