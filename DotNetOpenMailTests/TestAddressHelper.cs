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

namespace DotNetOpenMailTests
{
	/// <summary>
	/// Summary description for TestAddressHelper.
	/// </summary>
	public class TestAddressHelper
	{
		public TestAddressHelper()
		{
		}

		public static EmailAddress GetFromAddress() 
		{
			//return new EmailAddress("mike@bridgecanada.com", "Test Sender");
			
			String email=System.Configuration.ConfigurationSettings.AppSettings["dnom.test.fromaddress"];
			if (email==null) 
			{
				throw new ApplicationException("Please add the key \"dnom.test.fromaddress\" to your .config file");
			}
			return new EmailAddress(email, "NUnit Test Sender");
			
		}

		public static EmailAddress GetToAddress() 
		{
			String email=System.Configuration.ConfigurationSettings.AppSettings["dnom.test.toaddress"];
			if (email==null) 
			{
				throw new ApplicationException("Please add the key \"dnom.test.toaddress\" to your .config file");
			}
			return new EmailAddress(email, "NUnit Test Recipient");
		}

		public static SmtpServer GetSmtpServer() 
		{
			String smtpserver=System.Configuration.ConfigurationSettings.AppSettings["dnom.test.smtp.server"];
			if (smtpserver==null) 
			{
				throw new ApplicationException("Please add the key \"dnom.test.smtp.server\" to your .config file");
			}
			
			return new SmtpServer(smtpserver);
		}

	}
}
