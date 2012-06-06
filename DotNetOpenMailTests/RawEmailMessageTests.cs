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

using NUnit.Framework;
using log4net;

namespace DotNetOpenMailTests
{
	/// <summary>
	/// Summary description for RawEmailMessageTests.
	/// </summary>
	[TestFixture]
	public class RawEmailMessageTests
	{

		SmtpServer _smtpserver=null;
		private static readonly ILog log = LogManager.GetLogger(typeof(RawEmailMessageTests));

		[SetUp]
		public void SetUp() 
		{
			_smtpserver=TestAddressHelper.GetSmtpServer();
		}

		[TearDown]
		public void TearDown() {}

		public RawEmailMessageTests()
		{
		}

		[Test]
		public void TestRawEmail() 
		{			
			FileInfo contentfile=new FileInfo(@"..\..\TestFiles\ImportedEmail.txt");
			Assert.IsTrue(contentfile.Exists);
			StreamReader sr=new StreamReader(contentfile.OpenRead());
			
			RawEmailMessage message=new RawEmailMessage();
			message.Content=sr.ReadToEnd();
			message.AddRcptToAddress(TestAddressHelper.GetToAddress());
			message.MailFrom=TestAddressHelper.GetFromAddress();
			message.Send(_smtpserver);

		}

	}
}
