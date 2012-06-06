using System;

using DotNetOpenMail;
using DotNetOpenMail.Utils;

using log4net;

using NUnit.Framework;

namespace DotNetOpenMailTests.Utils 
{

	[TestFixture]
	public class EmailAddressParserTests 
	{

		private static readonly ILog log = LogManager.GetLogger(typeof(EmailAddressParserTests));

		public EmailAddressParserTests() 
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
		public void TestParser() 
		{
			EmailAddressParser emailaddressparser=new EmailAddressParser();
			String name1="Test Tester";
			String email1="test@domain.com";

			EmailAddress emailaddress=emailaddressparser.ParseRawEmailAddress(String.Format("{0} <{1}>", name1, email1));
			Assert.IsNull(emailaddressparser.LastError);
			Assert.IsNotNull(emailaddress, emailaddressparser.LastError);
			Assert.AreEqual(name1, emailaddress.Name);
			Assert.AreEqual(email1, emailaddress.Email);

			emailaddress=emailaddressparser.ParseRawEmailAddress(email1);
			Assert.IsNull(emailaddressparser.LastError);
			Assert.IsNotNull(emailaddress, emailaddressparser.LastError);
			Assert.AreEqual("", emailaddress.Name);
			Assert.AreEqual(email1, emailaddress.Email);

			emailaddress=emailaddressparser.ParseRawEmailAddress(String.Format("\"{0}\" <{1}>", name1, email1));
			Assert.IsNotNull(emailaddress, emailaddressparser.LastError);
			Assert.IsNull(emailaddressparser.LastError, emailaddressparser.LastError);
			Assert.AreEqual("\""+name1+"\"", emailaddress.Name);
			Assert.AreEqual(email1, emailaddress.Email);

			emailaddress=emailaddressparser.ParseRawEmailAddress(String.Format("{0} <{1}>", name1, "test@localhost"));
			Assert.IsNotNull(emailaddress, emailaddressparser.LastError);
			Assert.IsNull(emailaddressparser.LastError, emailaddressparser.LastError);
			Assert.AreEqual(name1, emailaddress.Name);
			Assert.AreEqual("test@localhost", emailaddress.Email);


		}

	
	}
}
