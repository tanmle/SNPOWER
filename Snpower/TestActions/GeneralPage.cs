using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Snpower.TestActions
{
    public class GeneralPage
    {
        private static BrowserMonkey browserMonkey = BrowserMonkey.Instance;

        private string pageTitle = "h1.ng-binding";
        private string selectedLeftNavigateItem = "ul.sims-left-nav__list li.active a";
        private string leftNavigateItem = "//ul[@class='sims-left-nav__list']//li/a[@ui-sref='{0}']";
        private string topRightMenu = "//button[@role='menuitem']/div[contains(@class,'mfp-header')]";
        private string logoutLink = "#O365_SubLink_ShellSignout";
        private string signoutText = ".signoutText";
        private string anotherSignoutText = "//div[@id='true_inner']/div[@class='login_workload_logo_container']/h1[contains(text(), 'You')]";
        private string toastMessage = "//div[@class='toast-message' and text()='{0}']";
        private string itemCreating = "//*[contains(@ng-if,'locked') and @title='Creating...']";
        public string mainLoadingDiv = "//div[@class='sims-overlay']";

        //Toast message
        public string processStarted = "The information process was started";
        public string activityWillCreate = "The activity will be created in background process";
        public string activityWillDelete = "The activity will be deleted in background process";
        public string projectWillCreate = "The project will be created in background process";
        public string documentWillRename = "The document will be renamed in background";
        public string documentWillUpdate = "The document will be updated in background process";
        public string documentWillCreate = "The document will be created in background process";
        public string faWillDelete = "The functional area will be deleted in background process";

        public string GetDocumentTitle()
        {
            return browserMonkey.Read(pageTitle).Trim();
        }

        public string GetSelectMenu()
        {
            return browserMonkey.GetAttribute(selectedLeftNavigateItem, "ui-sref");
        }

        public void SelecLeftNavigateItem(string item)
        {
            var temp = "";
            switch (item)
            {
                case "File Share":
                    temp = "Documents";
                    break;
                case "My Activities":
                    temp = "MyActivities.list";
                    break;
                case "Functional Areas":
                    temp = "FunctionalAreas.list";
                    break;
                case "Projects":
                    temp = "ProjectEntity.list";
                    break;
            }
            WaitForMainLoading();
            browserMonkey.Click(string.Format(leftNavigateItem, temp));
            WaitForMainLoading();
        }

        public void WaitForMainLoading()
        {
            browserMonkey.WaitForControlDisappear(mainLoadingDiv, 180);
        }

        public void WaitForCreating()
        {
            browserMonkey.WaitForControlDisappear(itemCreating, 180);
        }

        public void WaitForToastMessageAppear(string message)
        {
            browserMonkey.WaitForControlAppear(string.Format(toastMessage,message));
        }

        public void WaitForToastMessageDisappear(string message)
        {
            browserMonkey.WaitForControlDisappear(string.Format(toastMessage, message));
        }

        public void WaifForToastMessageAppearCompletely(string message)
        {
            WaitForToastMessageAppear(message);
            WaitForToastMessageDisappear(message);
        }

        public void Logout()
        {
            browserMonkey.Url("https://login.microsoftonline.com/common/oauth2/logout");
            browserMonkey.WaitForControlAppear(anotherSignoutText);
        }

        public void VerifySharedDocumentExistingInEmail(string email, string password, string subject, string fileName, string message)
        {
            var emailSubject =
                string.Format("//span[contains(@class,'lvHighlightSubjectClass')][contains(text(),'{0}')]",
                    subject);
            browserMonkey.ReloadUntilEmailDisplays(emailSubject);
            browserMonkey.Click(emailSubject);
            browserMonkey.Sleep(3);
            //Verify subject
            browserMonkey.WaitForControlAppear(string.Format("//div[@role='document']//span[contains(.,'{0}')]", subject));
            Assert.IsTrue(browserMonkey.IsVisible(string.Format("//div[@role='document']//span[contains(.,'{0}')]", subject)), "Subject should be: "+subject);
            //Verify file name
            browserMonkey.Sleep(3);
            Assert.IsTrue(browserMonkey.IsVisible(string.Format("//div[@role='document']//a[contains(.,'{0}')]", fileName)), "Filename should be: " + fileName);
            //Verify message
            Assert.IsTrue(browserMonkey.IsVisible(string.Format("//div[@role='document']//div[contains(.,'{0}')]", message)), "Message should be: " + message);
        }

        public void SelectToggle(string selector, string toggleValue)
        {
            browserMonkey.Click(selector);
            browserMonkey.Click(selector);

            if (toggleValue.Equals("Yes"))
            {
                while (browserMonkey.GetAttribute(selector + "/../input", "class").Equals("ms-Toggle-input ng-valid ng-dirty ng-valid-parse ng-touched ng-empty"))
                {
                    browserMonkey.Click(selector);
                }
            }
            else
            {
                while (browserMonkey.GetAttribute(selector + "/../input", "class").Equals("ms-Toggle-input ng-valid ng-dirty ng-valid-parse ng-touched ng-not-empty"))
                {
                    browserMonkey.Click(selector);
                }
            }
        }
    }
}
