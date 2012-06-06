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

namespace DotNetOpenMailTests {

	[TestFixture]
	public class EmailMessageTests {

		SmtpServer _smtpserver=null;
		private static readonly ILog log = LogManager.GetLogger(typeof(EmailMessageTests));

		public EmailMessageTests() {
		}

		[SetUp]
		public void SetUp() 
		{
			_smtpserver=TestAddressHelper.GetSmtpServer();
		}

		[TearDown]
		public void TearDown() {}

		#region TestHtmlAndText
		[Test]
		public void TestHtmlAndText() 
		{
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.Subject="EmailMessageTests Test HTML and Text";
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>\r\nis the<br>\r\n<strong>HTML</strong><br>\r\npart.</body></html>");

			emailmessage.AddCustomHeader("X-MyHeader1", "my header number one");
			emailmessage.AddCustomHeader("X-MyHeader2", "my header number two");

			emailmessage.Send(_smtpserver);

			String content=emailmessage.ToDataString();
			StringReader sr=new StringReader(content);
			log.Debug(content);

			int i=0;
			String line=null;
			bool hasToHeader=false;
			bool hasFromHeader=false;
			bool hasSubjectHeader=false;
			bool hasMimeVersion=false;
			bool hasMultipartAlternative=false;
			bool hasPlainText=false;
			bool hasHtmlText=false;
			bool hasDefaultXMailerHeader=false;
			bool hasCustomHeader1=false;
			bool hasCustomHeader2=false;
			int quotedPrintableParts=0;

			String expectedToAddress=TestAddressHelper.GetToAddress().Name+" <"+TestAddressHelper.GetToAddress().Email+">";
			
			while ((line=sr.ReadLine())!=null) 
			{
				i++;
				if (line.IndexOf("To: "+expectedToAddress)==0) 
				{
					hasToHeader=true;
				}
				if (line.IndexOf("From: "+emailmessage.FromAddress.ToString())==0) 
				{
					hasFromHeader=true;
				}
				if (line.IndexOf("Subject: "+emailmessage.Subject)==0) 
				{
					hasSubjectHeader=true;
				}
				if (line.IndexOf("MIME-Version: 1.0")==0) 
				{
					hasMimeVersion=true;
				}
				if (line.IndexOf("Content-Type: multipart/alternative;")==0) 
				{
					hasMultipartAlternative=true;
				}
				if (line.IndexOf("Content-Type: text/html")==0) 
				{
					hasHtmlText=true;
				}
				if (line.IndexOf("Content-Type: text/plain")==0) 
				{
					hasPlainText=true;
				}
				if (line.IndexOf("Content-Transfer-Encoding: quoted-printable")==0) 
				{
					quotedPrintableParts++;
				}
				if (line.IndexOf("X-Mailer: DotNetOpenMail") == 0)
				{
					hasDefaultXMailerHeader=true;
				}
				if (line.IndexOf("X-MyHeader1: my header number one") == 0)
				{
					hasCustomHeader1=true;
				}
				if (line.IndexOf("X-MyHeader2: my header number two") == 0)
				{
					hasCustomHeader2=true;
				}

				// log.Debug("Line "+i+": "+line);				
			}
			Assert.IsTrue(hasToHeader, "Missing TO Header");
			Assert.IsTrue(hasFromHeader, "Missing FROM Header");
			Assert.IsTrue(hasSubjectHeader, "Missing Subject Header");
			Assert.IsTrue(hasMimeVersion, "Missing Mime Version header");
			Assert.IsTrue(hasMultipartAlternative, "Missing Mime Multipart/Alternative setting");
			Assert.IsTrue(hasPlainText, "Missing Plain Text");
			Assert.IsTrue(hasHtmlText, "Missing HTML");
			Assert.IsTrue(hasDefaultXMailerHeader, "Missing X-Mailer Header");
			Assert.IsTrue(hasCustomHeader1, "Missing Custom Header 1");
			Assert.IsTrue(hasCustomHeader2, "Missing Custom Header 2");
			Assert.AreEqual(2, quotedPrintableParts, "Expected 2 quoted printable declarations, found "+quotedPrintableParts);
		
			
		

		}
		#endregion

		#region TestGraphicAttachment
		[Test]
		public void TestGraphicAttachment() 
		{
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.Subject="EmailMessageTests Test HTML+Text+Graphic";
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>\r\nis the<br>\r\n<strong>HTML</strong><br>\r\npart.</body></html>");
			FileInfo fileinfo=new FileInfo(@"..\..\TestFiles\grover.jpg");
			Assert.IsTrue(fileinfo.Exists);
			FileAttachment fileattachment=new FileAttachment(fileinfo);
			
			fileattachment.ContentType="image/jpeg";
			emailmessage.AddMixedAttachment(fileattachment);
			//emailmessage.Send(_smtpserver);
			//log.Debug("MESSAGE: "+emailmessage.ToDataString());
			String content=emailmessage.ToDataString();
			StringReader sr=new StringReader(content);
			
			int i=0;
			String line=null;
			bool hasMimeVersion=false;
			bool hasMixedHeader=false;
			bool mixedHeaderComesFirst=false;
			bool hasMultipartAlternative=false;
			bool hasPlainText=false;
			bool hasHtmlText=false;
			bool hasAttachment=false;
			bool hasRelatedHeader=false;
			int quotedPrintableParts=0;

			String expectedToAddress=TestAddressHelper.GetToAddress().Name+" <"+TestAddressHelper.GetToAddress().Email+">";
			//log.Debug("To Address is "+expectedToAddress);
			while ((line=sr.ReadLine())!=null) 
			{
				i++;
				if (line.IndexOf("Content-Type: multipart/mixed")==0) 
				{
					hasMixedHeader=true;
					if (!hasMultipartAlternative) 
					{
						mixedHeaderComesFirst=true;
					}
				}
				if (line.IndexOf("MIME-Version: 1.0")==0) 
				{
					hasMimeVersion=true;
				}
				if (line.IndexOf("Content-Type: multipart/alternative;")==0) 
				{
					hasMultipartAlternative=true;
				}
				if (line.IndexOf("Content-Type: text/html")==0) 
				{
					hasHtmlText=true;
				}
				if (line.IndexOf("Content-Type: text/plain")==0) 
				{
					hasPlainText=true;
				}
				if (line.IndexOf("Content-Type: image/jpeg")==0) 
				{
					hasAttachment=true;
				}
				if (line.IndexOf("Content-Transfer-Encoding: quoted-printable")==0) 
				{
					quotedPrintableParts++;
				}
				if (line.IndexOf("/9j/4AAQSkZJRgABAQEASABIAAD/")==0) 
				{
					hasAttachment=true;
				}
				if (line.IndexOf("Content-Type: multipart/related")>=0)
				{
					hasRelatedHeader=true;
				}
				//log.Debug("Line "+i+": "+line);				
			}
			Assert.IsTrue(hasMixedHeader, "Missing multipart/mixed header");
			Assert.IsTrue(hasMimeVersion, "Missing Mime Version header");
			Assert.IsTrue(mixedHeaderComesFirst, "The mixed header should come first");
			Assert.IsTrue(hasMultipartAlternative, "Missing Mime Multipart/Alternative setting");
			Assert.IsTrue(hasPlainText, "Missing Plain Text");
			Assert.IsTrue(hasHtmlText, "Missing HTML");
			Assert.IsTrue(hasAttachment, "Missing the base64 Attachment");
			Assert.IsFalse(hasRelatedHeader, "Should not have a related header.");
			Assert.AreEqual(2, quotedPrintableParts, "Expected 2 quoted printable declarations, found "+quotedPrintableParts);

		}
		#endregion

		#region TestRelatedAndMixedAttachments
		[Test]
		public void TestRelatedAndMixedAttachments() 
		{
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.Subject="EmailMessageTests Test HTML+Text+Graphic";
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>  \r\nis the<br><img src=\"cid:mycontentid@rhombus\">  \r\n<strong>HTML</strong><br>\r\npart.</body></html>");
			FileInfo relatedfileinfo=new FileInfo(@"..\..\TestFiles\grover.jpg");
			FileInfo mixedfileinfo=new FileInfo(@"..\..\TestFiles\groverUpsideDown.jpg");
			Assert.IsTrue(relatedfileinfo.Exists);
			Assert.IsTrue(mixedfileinfo.Exists);
			FileAttachment relatedfileattachment=new FileAttachment(relatedfileinfo,"mycontentid@rhombus");
			relatedfileattachment.ContentType="image/jpeg";
			FileAttachment mixedfileattachment=new FileAttachment(mixedfileinfo);
			mixedfileattachment.ContentType="image/jpeg";
			emailmessage.AddRelatedAttachment(relatedfileattachment);
			emailmessage.AddMixedAttachment(mixedfileattachment);
			//emailmessage.Send(_smtpserver);
			String content=emailmessage.ToDataString();
			StringReader sr=new StringReader(content);
			
			int i=0;
			String line=null;
			bool hasMimeVersion=false;
			bool hasMixedHeader=false;
			bool mixedHeaderComesFirst=false;
			bool hasMultipartAlternative=false;
			bool hasPlainText=false;
			bool hasHtmlText=false;
			bool hasAttachment=false;
			bool hasRelatedHeader=false;
			int quotedPrintableParts=0;

			String expectedToAddress=TestAddressHelper.GetToAddress().Name+" <"+TestAddressHelper.GetToAddress().Email+">";
			//log.Debug("To Address is "+expectedToAddress);
			while ((line=sr.ReadLine())!=null) 
			{
				i++;
				if (line.IndexOf("Content-Type: multipart/mixed")==0) 
				{
					hasMixedHeader=true;
					if (!hasMultipartAlternative && !hasRelatedHeader) 
					{
						mixedHeaderComesFirst=true;
					}
				}
				if (line.IndexOf("MIME-Version: 1.0")==0) 
				{
					hasMimeVersion=true;
				}
				if (line.IndexOf("Content-Type: multipart/alternative;")==0) 
				{
					hasMultipartAlternative=true;
				}
				if (line.IndexOf("Content-Type: text/html")==0) 
				{
					hasHtmlText=true;
				}
				if (line.IndexOf("Content-Type: text/plain")==0) 
				{
					hasPlainText=true;
				}
				if (line.IndexOf("Content-Type: image/jpeg")==0) 
				{
					hasAttachment=true;
				}
				if (line.IndexOf("Content-Transfer-Encoding: quoted-printable")==0) 
				{
					quotedPrintableParts++;
				}
				if (line.IndexOf("/9j/4AAQSkZJRgABAQEASABIAAD/")==0) 
				{
					hasAttachment=true;
				}
				if (line.IndexOf("Content-Type: multipart/related")>=0)
				{
					hasRelatedHeader=true;
				}
				//log.Debug("Line "+i+": "+line);				
			}
			Assert.IsTrue(hasMixedHeader, "Missing multipart/mixed header");
			Assert.IsTrue(hasMimeVersion, "Missing Mime Version header");
			Assert.IsTrue(mixedHeaderComesFirst, "The mixed header should come first");
			Assert.IsTrue(hasMultipartAlternative, "Missing Mime Multipart/Alternative setting");
			Assert.IsTrue(hasPlainText, "Missing Plain Text");
			Assert.IsTrue(hasHtmlText, "Missing HTML");
			Assert.IsTrue(hasAttachment, "Missing the base64 Attachment");
			Assert.IsTrue(hasRelatedHeader, "Missing the related header.");
			Assert.AreEqual(2, quotedPrintableParts, "Expected 2 quoted printable declarations, found "+quotedPrintableParts);

		}
		#endregion

		#region TestRelatedAttachments
		[Test]
		public void TestRelatedAttachments() 
		{

			EmailMessage emailmessage=new EmailMessage();

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.Subject="EmailMessageTests Test Related Graphic";
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>\r\nis the<br><img src=\"cid:mycontentid\">\r\n<strong>HTML</strong><br>\r\npart.</body></html>");
			FileInfo relatedfileinfo=new FileInfo(@"..\..\TestFiles\grover.jpg");
			Assert.IsTrue(relatedfileinfo.Exists);
			FileAttachment relatedfileattachment=new FileAttachment(relatedfileinfo,"mycontentid");
			relatedfileattachment.ContentType="image/jpeg";			
			emailmessage.AddRelatedAttachment(relatedfileattachment);
			//emailmessage.Send(_smtpserver);
			String content=emailmessage.ToDataString();
			StringReader sr=new StringReader(content);
			
			int i=0;
			String line=null;
			bool hasMimeVersion=false;
			bool hasMixedHeader=false;
			bool relatedHeaderComesFirst=false;
			bool hasMultipartAlternative=false;
			bool hasPlainText=false;
			bool hasHtmlText=false;
			bool hasAttachment=false;
			bool hasRelatedHeader=false;
			int quotedPrintableParts=0;

			String expectedToAddress=TestAddressHelper.GetToAddress().Name+" <"+TestAddressHelper.GetToAddress().Email+">";
			//log.Debug("To Address is "+expectedToAddress);
			while ((line=sr.ReadLine())!=null) 
			{
				i++;
				if (line.IndexOf("Content-Type: multipart/mixed")==0) 
				{
					hasMixedHeader=true;

				}

				if (line.IndexOf("MIME-Version: 1.0")==0) 
				{
					hasMimeVersion=true;
				}
				if (line.IndexOf("Content-Type: multipart/alternative;")==0) 
				{
					hasMultipartAlternative=true;
				}
				if (line.IndexOf("Content-Type: text/html")==0) 
				{
					hasHtmlText=true;
				}
				if (line.IndexOf("Content-Type: text/plain")==0) 
				{
					hasPlainText=true;
				}
				if (line.IndexOf("Content-Type: image/jpeg")==0) 
				{
					hasAttachment=true;
				}
				if (line.IndexOf("Content-Transfer-Encoding: quoted-printable")==0) 
				{
					quotedPrintableParts++;
				}
				if (line.IndexOf("/9j/4AAQSkZJRgABAQEASABIAAD/")==0) 
				{
					hasAttachment=true;
				}
				if (line.IndexOf("Content-Type: multipart/related")>=0)
				{
					hasRelatedHeader=true;
					if (!hasMultipartAlternative && !hasMixedHeader) 
					{
						relatedHeaderComesFirst=true;
					}
				}
				//log.Debug("Line "+i+": "+line);				
			}
			Assert.IsFalse(hasMixedHeader, "Should not have a  multipart/mixed header");
			Assert.IsTrue(hasMimeVersion, "Missing Mime Version header");
			Assert.IsTrue(relatedHeaderComesFirst, "The related header should come first");
			Assert.IsTrue(hasMultipartAlternative, "Missing Mime Multipart/Alternative setting");
			Assert.IsTrue(hasPlainText, "Missing Plain Text");
			Assert.IsTrue(hasHtmlText, "Missing HTML");
			Assert.IsTrue(hasAttachment, "Missing the base64 Attachment");
			Assert.IsTrue(hasRelatedHeader, "Missing the related header.");
			Assert.AreEqual(2, quotedPrintableParts, "Expected 2 quoted printable declarations, found "+quotedPrintableParts);


		}
		#endregion

		#region TestHeaders
		[Test]
		public void TestHeaders() 
		{
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.AddCcAddress(TestAddressHelper.GetToAddress());
			emailmessage.AddCcAddress(TestAddressHelper.GetToAddress());
			emailmessage.AddBccAddress(TestAddressHelper.GetToAddress());
			emailmessage.AddBccAddress(TestAddressHelper.GetToAddress());		
			emailmessage.Subject="Test Headers";
			emailmessage.XMailer="NUnit Test Mailer";
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>\r\nis the<br>\r\n<strong>HTML</strong><br>\r\npart.</body></html>");

			String content=emailmessage.ToDataString();
			StringReader sr=new StringReader(content);
			
			int i=0;
			String line=null;
			bool hasToHeader=false;
			bool hasFromHeader=false;
			bool hasSubjectHeader=false;
			bool hasBccHeader=false;
			bool hasCcHeader=false;
			bool hasXMailerHeader=false;

			String expectedToAddress=TestAddressHelper.GetToAddress().Name+" <"+TestAddressHelper.GetToAddress().Email+">";


			while ((line=sr.ReadLine())!=null) 
			{
				i++;
				if (line.IndexOf("To: "+expectedToAddress+", "+expectedToAddress)==0) 
				{
					hasToHeader=true;
				}
				if (line.IndexOf("Cc: "+expectedToAddress+", "+expectedToAddress)==0) 
				{
					hasCcHeader=true;
				}
				if (line.IndexOf("Bcc: "+expectedToAddress+", "+expectedToAddress)==0) 
				{
					hasBccHeader=true;
				}

				if (line.IndexOf("From: "+emailmessage.FromAddress.ToString())==0) 
				{
					hasFromHeader=true;
				}
				if (line.IndexOf("Subject: "+emailmessage.Subject)==0) 
				{
					hasSubjectHeader=true;
				}
				if (line.IndexOf("X-Mailer: NUnit Test Mailer")==0) 
				{
					hasXMailerHeader=true;
				}
				
			}
			Assert.IsTrue(hasToHeader, "Missing TO Header");
			Assert.IsTrue(hasFromHeader, "Missing FROM Header");
			Assert.IsTrue(hasSubjectHeader, "Missing Subject Header");
			Assert.IsTrue(hasCcHeader, "Missing Cc Header");
			Assert.IsFalse(hasBccHeader, "Bcc Header Shouldn't appear");
			Assert.IsTrue(hasXMailerHeader, "Missing XMailer Header");
			//emailmessage.Send(_smtpserver);

		}
		#endregion

		#region TestLongSubjectEncoded
		[Test]
		public void TestLongSubjectEncoded() 
		{
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.AddCcAddress(TestAddressHelper.GetToAddress());
			emailmessage.AddBccAddress(TestAddressHelper.GetToAddress());
			emailmessage.Subject="Join our Group's Fundraising Efforts é test test test Gulf Little League";
			emailmessage.HeaderEncoding=DotNetOpenMail.Encoding.EncodingType.QuotedPrintable;
			emailmessage.XMailer="NUnit Test Mailer";
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>\r\nis the<br>\r\n<strong>HTML</strong><br>\r\npart.</body></html>");

			String content=emailmessage.ToDataString();
			emailmessage.Send(_smtpserver);

		}
		#endregion

		#region TestExtraHeader
		[Test]
		public void TestExtraHeader() 
		{
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.AddCcAddress(TestAddressHelper.GetToAddress());
			emailmessage.AddBccAddress(TestAddressHelper.GetToAddress());
			emailmessage.Subject="Extra Header";
			emailmessage.HeaderEncoding=DotNetOpenMail.Encoding.EncodingType.QuotedPrintable;
			emailmessage.XMailer="NUnit Test Mailer";
			emailmessage.AddCustomHeader("MyHeader", "Value");
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>\r\nis the<br>\r\n<strong>HTML</strong><br>\r\npart.</body></html>");

			String content=emailmessage.ToDataString();
			emailmessage.Send(_smtpserver);

		}
		#endregion

		#region TestMissingFrom
		[Test]
		public void TestMissingFrom() 
		{
			EmailMessage emailmessage=new EmailMessage();
			try 
			{
				emailmessage.Send(_smtpserver);
				Assert.Fail("This should throw an error if no from address");
			}
			catch (MailException ex)
			{
				log.Debug("Ignoring exception "+ex.Message);
			}
			
		}
		#endregion


	}
}

