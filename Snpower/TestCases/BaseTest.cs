using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.IO;
using Snpower.TestActions;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Linq;

namespace Snpower.TestCases
{
    [TestClass]
    public class BaseTest
    {
        public TestContext TestContext { get; set; }
        private static BrowserMonkey browserMonkey = BrowserMonkey.Instance;

        public static string BROWSER = ConfigurationManager.AppSettings["BROWSER"];
        public static string ENV = ConfigurationManager.AppSettings["ENV"];
        public static string URL = ConfigurationManager.AppSettings[ENV + "_URL"];
        public static string USER_NAME = ConfigurationManager.AppSettings[ENV + "_USER_NAME"];
        public static string PASSWORD = ConfigurationManager.AppSettings[ENV + "_PASSWORD"];
        public static string NAME = ConfigurationManager.AppSettings[ENV + "_NAME"];

        public void TestSetUp()
        {
            switch (BROWSER.ToLower())
            {
                case "chrome":
                    browserMonkey.Open(Browser.Chrome);
                    break;
                case "firefox":
                    browserMonkey.Open(Browser.Firefox);
                    break;
                case "ie":
                    browserMonkey.Open(Browser.IE);
                    break;
            }
            
            browserMonkey.PinFullScreen();
            browserMonkey.Url(URL);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
            {
                if (File.Exists(Environment.CurrentDirectory + "\\" + TestContext.TestName + ".png"))
                {
                    File.Delete(Environment.CurrentDirectory + "\\" + TestContext.TestName + ".png");
                }

                browserMonkey.Screenshot(Environment.CurrentDirectory, TestContext.TestName + ".png");
                TestContext.AddResultFile(Environment.CurrentDirectory + "\\" + TestContext.TestName + ".png");
            }
            browserMonkey.Quit();
        }

        protected static Dictionary<string, object> GetTestData(string dataFileName)
        {
            try
            {
                string dataFilePath = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) + @"\TestData\"+ dataFileName;
                var jsonText = File.ReadAllText(dataFilePath);
                var jss = new JavaScriptSerializer();
                return jss.Deserialize<Dictionary<string, object>>(jsonText);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
