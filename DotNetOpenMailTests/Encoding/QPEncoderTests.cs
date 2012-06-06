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

using NUnit.Framework;
using log4net;

namespace DotNetOpenMailTests.Encoding {

	[TestFixture]
	public class QPEncoderTests {

		private static readonly ILog log = LogManager.GetLogger(typeof(QPEncoderTests));

		public QPEncoderTests() {
		}

		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void SimpleTest() {
			//String setofchars=Get256Chars();
			String setofchars=""+(char)0x01 +(char)0x02;
			QPEncoder qpencoder=QPEncoder.GetInstance();
			StringReader sr=new StringReader(setofchars);
			StringBuilder sb=new StringBuilder();
			StringWriter sw=new StringWriter(sb);

			qpencoder.Encode(sr, sw, System.Text.Encoding.GetEncoding("iso-8859-1"));
			Assert.AreEqual("=01=02", sb.ToString());
		}

		[Test]
		public void LineLengthTest() 
		{
			String line="12345678901234567890123456789012345678901234567890123456789012345678901234567890";
			String expectedresult="1234567890123456789012345678901234567890123456789012345678901234567890123456=\r\n7890";

			QPEncoder qpencoder=QPEncoder.GetInstance();
			StringReader sr=new StringReader(line);
			StringBuilder sb=new StringBuilder();
			StringWriter sw=new StringWriter(sb);
			qpencoder.Encode(sr, sw, System.Text.Encoding.GetEncoding("iso-8859-1"));
			Assert.AreEqual(expectedresult, sb.ToString());

			Assert.AreEqual(expectedresult, qpencoder.EncodeString(line, System.Text.Encoding.GetEncoding("iso-8859-1")));
		}

		[Test]
		public void EndOfLineSpacesTest() 
		{
			QPEncoder qpencoder=QPEncoder.GetInstance();
			
			Assert.AreEqual("123 456  7890=20=20=20=20=20",qpencoder.EncodeString("123 456  7890     ", System.Text.Encoding.GetEncoding("iso-8859-1")));
			Assert.AreEqual("=20=20=20=20=20",qpencoder.EncodeString("     ", System.Text.Encoding.GetEncoding("iso-8859-1")));
			Assert.AreEqual("123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456=\r\n7890=20=20=20",qpencoder.EncodeString("123456789 123456789 123456789 123456789 123456789 123456789 123456789 1234567890   ", System.Text.Encoding.GetEncoding("iso-8859-1")));
			Assert.AreEqual("123456789 123456789 123456789 123456789 123456789 123456789 123456789=\r\n=20=20=20=20=20=20=20=20=20=20=20",qpencoder.EncodeString("123456789 123456789 123456789 123456789 123456789 123456789 123456789           ", System.Text.Encoding.GetEncoding("iso-8859-1")));
			Assert.AreEqual("123456789 123456789 123456789 123456789 123456789 123456789 123456789=\r\n=09=09=09=09=09=09=09=09=09=09=09",qpencoder.EncodeString("123456789 123456789 123456789 123456789 123456789 123456789 123456789											", System.Text.Encoding.GetEncoding("iso-8859-1")));
		}

		[Test]
		public void UnixAndMacCRLF() 
		{
			QPEncoder qpencoder=QPEncoder.GetInstance();
			
			Assert.AreEqual("123\r\n456\r\n789",qpencoder.EncodeString("123\r456\r789\r", System.Text.Encoding.GetEncoding("iso-8859-1")));
			Assert.AreEqual("123\r\n456\r\n789",qpencoder.EncodeString("123\n456\n789\n", System.Text.Encoding.GetEncoding("iso-8859-1")));
		}

		[Test]
		public void Test256Chars() 
		{
			QPEncoder qpencoder=QPEncoder.GetInstance();
			
			String expected="=00=01=02=03=04=05=06=07=08=09\r\n"+
				"=0B=0C\r\n"+
				"=0E=0F=10=11=12=13=14=15=16=17=18=19=1A=1B=1C=1D=1E=1F !\"#$%&'()*+,-./012345=\r\n"+
				"6789:;<=3D>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~=\r\n"+
				"=7F=80=81=82=83=84=85=86=87=88=89=8A=8B=8C=8D=8E=8F=90=91=92=93=94=95=96=97=98=\r\n"+
				"=99=9A=9B=9C=9D=9E=9F=A0=A1=A2=A3=A4=A5=A6=A7=A8=A9=AA=AB=AC=AD=AE=AF=B0=B1=B2=\r\n"+
				"=B3=B4=B5=B6=B7=B8=B9=BA=BB=BC=BD=BE=BF=C0=C1=C2=C3=C4=C5=C6=C7=C8=C9=CA=CB=CC=\r\n"+
				"=CD=CE=CF=D0=D1=D2=D3=D4=D5=D6=D7=D8=D9=DA=DB=DC=DD=DE=DF=E0=E1=E2=E3=E4=E5=E6=\r\n"+
				"=E7=E8=E9=EA=EB=EC=ED=EE=EF=F0=F1=F2=F3=F4=F5=F6=F7=F8=F9=FA=FB=FC=FD=FE=FF";

			Assert.AreEqual(256,Get256Chars().Length);
			Assert.AreEqual(expected, qpencoder.EncodeString(Get256Chars(), System.Text.Encoding.GetEncoding("iso-8859-1")));

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

		[Test]
		public void HeaderEncodingTest() 
		{
			QPEncoder qpencoder=QPEncoder.GetInstance();

			Assert.AreEqual("=?iso-8859-1?Q?hello=F8=E6=F4?=", qpencoder.EncodeHeaderString("Test", "helloøæô", System.Text.Encoding.GetEncoding("iso-8859-1"), false));
			Assert.AreEqual("=?iso-8859-1?Q?hello=20there=20=F8=E6=F4?=", qpencoder.EncodeHeaderString("Test", "hello there øæô", System.Text.Encoding.GetEncoding("iso-8859-1"), false));			
			Assert.AreEqual("hello there", qpencoder.EncodeHeaderString("Test", "hello there", System.Text.Encoding.GetEncoding("iso-8859-1"), false));			
			Assert.AreEqual("=?iso-8859-1?Q?hello=20there?=", qpencoder.EncodeHeaderString("Test", "hello there", System.Text.Encoding.GetEncoding("iso-8859-1"), true));
			Assert.AreEqual("=?iso-8859-1?Q?TSpecials=20Equals=20=3D=20QuestionMark=3FEnd?=", qpencoder.EncodeHeaderString("Test", "TSpecials Equals = QuestionMark?End", System.Text.Encoding.GetEncoding("iso-8859-1"), true));
			Assert.AreEqual("=?iso-8859-1?Q?en=20to,=20sn=F8r=20min=20sko,=20tre=20fire=20kom=20E?=\r\n\t=?iso-8859-1?Q?lvire,=20fem=20seks=20tegn=20en=20heks,=20sju=20=E5tte?=\r\n\t=?iso-8859-1?Q?=20ris=20til=20en=20rotte?=", qpencoder.EncodeHeaderString("Test", "en to, snør min sko, tre fire kom Elvire, fem seks tegn en heks, sju åtte ris til en rotte", 
				System.Text.Encoding.GetEncoding("iso-8859-1"), true));			

			Assert.AreEqual("=?koi8-r?Q?n=CE=C5=D3=CB=CF=CC=D8=CB=CF=20=D3=D4=D2=CF=CB=20=D0=CF?=\r\n\t=?koi8-r?Q?=20=D2=D5=D3=D3=CB=C9=3F?=", qpencoder.EncodeHeaderString("Test", "nнесколько строк по русски?", System.Text.Encoding.GetEncoding("koi8-r"), true));

		}

		/*
		[Test]
		public void KOI8_QP_Tests() 
		{
			EmailMessage msg = new EmailMessage();
			msg.FromAddress = new EmailAddress("pietrovich@talk.test","pietrovich");
			msg.ToAddresses.Add(new EmailAddress("pietrovich@talk.test","pietrovich"));
			msg.Subject = "ошибочка?";
			msg.HeaderCharSet = Encoding.GetEncoding("windows-1251");
			TextAttachment ta = new TextAttachment("\r\nнесколько строк по русски");
			ta.CharSet = Encoding.GetEncoding("windows-1251");
			msg.TextPart = ta;
			Console.WriteLine(msg.ToDataString());

		}
		*/
	}
}

