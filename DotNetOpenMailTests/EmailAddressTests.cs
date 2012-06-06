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

using DotNetOpenMail;

using NUnit.Framework;
using log4net;

namespace DotNetOpenMailTests {

	[TestFixture]
	public class EmailAddressTests {

		private static readonly ILog log = LogManager.GetLogger(typeof(EmailAddressTests));

		public EmailAddressTests() {
		}

		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void TestSerialization() 
		{
			EmailAddress emailaddress=new EmailAddress("test@test.com", "My Name");
			Assert.AreEqual("My Name <test@test.com>",emailaddress.ToString());

			emailaddress=new EmailAddress("test@test.com", "Last, First");
			Assert.AreEqual("\"Last, First\" <test@test.com>",emailaddress.ToString());

			emailaddress=new EmailAddress("test@test.com", "<First Last>");
			Assert.AreEqual("\"<First Last>\" <test@test.com>",emailaddress.ToString());

			emailaddress=new EmailAddress("test@test.com", "[First Last]");
			Assert.AreEqual("\"[First Last]\" <test@test.com>",emailaddress.ToString());

			emailaddress=new EmailAddress("test@test.com", "(First Last)");
			Assert.AreEqual("\"(First Last)\" <test@test.com>",emailaddress.ToString());
			emailaddress=new EmailAddress("test@test.com", "First \"NickName\" Last");
			Assert.AreEqual("\"First \\\"NickName\\\" Last\" <test@test.com>",emailaddress.ToString());


		}
	}
}
