﻿
namespace SrkToolkit.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Web.Mvc;
    using Moq;
    using System.Web.Routing;
    using System.Web;
    using System.IO;
    using System.Diagnostics;
    using System.Threading;
    using System.Globalization;

    public class SrkHtmlExtensionsTests
    {
        public static readonly CultureInfo TestCulture1 = new CultureInfo("en-GB");

        public static HtmlHelper CreateHtmlHelper(ViewDataDictionary vd)
        {
            var mockViewContext = new Mock<ViewContext>(
                new ControllerContext(
                    new Mock<HttpContextBase>().Object,
                    new RouteData(),
                    new Mock<ControllerBase>().Object),
                new Mock<IView>().Object,
                vd,
                new TempDataDictionary(),
                new Mock<TextWriter>().Object);
            var mockViewDataContainer = new Mock<IViewDataContainer>();
            mockViewDataContainer.Setup(v => v.ViewData)
                .Returns(vd);
            return new HtmlHelper(mockViewContext.Object, mockViewDataContainer.Object);
        }

        [TestClass]
        public class SetTimezoneMethod
        {
            [TestMethod]
            public void WorksWithTzObject()
            {
                var data = new ViewDataDictionary();
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                var html = CreateHtmlHelper(data);
                SrkHtmlExtensions.SetTimezone(html, tz);

                Assert.IsNotNull(data["Timezone"]);
                Assert.AreEqual(tz, data["Timezone"]);
            }

            [TestMethod]
            public void WorksWithTzName()
            {
                var data = new ViewDataDictionary();
                var tzName = "Romance Standard Time";
                var tz = TimeZoneInfo.FindSystemTimeZoneById(tzName);
                var html = CreateHtmlHelper(data);
                SrkHtmlExtensions.SetTimezone(html, tzName);

                Assert.IsNotNull(data["Timezone"]);
                Assert.AreEqual(tz, data["Timezone"]);
            }

            [TestMethod]
            public void GetterWorks()
            {
                var data = new ViewDataDictionary();
                var tzName = "Romance Standard Time";
                var tz = TimeZoneInfo.FindSystemTimeZoneById(tzName);
                var html = CreateHtmlHelper(data);
                SrkHtmlExtensions.SetTimezone(html, tzName);
                var result = SrkHtmlExtensions.GetTimezone(html);

                Assert.IsNotNull(result);
                Assert.AreEqual(tz, result);
            }
        }

        [TestClass]
        public class GetUserDateMethod
        {
            [TestMethod]
            public void UndefinedTzIsUtc_ArgIsUtc_ResultIsUtc()
            {
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                TimeZoneInfo tz = null;
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                DateTime utcResult;
                var result = SrkHtmlExtensions.GetUserDate(html, source, out utcResult);

                Assert.AreEqual(source, result);
                Assert.AreEqual(source, utcResult);
            }

            [TestMethod]
            public void UndefinedTzIsUtc_ArgIsLocal_ResultIsUtc()
            {
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = orig.ToLocalTime();
                Assert.AreEqual(DateTimeKind.Local, source.Kind);
                TimeZoneInfo tz = null;
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                DateTime utcResult;
                var result = SrkHtmlExtensions.GetUserDate(html, source, out utcResult);

                Assert.AreEqual(orig, result);
                Assert.AreEqual(orig, utcResult);
            }

            [TestMethod]
            public void UndefinedTzIsUtc_ArgIsUnspecified_ResultIsUtc()
            {
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = TimeZoneInfo.Utc.ConvertFromUtc(orig);
                Assert.AreEqual(DateTimeKind.Unspecified, source.Kind);
                TimeZoneInfo tz = null;
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                DateTime utcResult;
                var result = SrkHtmlExtensions.GetUserDate(html, source, out utcResult);

                Assert.AreEqual(orig, result);
                Assert.AreEqual(orig, utcResult);
            }

            [TestMethod]
            public void RomanceTz_ArgIsUtc_ResultIsRomance()
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = tz.ConvertFromUtc(orig);
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                DateTime utcResult;
                var result = SrkHtmlExtensions.GetUserDate(html, source, out utcResult);

                Assert.AreEqual(source, result, "wrong user result");
                Assert.AreEqual(orig, utcResult, "wrong UTC result");
            }

            [TestMethod]
            public void RomanceTz_ArgIsLocal_ResultIsRomance()
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = orig.ToLocalTime();
                DateTime expected = tz.ConvertFromUtc(orig);
                Assert.AreEqual(DateTimeKind.Local, source.Kind);
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                DateTime utcResult;
                var result = SrkHtmlExtensions.GetUserDate(html, source, out utcResult);

                Assert.AreEqual(expected, result, "wrong user result");
                Assert.AreEqual(orig, utcResult, "wrong UTC result");
            }

            [TestMethod]
            public void RomanceTz_ArgIsUnspecified_ResultIsUtc()
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = tz.ConvertFromUtc(orig);
                Assert.AreEqual(DateTimeKind.Unspecified, source.Kind);
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                DateTime utcResult;
                var result = SrkHtmlExtensions.GetUserDate(html, source, out utcResult);

                Assert.AreEqual(source, result, "wrong user result");
                Assert.AreEqual(orig, utcResult, "wrong UTC result");
            }
        }

        [TestClass]
        public class DisplayDateMethod
        {
            [TestMethod]
            public void UserIsUtc_ArgIsUtc_ResultIsUtc()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                TimeZoneInfo tz = null;
                string expected = "<time datetime=\"2013-01-29T13:28:21.0010000Z\" title=\"29 January 2013\" class=\"past not-today display-date\">29 January 2013</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDate(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsUtc_ArgIsLocal_ResultIsUtc()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                var localTz = TimeZoneInfo.Local;
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = orig.ToLocalTime();
                Debug.Assert(source.Kind == DateTimeKind.Local);
                TimeZoneInfo tz = null;
                string expected = "<time datetime=\"2013-01-29T" + orig.Hour + ":28:21.0010000Z\" title=\"29 January 2013\" class=\"past not-today display-date\">29 January 2013</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDate(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsUtc_ArgIsUser_ResultIsUtc()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Unspecified);
                Debug.Assert(source.Kind == DateTimeKind.Unspecified);
                TimeZoneInfo tz = null;
                string expected = "<time datetime=\"2013-01-29T13:28:21.0010000Z\" title=\"29 January 2013\" class=\"past not-today display-date\">29 January 2013</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDate(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsRomance_ArgIsUtc_ResultIsRomance()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime romance = tz.ConvertFromUtc(source);
                string expected = "<time datetime=\"2013-01-29T" + source.Hour + ":28:21.0010000Z\" title=\"29 January 2013\" class=\"past not-today display-date\">29 January 2013</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDate(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsRomance_ArgIsLocal_ResultIsRomance()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                var localTz = TimeZoneInfo.Local;
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = orig.ToLocalTime();
                DateTime romance = tz.ConvertFromUtc(source);
                Debug.Assert(source.Kind == DateTimeKind.Local);
                string expected = "<time datetime=\"2013-01-29T" + orig.Hour + ":28:21.0010000Z\" title=\"29 January 2013\" class=\"past not-today display-date\">29 January 2013</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDate(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsRomance_ArgIsUser_ResultIsRomance()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Unspecified);
                DateTime utc = tz.ConvertToUtc(source);
                Debug.Assert(source.Kind == DateTimeKind.Unspecified);
                string expected = "<time datetime=\"2013-01-29T" + utc.Hour + ":28:21.0010000Z\" title=\"29 January 2013\" class=\"past not-today display-date\">29 January 2013</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDate(html, source);

                Assert.AreEqual(expected, result.ToString());
            }
        }

        [TestClass]
        public class DisplayDateTimeMethod
        {
            [TestMethod]
            public void UserIsUtc_ArgIsUtc_ResultIsUtc()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                TimeZoneInfo tz = null;
                string expected = "<time datetime=\"2013-01-29T13:28:21.0010000Z\" title=\"29/01/2013 13:28:21\" class=\"past not-today display-datetime\">29 January 2013 13:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDateTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsUtc_ArgIsLocal_ResultIsUtc()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                var localTz = TimeZoneInfo.Local;
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = orig.ToLocalTime();
                Debug.Assert(source.Kind == DateTimeKind.Local);
                TimeZoneInfo tz = null;
                string expected = "<time datetime=\"2013-01-29T" + orig.Hour + ":28:21.0010000Z\" title=\"29/01/2013 13:28:21\" class=\"past not-today display-datetime\">29 January 2013 13:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDateTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsUtc_ArgIsUser_ResultIsUtc()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Unspecified);
                Debug.Assert(source.Kind == DateTimeKind.Unspecified);
                TimeZoneInfo tz = null;
                string expected = "<time datetime=\"2013-01-29T13:28:21.0010000Z\" title=\"29/01/2013 13:28:21\" class=\"past not-today display-datetime\">29 January 2013 13:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDateTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsRomance_ArgIsUtc_ResultIsRomance()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime romance = tz.ConvertFromUtc(source);
                string expected = "<time datetime=\"2013-01-29T13:28:21.0010000Z\" title=\"29/01/2013 14:28:21\" class=\"past not-today display-datetime\">29 January 2013 14:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDateTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsRomance_ArgIsLocal_ResultIsRomance()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                var localTz = TimeZoneInfo.Local;
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = orig.ToLocalTime();
                DateTime romance = tz.ConvertFromUtc(source);
                Debug.Assert(source.Kind == DateTimeKind.Local);
                string expected = "<time datetime=\"2013-01-29T" + orig.Hour + ":28:21.0010000Z\" title=\"29/01/2013 14:28:21\" class=\"past not-today display-datetime\">29 January 2013 14:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDateTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsRomance_ArgIsUser_ResultIsRomance()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Unspecified);
                DateTime utc = tz.ConvertToUtc(source);
                Debug.Assert(source.Kind == DateTimeKind.Unspecified);
                string expected = "<time datetime=\"2013-01-29T12:28:21.0010000Z\" title=\"29/01/2013 13:28:21\" class=\"past not-today display-datetime\">29 January 2013 13:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayDateTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }
        }

        [TestClass]
        public class DisplayTimeMethod
        {
            [TestMethod]
            public void UserIsUtc_ArgIsUtc_ResultIsUtc()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                TimeZoneInfo tz = null;
                string expected = "<time datetime=\"2013-01-29T13:28:21.0010000Z\" title=\"29/01/2013 13:28:21\" class=\"past not-today display-time\">13:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsUtc_ArgIsLocal_ResultIsUtc()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                var localTz = TimeZoneInfo.Local;
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = orig.ToLocalTime();
                Debug.Assert(source.Kind == DateTimeKind.Local);
                TimeZoneInfo tz = null;
                string expected = "<time datetime=\"2013-01-29T" + orig.Hour + ":28:21.0010000Z\" title=\"29/01/2013 13:28:21\" class=\"past not-today display-time\">13:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsUtc_ArgIsUser_ResultIsUtc()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Unspecified);
                Debug.Assert(source.Kind == DateTimeKind.Unspecified);
                TimeZoneInfo tz = null;
                string expected = "<time datetime=\"2013-01-29T13:28:21.0010000Z\" title=\"29/01/2013 13:28:21\" class=\"past not-today display-time\">13:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsRomance_ArgIsUtc_ResultIsRomance()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime romance = tz.ConvertFromUtc(source);
                string expected = "<time datetime=\"2013-01-29T13:28:21.0010000Z\" title=\"29/01/2013 14:28:21\" class=\"past not-today display-time\">14:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsRomance_ArgIsLocal_ResultIsRomance()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                var localTz = TimeZoneInfo.Local;
                DateTime orig = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Utc);
                DateTime source = orig.ToLocalTime();
                DateTime romance = tz.ConvertFromUtc(source);
                Debug.Assert(source.Kind == DateTimeKind.Local);
                string expected = "<time datetime=\"2013-01-29T" + orig.Hour + ":28:21.0010000Z\" title=\"29/01/2013 14:28:21\" class=\"past not-today display-time\">14:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void UserIsRomance_ArgIsUser_ResultIsRomance()
            {
                Thread.CurrentThread.CurrentCulture = TestCulture1;
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime source = new DateTime(2013, 1, 29, 13, 28, 21, 1, DateTimeKind.Unspecified);
                DateTime utc = tz.ConvertToUtc(source);
                Debug.Assert(source.Kind == DateTimeKind.Unspecified);
                string expected = "<time datetime=\"2013-01-29T12:28:21.0010000Z\" title=\"29/01/2013 13:28:21\" class=\"past not-today display-time\">13:28:21</time>";
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);

                var result = SrkHtmlExtensions.DisplayTime(html, source);

                Assert.AreEqual(expected, result.ToString());
            }
        }

        [TestClass]
        public class JsDateMethod
        {
            private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            [TestMethod]
            public void BackAndForth_UtcIn()
            {
                long epoch = 13912786171000L; // GMT: Sat, 01 Feb 2014 18:16:57 GMT
                DateTime utc = UnixEpoch.AddMilliseconds(epoch);
                var result = SrkHtmlExtensions.JsDate(null, utc);
                var expected = "new Date(13912786171000)";
                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void BackAndForth_UserTzIn()
            {
                long epoch = 13912786171000L; // GMT: Sat, 01 Feb 2014 18:16:57 GMT
                DateTime utc = UnixEpoch.AddMilliseconds(epoch);
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime user = tz.ConvertFromUtc(utc);
                var html = CreateHtmlHelper(new ViewDataDictionary());
                html.SetTimezone(tz);
                var result = SrkHtmlExtensions.JsDate(html, user);
                var expected = "new Date(13912786171000)";
                Assert.AreEqual(expected, result.ToString());
            }

            [TestMethod]
            public void BackAndForth_LocalTzIn()
            {
                long epoch = 13912786171000L; // GMT: Sat, 01 Feb 2014 18:16:57 GMT
                DateTime utc = UnixEpoch.AddMilliseconds(epoch);
                DateTime local = utc.ToLocalTime();
                var html = CreateHtmlHelper(new ViewDataDictionary());
                var result = SrkHtmlExtensions.JsDate(html, local);
                var expected = "new Date(13912786171000)";
                Assert.AreEqual(expected, result.ToString());
            }
        }
    }
}
