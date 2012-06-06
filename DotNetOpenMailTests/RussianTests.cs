using System;
using System.IO;
using System.Text;
using System.Globalization;

using DotNetOpenMail; 
using DotNetOpenMail.Utils; 
using DotNetOpenMail.Encoding;


using NUnit.Framework;
using log4net;


namespace DotNetOpenMailTests 
{

	[TestFixture]
	public class RussianTests {

		private static readonly ILog log = LogManager.GetLogger(typeof(RussianTests));

		public RussianTests() {
		}

		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void TestWindows1251() {

			EmailMessage emailmessage = new EmailMessage();
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());

			emailmessage.Subject = "ошибочка?";
			emailmessage.HeaderCharSet = System.Text.Encoding.GetEncoding("windows-1251");

			TextAttachment ta = new TextAttachment("\r\nнесколько строк по русски");
			ta.CharSet = System.Text.Encoding.GetEncoding("windows-1251");
			emailmessage.TextPart = ta;
			emailmessage.Send(TestAddressHelper.GetSmtpServer());
			Assert.IsTrue(emailmessage.ToDataString().IndexOf("Subject: =?windows-1251") > 0, "Missing windows-1251 in subject");
			Assert.IsTrue(emailmessage.ToDataString().IndexOf("koi8-r") < 0);
		}

		[Test]
		public void TestKOI8R() 
		{

			EmailMessage emailmessage = new EmailMessage();
			emailmessage.FromAddress=TestAddressHelper.GetFromAddress();
			emailmessage.AddToAddress(TestAddressHelper.GetToAddress());

			emailmessage.Subject = "ошибочка?";
			emailmessage.HeaderCharSet = System.Text.Encoding.GetEncoding("koi8-r");

			TextAttachment ta = new TextAttachment("\r\nнесколько строк по русски");
			ta.CharSet = System.Text.Encoding.GetEncoding("koi8-r");
			emailmessage.TextPart = ta;
			//log.Debug("1251");
			//log.Debug(emailmessage.ToDataString());
			emailmessage.Send(TestAddressHelper.GetSmtpServer());
			Assert.IsTrue(emailmessage.ToDataString().IndexOf("Subject: =?koi8-r") > 0, "Missing koi8-r in subject");
			Assert.IsTrue(emailmessage.ToDataString().IndexOf("windows-1251") < 0);
		}

	}
}
