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
	public class FileAttachmentTests 
	{

		private static readonly ILog log = LogManager.GetLogger(typeof(FileAttachmentTests));

		public FileAttachmentTests() 
		{
		}

		[SetUp]
		public void SetUp() 
		{
		}

		[TearDown]
		public void TearDown() 
		{
		}

		
		[Test]
		public void TestFileFromStream() 
		{
			EmailMessage emailmessage=new EmailMessage();
	
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
	
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());	
	
			emailmessage.Subject="Here's your license";
	
			emailmessage.TextPart=new TextAttachment("This is a license.\r\n\r\n"+
				"We will spend your money on a new plasma TV.");
		
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>"+
				"<p>This is a license.</p>\r\n"+
				"<p>We will spend your money on a new <i>plasma TV</i>.</p>\r\n"+
				"</body></html>");
		
			MemoryStream stream=new MemoryStream();
			StreamWriter sw=new StreamWriter(stream);
			sw.WriteLine("this is some test data 1");
			sw.WriteLine("this is some test data 2");
			sw.WriteLine("this is some test data 3");
			sw.WriteLine("this is some test data 4");
			sw.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			//BinaryReader br=new BinaryReader(stream);

			FileAttachment fileAttachment=new FileAttachment(new StreamReader(stream));
			
			//fileAttachment.ContentType
			fileAttachment.FileName="License.txt";
			fileAttachment.CharSet=System.Text.Encoding.ASCII;
			fileAttachment.ContentType="text/plain";
			emailmessage.AddMixedAttachment(fileAttachment);
			
			//emailmessage.Send(TestAddressHelper.GetSmtpServer());		
		}

		[Test]
		public void TestBinaryFileFromStream() 
		{
			EmailMessage emailmessage=new EmailMessage();
	
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
	
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());	
	
			emailmessage.Subject="Here's your file";
	
			emailmessage.TextPart=new TextAttachment("This a jpeg.");
		
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>"+
				"<p>This a jpeg.</p>\r\n");

			FileInfo fileinfo=new FileInfo(@"..\..\TestFiles\grover.jpg");		
			//FileInfo fileinfo=new FileInfo(@"..\..\TestFiles\casingTheJoint.jpg");		
			FileStream filestream = fileinfo.OpenRead();

			MemoryStream stream=new MemoryStream();

			StreamWriter sw=new StreamWriter(stream);

			sw.Flush();

			//BinaryReader br=new BinaryReader(stream);

			BinaryReader br=new BinaryReader(filestream);
			byte[] bytes=br.ReadBytes((int) fileinfo.Length);
			br.Close();

			FileAttachment fileAttachment=new FileAttachment(bytes);
			
			//fileAttachment.ContentType
			fileAttachment.FileName="grover.jpg";
			fileAttachment.ContentType="image/jpeg";
			emailmessage.AddMixedAttachment(fileAttachment);
			
			emailmessage.Send(TestAddressHelper.GetSmtpServer());		
		}

		
		[Test]
		public void TestFileFromString() 
		{
			EmailMessage emailmessage=new EmailMessage();
	
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
	
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());	
	
			emailmessage.Subject="Here's your license";
	
			emailmessage.TextPart=new TextAttachment("This is a license.\r\n\r\n"+
				"We will spend your money on a new plasma TV.");
		
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>"+
				"<p>This is a license.</p>\r\n"+
				"<p>We will spend your money on a new <i>plasma TV</i>.</p>\r\n"+
				"</body></html>");
		
			String content="This is String Line 1\r\nThis is String Line 2";

			FileAttachment fileAttachment=new FileAttachment(content);
			
			//fileAttachment.ContentType
			fileAttachment.FileName="License.txt";
			fileAttachment.CharSet=System.Text.Encoding.ASCII;
			fileAttachment.ContentType="text/plain";
			fileAttachment.Encoding=DotNetOpenMail.Encoding.EncodingType.SevenBit;
			emailmessage.AddMixedAttachment(fileAttachment);
			
			//emailmessage.Send(TestAddressHelper.GetSmtpServer());		
		}

		[Test]
		public void TestLargerBinaryFileFromStream() 
		{

			String filename="casingTheJoint.jpg";

			EmailMessage emailmessage=new EmailMessage();
	
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
	
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());	
	
			emailmessage.Subject="Here's your file";
	
			emailmessage.TextPart=new TextAttachment("This a zip file.");
		
			emailmessage.HtmlPart=new HtmlAttachment("<html><body>"+
				"<p>This a zip file.</p>\r\n");

			FileInfo fileinfo=new FileInfo(@"..\..\TestFiles\"+filename);		
			FileStream filestream = fileinfo.OpenRead();

			MemoryStream stream=new MemoryStream();

			StreamWriter sw=new StreamWriter(stream);

			sw.Flush();

			//BinaryReader br=new BinaryReader(stream);

			BinaryReader br=new BinaryReader(filestream);
			byte[] bytes=br.ReadBytes((int) fileinfo.Length);
			br.Close();

			FileAttachment fileAttachment=new FileAttachment(bytes);
			
			//fileAttachment.ContentType
			fileAttachment.FileName=filename;
			fileAttachment.ContentType="application/zip";
			emailmessage.AddMixedAttachment(fileAttachment);
			
			emailmessage.Send(TestAddressHelper.GetSmtpServer());		
		}

	

		[Test]
		public void TestDocFile()
		{
			EmailMessage mail = new EmailMessage();

			FileInfo fileinfo=new FileInfo(@"..\..\TestFiles\TestWord.doc");		
			Assert.IsTrue(fileinfo.Exists);

			FileAttachment fileAttachment = new FileAttachment(fileinfo);
			fileAttachment.ContentType = "application/msword";
			
			//EmailAddress emailAddress = new EmailAddress(emailAddressParam); 
			mail.TextPart=new TextAttachment("Here is your file");
			mail.AddMixedAttachment(fileAttachment);

			mail.FromAddress=TestAddressHelper.GetFromAddress();
			mail.AddToAddress("outlook@mymailout.com");	

			SmtpServer smtpServer = TestAddressHelper.GetSmtpServer();

			mail.Send(smtpServer);
		}
	}
}
