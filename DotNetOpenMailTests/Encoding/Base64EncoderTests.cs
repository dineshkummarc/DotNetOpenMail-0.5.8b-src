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

using DotNetOpenMail.Encoding;

using log4net;

using NUnit.Framework;

namespace DotNetOpenMailTests.Encoding 
{

	[TestFixture]
	public class Base64EncoderTests 
	{

		private static readonly ILog log = LogManager.GetLogger(typeof(Base64EncoderTests));

		public Base64EncoderTests() 
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
		public void SimpleTest() 
		{
			String setofchars=""+(char)0x01 +(char)0x02+(char)0x03;
			Base64Encoder base64encoder=Base64Encoder.GetInstance();
			StringReader sr=new StringReader(setofchars);
			StringBuilder sb=new StringBuilder();
			StringWriter sw=new StringWriter(sb);

			base64encoder.Encode(sr, sw, System.Text.Encoding.GetEncoding("iso-8859-1"));
			log.Debug(sb.ToString());
			Assert.AreEqual("AQID", sb.ToString());
		}

		[Test]
		public void LineLengthTest() 
		{
			String line="12345678901234567890123456789012345678901234567890123456789012345678901234567890";
			String expectedresult="MTIzNDU2Nzg5MDEyMzQ1Njc4OTAxMjM0NTY3ODkwMTIzNDU2Nzg5MDEyMzQ1Njc4OTAxMjM0NTY3\r\nODkwMTIzNDU2Nzg5MDEyMzQ1Njc4OTA=";
			Base64Encoder base64=Base64Encoder.GetInstance();
			StringReader sr=new StringReader(line);
			StringBuilder sb=new StringBuilder();
			StringWriter sw=new StringWriter(sb);
			base64.Encode(sr, sw, System.Text.Encoding.GetEncoding("iso-8859-1"));
			log.Debug(sb.ToString());
			Assert.AreEqual(expectedresult, sb.ToString());

			Assert.AreEqual(expectedresult, base64.EncodeString(line, System.Text.Encoding.GetEncoding("iso-8859-1")));
		}
	
		[Test]
		public void HeaderEncodingTest() 
		{
			Base64Encoder base64=Base64Encoder.GetInstance();
			String result=base64.EncodeHeaderString("Test", "helloøæô", System.Text.Encoding.GetEncoding("iso-8859-1"), false);
			log.Debug("RESULT IS "+result);
			Assert.AreEqual("=?iso-8859-1?B?aGVsbG/45vQ=?=", result);

		}

	}
}
