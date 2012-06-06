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
using System.Text;
using System.IO;

using DotNetOpenMail; 
using DotNetOpenMail.Encoding; 

using NUnit.Framework;
using log4net;

namespace DotNetOpenMailTests.Encoding {

	[TestFixture]
	public class SevenBitEncoderTests {

		private static readonly ILog log = LogManager.GetLogger(typeof(SevenBitEncoderTests));

		public SevenBitEncoderTests() {
		}

		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void SimpleTest() 
		{
			// this tests that the 8bit encoder does nothing
			String setofchars=Get256Chars();
			SevenBitEncoder sevenbitencoder=SevenBitEncoder.GetInstance();
			StringReader sr=new StringReader(setofchars);
			StringBuilder sb=new StringBuilder();
			StringWriter sw=new StringWriter(sb);

			sevenbitencoder.Encode(sr, sw, System.Text.Encoding.GetEncoding("iso-8859-1"));
			Assert.AreEqual(setofchars, sb.ToString());
		}

		private String Get256Chars() 
		{
			StringBuilder sb=new StringBuilder();
			for (int i=0; i<256; i++) 
			{
				sb.Append((char) i);
			}
			return sb.ToString();
		}

		#region TestFor7BitHeader
		[Test]
		public void TestFor7BitHeader() 
		{
			EmailMessage emailmessage=new EmailMessage();
			emailmessage.HeaderCharSet=System.Text.ASCIIEncoding.ASCII;
			emailmessage.HeaderEncoding=DotNetOpenMail.Encoding.EncodingType.SevenBit;

			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());
			emailmessage.Subject="EmailMessageTests Test HTML and Text";
			emailmessage.TextPart=new TextAttachment("This\r\nis the\r\ntext\r\npart.");
			emailmessage.TextPart.CharSet=System.Text.ASCIIEncoding.ASCII;
			emailmessage.TextPart.Encoding=DotNetOpenMail.Encoding.EncodingType.SevenBit;

			emailmessage.HtmlPart=new HtmlAttachment("<html><body>This<br>\r\nis the<br>\r\n<strong>HTML</strong><br>\r\npart.</body></html>");
			emailmessage.HtmlPart.CharSet=System.Text.ASCIIEncoding.ASCII;
			emailmessage.HtmlPart.Encoding=DotNetOpenMail.Encoding.EncodingType.SevenBit;

			//emailmessage.Send(_smtpserver);

			String content=emailmessage.ToDataString();
			StringReader sr=new StringReader(content);
			//log.Debug(content);

			int i=0;
			String line=null;

			String expectedToAddress=TestAddressHelper.GetToAddress().Name+" <"+TestAddressHelper.GetToAddress().Email+">";
			int has7Bit=0;
			int hasUSASCII=0;

			while ((line=sr.ReadLine())!=null) 
			{
				i++;
				if (line.IndexOf("Content-Transfer-Encoding: 7bit")==0) 
				{
					has7Bit++;
				}
				if (line.IndexOf("charset=\"us-ascii\"")>0) 
				{
					hasUSASCII++;
				}
				//log.Debug("Line "+i+": "+line);				
			}
			Assert.AreEqual(2, has7Bit, "Not enough 7bit lines");
			Assert.AreEqual(2, hasUSASCII, "Not enough us-ascii lines");

		}
		#endregion


	}
}
