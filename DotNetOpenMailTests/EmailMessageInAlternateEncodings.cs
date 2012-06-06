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
using DotNetOpenMail.Encoding; 

using NUnit.Framework;
using log4net;

namespace DotNetOpenMailTests 
{

	[TestFixture]
	public class EmailMessageInAlternateEncodings 
	{

		SmtpServer _smtpserver=null;
		private static readonly ILog log = LogManager.GetLogger(typeof(EmailMessageInAlternateEncodings));

		public EmailMessageInAlternateEncodings() 
		{
		}

		[SetUp]
		public void SetUp() 
		{
			_smtpserver=TestAddressHelper.GetSmtpServer();
		}

		[TearDown]
		public void TearDown() {}

		#region TestJapaneseHtmlAndTextQPEncoding
		[Test]
		public void TestJapaneseHtmlAndTextQPEncoding() 
		{
			//SmtpServer smtpserver=TestAddressHelper.GetSmtpServer();
			System.Text.Encoding encoding=System.Text.Encoding.GetEncoding("Shift_JIS");

			EmailAddress japanesefromaddress=new EmailAddress(TestAddressHelper.GetFromAddress().Email, "日本語", EncodingType.QuotedPrintable, encoding);
			EmailAddress japanesetoaddress=new EmailAddress(TestAddressHelper.GetToAddress().Email, "日本語", EncodingType.QuotedPrintable, encoding);
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.HeaderEncoding=DotNetOpenMail.Encoding.EncodingType.QuotedPrintable;
			System.Text.Encoding jencoding=System.Text.Encoding.GetEncoding("iso-2022-jp");
			//emailmessage.HeaderCharSet=System.Text.Encoding.GetEncoding("Shift_JIS");
			emailmessage.HeaderCharSet=jencoding;
			
			log.Debug("ENCODING IS "+jencoding.EncodingName);
			log.Debug("IN HEADER:"+jencoding.HeaderName);
			log.Debug("IN BODY:"+jencoding.BodyName);
			log.Debug("CODE PAGE:"+jencoding.CodePage);
			log.Debug("WebName:"+jencoding.WebName);
			log.Debug("WINDOWS CODE PAGE:"+jencoding.WindowsCodePage);
			emailmessage.FromAddress=japanesefromaddress;
			emailmessage.AddToAddress(japanesetoaddress);
			emailmessage.Subject="日本語 - Quoted Printable";
			emailmessage.TextPart=new TextAttachment("東京、日本語");
			//emailmessage.TextPart.CharSet+AD0AIg-Shift_JIS+ACIAOw-
			emailmessage.TextPart.CharSet=jencoding;
			emailmessage.TextPart.Encoding=DotNetOpenMail.Encoding.EncodingType.QuotedPrintable;
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>東京、日本語</body></html>");
			//emailmessage.HtmlPart.CharSet+AD0AIg-Shift_JIS+ACIAOw-
			emailmessage.HtmlPart.CharSet=jencoding;
			emailmessage.HtmlPart.Encoding=DotNetOpenMail.Encoding.EncodingType.QuotedPrintable;

			emailmessage.Send(_smtpserver);

			String content=emailmessage.ToDataString();
			StringReader sr=new StringReader(content);
			log.Debug(content);

			int i=0;
			String line=null;


			bool toHeaderEncoded=false;
			bool fromHeaderEncoded=false;
			bool subjectHeaderEncoded=false;
			bool htmlEncoded=false;
			bool textEncoded=false;
			bool hasPlainText=false;
			bool hasHtmlText=false;

			
			
			while ((line=sr.ReadLine())!=null) 
			{
				i++;
				//log.Debug("FOUND +ACIAKw-line);
				if (line.IndexOf("To: =?iso-2022-jp?Q?=93=FA=96{=8C=EA?= <"+japanesetoaddress.Email+">")==0)
				{					
					toHeaderEncoded=true;
				}
				if (line.IndexOf("From") == 0) 
				{
					String expectedfrom="From: =?iso-2022-jp?Q?=93=FA=96{=8C=EA?= <"+japanesefromaddress.Email+">";

					if (line.IndexOf(expectedfrom)== 0) 
					{
						fromHeaderEncoded=true;
					}
				}
				if (line.IndexOf("Subject: =?iso-2022-jp?Q?=1B$BF|K\\8l=1B(B=20-=20Quoted=20Printable?=")==0)
				{
					subjectHeaderEncoded=true;
				}
				if (line.IndexOf("<html><body>=1B$BEl5~!\"F|K\\8l=1B(B</body></html>")==0)
				{
					//<html><body>=67=71=4E=AC=30=01=65=E5=67=2C=8A=9E</body></html>

					htmlEncoded=true;
				}
				if (line.IndexOf("=1B$BEl5~!\"F|K\\8l=1B(B")==0)
				{
					textEncoded=true;
				}
				if (line.IndexOf("Content-Type: text/plain")==0)
				{
					hasPlainText=true;
				}
				if (line.IndexOf("Content-Type: text/html")==0)
				{
					hasHtmlText=true;
				}
				if (line.IndexOf("X-Mailer: DotNetOpenMail " + VersionInfo.GetInstance().ToString())==0)
				{
					//hasDefaultXMailerHeader=true;
				}
				if (line.IndexOf("X-MyHeader1: my header number one") == 0)
				{
					//hasCustomHeader1=true;
				}
				if (line.IndexOf("X-MyHeader2: my header number two") == 0)
				{
					//hasCustomHeader2=true;
				}

				// log.Debug("Line +ACIAKw-i+-": +ACIAKw-line);				
			}
			Assert.IsTrue(toHeaderEncoded, "To Header not encoded");
			Assert.IsTrue(fromHeaderEncoded, "From Header not encoded");
			Assert.IsTrue(subjectHeaderEncoded, "Subject Header not encoded");
			//Assert.IsTrue(hasSubjectHeader, "Missing Subject Header");
			//Assert.IsTrue(hasMimeVersion, "Missing Mime Version header");
			//Assert.IsTrue(hasMultipartAlternative, "Missing Mime Multipart/Alternative setting");
			Assert.IsTrue(hasPlainText, "Missing Plain Text");
			Assert.IsTrue(hasHtmlText, "Missing HTML");
			Assert.IsTrue(htmlEncoded, "HTML Not encoded");
			Assert.IsTrue(textEncoded, "Text Not encoded");
			//Assert.IsTrue(hasDefaultXMailerHeader, "Missing X-Mailer Header");
			//Assert.IsTrue(hasCustomHeader1, "Missing Custom Header 1");
			//Assert.IsTrue(hasCustomHeader2, "Missing Custom Header 2");
			//Assert.AreEqual(2, quotedPrintableParts, "Expected 2 quoted printable declarations, found +ACIAKw-quotedPrintableParts);
		
			


		}
		#endregion

		#region TestJapaneseHtmlAndTextB64Encoding
		[Test]
		public void TestJapaneseHtmlAndTextB64Encoding() 
		{
			//SmtpServer smtpserver=TestAddressHelper.GetSmtpServer();
			System.Text.Encoding encoding=System.Text.Encoding.GetEncoding("Shift_JIS");

			EmailAddress japanesefromaddress=new EmailAddress(TestAddressHelper.GetFromAddress().Email, "日本語", EncodingType.Base64, encoding);
			EmailAddress japanesetoaddress=new EmailAddress(TestAddressHelper.GetToAddress().Email, "日本語", EncodingType.Base64, encoding);
			EmailMessage emailmessage=new EmailMessage();

			emailmessage.HeaderEncoding=DotNetOpenMail.Encoding.EncodingType.Base64;
			emailmessage.HeaderCharSet=System.Text.Encoding.GetEncoding("Shift_JIS");

			emailmessage.FromAddress=japanesefromaddress;
			emailmessage.AddToAddress(japanesetoaddress);
			emailmessage.Subject="日本語 - Base 64";
			emailmessage.TextPart=new TextAttachment("東京、日本語");
			
			emailmessage.TextPart.CharSet=System.Text.Encoding.GetEncoding("Shift_JIS");
			emailmessage.TextPart.Encoding=DotNetOpenMail.Encoding.EncodingType.Base64;
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>東京、日本語</body></html>");
			
			emailmessage.HtmlPart.CharSet=System.Text.Encoding.GetEncoding("Shift_JIS");
			emailmessage.HtmlPart.Encoding=DotNetOpenMail.Encoding.EncodingType.Base64;

			emailmessage.Send(_smtpserver);

			String content=emailmessage.ToDataString();
			StringReader sr=new StringReader(content);
			log.Debug(content);

			int i=0;
			String line=null;


			bool toHeaderEncoded=false;
			bool fromHeaderEncoded=false;
			bool subjectHeaderEncoded=false;
			//bool htmlEncoded=false;
			//bool textEncoded=false;
			bool hasPlainText=false;
			bool hasHtmlText=false;

			
			
			while ((line=sr.ReadLine())!=null) 
			{
				i++;
				//log.Debug("FOUND +ACIAKw-line);
				if (line.IndexOf("To: =?iso-2022-jp?B?k/qWe4zq?= <"+japanesetoaddress.Email+">")==0)
				{					
					toHeaderEncoded=true;
				}
				if (line.IndexOf("From") == 0) 
				{
					String expectedfrom="From: =?iso-2022-jp?B?k/qWe4zq?= <"+japanesefromaddress.Email+">";
					if (line.IndexOf(expectedfrom)== 0) 
					{
						fromHeaderEncoded=true;
					}
				}
				if (line.IndexOf("Subject: =?iso-2022-jp?B?k/qWe4zqIC0gQmFzZSA2NA==?=")==0)
				{
					subjectHeaderEncoded=true;
				}
				if (line.IndexOf("Content-Type: multipart/alternative")==0)
				{
					//htmlEncoded=true;
				}
				if (line.IndexOf("Content-Type: text/html")==0)
				{
					hasHtmlText=true;
				}
				if (line.IndexOf("Content-Type: text/plain")==0)
				{
					hasPlainText=true;
				}
				if (line.IndexOf("X-Mailer: DotNetOpenMail " + VersionInfo.GetInstance().ToString())==0)
				{
					//hasDefaultXMailerHeader=true;
				}
				if (line.IndexOf("X-MyHeader1: my header number one") == 0)
				{
					//hasCustomHeader1=true;
				}
				if (line.IndexOf("X-MyHeader2: my header number two") == 0)
				{
					//hasCustomHeader2=true;
				}

				// log.Debug("Line +ACIAKw-i+-": +ACIAKw-line);				
			}
			Assert.IsTrue(toHeaderEncoded, "To Header not encoded");
			Assert.IsTrue(fromHeaderEncoded, "From Header not encoded");
			Assert.IsTrue(subjectHeaderEncoded, "Subject Header not encoded");
			//Assert.IsTrue(hasSubjectHeader, "Missing Subject Header");
			//Assert.IsTrue(hasMimeVersion, "Missing Mime Version header");
			//Assert.IsTrue(hasMultipartAlternative, "Missing Mime Multipart/Alternative setting");
			Assert.IsTrue(hasPlainText, "Missing Plain Text");
			Assert.IsTrue(hasHtmlText, "Missing HTML");
			//Assert.IsTrue(htmlEncoded, "HTML Not encoded");
			//Assert.IsTrue(textEncoded, "Text Not encoded");
			//Assert.IsTrue(hasDefaultXMailerHeader, "Missing X-Mailer Header");
			//Assert.IsTrue(hasCustomHeader1, "Missing Custom Header 1");
			//Assert.IsTrue(hasCustomHeader2, "Missing Custom Header 2");
			//Assert.AreEqual(2, quotedPrintableParts, "Expected 2 quoted printable declarations, found +ACIAKw-quotedPrintableParts);
		
			


		}
		#endregion

	
	}
}
