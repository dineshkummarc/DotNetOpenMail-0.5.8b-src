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
using System.Globalization;

using DotNetOpenMail;
using log4net;

using NUnit.Framework;

namespace DotNetOpenMailTests 
{

	[TestFixture]
	public class RFC2822DateTests 
	{

		private static readonly ILog log = LogManager.GetLogger(typeof(RFC2822DateTests));

		public RFC2822DateTests() 
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
		public void TestSMTPDate() 
		{
			TimeZone timezone=TimeZone.CurrentTimeZone;

			DateTime datetime=new DateTime(2005, 2, 8, 9, 34,56);
			String tzhours=String.Format("{0:00}", timezone.GetUtcOffset(datetime).Hours);
			String tzminutes=String.Format("{0:00}", timezone.GetUtcOffset(datetime).Minutes);
			String tzstring=tzhours+tzminutes;
			if (timezone.GetUtcOffset(datetime).Hours >= 0) 
			{
				tzstring="+"+tzstring;
			}
			RFC2822Date rfcdate=new RFC2822Date(datetime, timezone);
			Assert.AreEqual("Tue, 8 Feb 2005 09:34:56 "+tzstring, rfcdate.ToString());

			datetime=new DateTime(2005, 2, 8, 19, 34,56);
			rfcdate=new RFC2822Date(datetime, timezone);
			Assert.AreEqual("Tue, 8 Feb 2005 19:34:56 "+tzstring, rfcdate.ToString());

		}

		[Test]
		public void TestNonEnglishSMTPDate() 
		{
			CultureInfo japanese=new CultureInfo("ja-JP");
			System.Threading.Thread.CurrentThread.CurrentCulture=japanese;

			TimeZone timezone=TimeZone.CurrentTimeZone;

			DateTime datetime=new DateTime(2005, 2, 8, 9, 34,56);
			String tzhours=String.Format("{0:00}", timezone.GetUtcOffset(datetime).Hours);
			String tzminutes=String.Format("{0:00}", timezone.GetUtcOffset(datetime).Minutes);
			String tzstring=tzhours+tzminutes;
			if (timezone.GetUtcOffset(datetime).Hours >= 0) 
			{
				tzstring="+"+tzstring;
			}
			RFC2822Date rfcdate=new RFC2822Date(datetime, timezone);
			Assert.AreEqual("Tue, 8 Feb 2005 09:34:56 "+tzstring, rfcdate.ToString());

			datetime=new DateTime(2005, 2, 8, 19, 34,56);
			rfcdate=new RFC2822Date(datetime, timezone);
			Assert.AreEqual("Tue, 8 Feb 2005 19:34:56 "+tzstring, rfcdate.ToString());

		}

	
	}
}
