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
	public class FileAttachmentTests2 
	{

		private static readonly ILog log = LogManager.GetLogger(typeof(FileAttachmentTests2));

		public FileAttachmentTests2() 
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

		#region TestVariousAttachments
		[Test]
		public void TestVariousAttachments() 
		{
			// Read from the file
			FileAttachment fromfile=GetAttachmentFromFile(new FileInfo(@"..\..\TestFiles\acrobattestpage.pdf"));
			Assert.IsNotNull(fromfile);

			// Create from a stream reader (this won't work with binary attachments)
			FileAttachment fromstreamreader=GetAttachmentFromStreamReader(new FileInfo(@"..\..\TestFiles\acrobattestpage.pdf"));
			Assert.IsNotNull(fromstreamreader);
			
			// Create from byte array
			FileAttachment frombytearray=GetAttachmentFromByteArray(new FileInfo(@"..\..\TestFiles\acrobattestpage.pdf"));
			Assert.IsNotNull(frombytearray);

			// Create from a stream reader (this won't work with binary attachments)
			FileAttachment frombinaryreader=GetAttachmentFromBinaryReader(new FileInfo(@"..\..\TestFiles\acrobattestpage.pdf"));
			Assert.IsNotNull(frombinaryreader);

			String fromFileStr=fromfile.ToDataString();
			String fromByteArrayStr=frombytearray.ToDataString();
			String fromStreamReaderStr=fromstreamreader.ToDataString();
			String fromBinaryReaderStr=frombinaryreader.ToDataString();

			log.Debug("FROM FILE: "+fromFileStr.Length+" bytes");
			log.Debug("FROM BYTE ARRAY: "+fromByteArrayStr.Length+" bytes");
			log.Debug("FROM STREAM READER: "+fromStreamReaderStr.Length+" bytes");
			log.Debug("FROM BINARY READER: "+fromBinaryReaderStr.Length+" bytes");

			Assert.AreEqual(fromFileStr, fromBinaryReaderStr);

		}
		#endregion

		#region GetBasicMessage
		private EmailMessage GetBasicMessage() 
		{
			EmailMessage emailmessage=new EmailMessage();
	
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
	
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());	
	
			emailmessage.Subject="PDF Test";
	
			emailmessage.TextPart=new TextAttachment("This a PDF File");
		
			emailmessage.HtmlPart=new HtmlAttachment("<p>This is a PDF File</p>\r\n");

			return emailmessage;

		}
		#endregion

		#region GetAttachmentFromByteArray
		private FileAttachment GetAttachmentFromByteArray(FileInfo fileinfo) 
		{
			FileStream filestream = fileinfo.OpenRead();

			BinaryReader br=new BinaryReader(filestream);
			byte[] bytes=br.ReadBytes((int) fileinfo.Length);
			br.Close();

			FileAttachment fileAttachment=new FileAttachment(bytes);
			fileAttachment.Encoding=DotNetOpenMail.Encoding.EncodingType.Base64;
			fileAttachment.FileName="acrobattestpage.pdf";
			fileAttachment.ContentType="application/pdf";
			return fileAttachment;

		}
		#endregion

		#region GetAttachmentFromFile
		private FileAttachment GetAttachmentFromFile(FileInfo fileinfo) 
		{
			FileStream filestream = fileinfo.OpenRead();

			FileAttachment fileAttachment=new FileAttachment(fileinfo);
			fileAttachment.Encoding=DotNetOpenMail.Encoding.EncodingType.Base64;
			fileAttachment.FileName="acrobattestpage.pdf";
			fileAttachment.ContentType="application/pdf";
			return fileAttachment;

		}
		#endregion

		#region GetAttachmentFromStreamReader
		private FileAttachment GetAttachmentFromStreamReader(FileInfo fileinfo) 
		{
			FileStream filestream = fileinfo.OpenRead();
	
			StreamReader streamreader=new StreamReader(filestream);
			

			FileAttachment fileAttachment=new FileAttachment(streamreader);
			
			//fileAttachment.ContentType
			fileAttachment.Encoding=DotNetOpenMail.Encoding.EncodingType.Base64;
			fileAttachment.FileName="acrobattestpage.pdf";
			fileAttachment.ContentType="application/pdf";
			return fileAttachment;
		}
		#endregion

		#region GetAttachmentFromBinaryReader
		private FileAttachment GetAttachmentFromBinaryReader(FileInfo fileinfo) 
		{
			FileStream filestream = fileinfo.OpenRead();
	
			BinaryReader binaryreader=new BinaryReader(filestream);
			

			FileAttachment fileAttachment=new FileAttachment(binaryreader);
			
			//fileAttachment.ContentType
			fileAttachment.Encoding=DotNetOpenMail.Encoding.EncodingType.Base64;
			fileAttachment.FileName="acrobattestpage.pdf";
			fileAttachment.ContentType="application/pdf";
			return fileAttachment;
		}
		#endregion


	}
}
