using System;
using Snpower.TestActions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Snpower.TestCases
{
    [TestClass]
    public class FS_AboutFile : BaseTest
    {
        private static LoginPage loginPage = new LoginPage();
        private static FileSharePage fileSharePage = new FileSharePage();
        private static GeneralPage generalPage = new GeneralPage();
        private static BrowserMonkey browserMonkey = BrowserMonkey.Instance;

        //Class variables
        FileInfo fileInfo = new FileInfo();
        List<string> fileName = new List<string>();
        private string country = "Norway";
        private string wordTemplate = "Reference Document Word";
        private string randomText;

        [TestInitialize]
        public void TestInit()
        {
            TestSetUp();
            randomText = "AUTO" + Guid.NewGuid().ToString().Split('-')[0];

            loginPage.Login(USER_NAME, PASSWORD);

            fileSharePage.SelectCountry(country);

            fileInfo = new FileInfo()
            {
                FileTitle = "Auto Test",
                FileDescription = "Auto Description",
            };

            fileName = fileSharePage.CreateDocuments(fileInfo, wordTemplate, 1);
        }

        [TestMethod]
        [Owner("Danny Cai")]
        [TestCategory("CreateReferenceDocument")]
        [Description("File Share - Verify About File  Functionality of any document")]
        public void TC3722_AboutFileDocument()
        {
            //Edit about file
            fileSharePage.EditTitleAboutFile(fileName[0], "Auto Test2", "Auto Description2");

            //Verify edited information is saved
            Assert.IsTrue(fileSharePage.DoesAboutFileCorrect(fileName[0], "Auto Test2", "Auto Description2"), "About file should be updated.");
            fileSharePage.CloseAboutFile();

            //Delete
            fileSharePage.DeleteFile(fileName[0], "Auto reason");
        }
    }
}