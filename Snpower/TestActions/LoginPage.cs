using System.Collections.Generic;
using System.Data;
using OpenQA.Selenium;
using Snpower.TestCases;

namespace Snpower.TestActions
{
    public class LoginPage : GeneralPage
    {
        private static BrowserMonkey browserMonkey = BrowserMonkey.Instance;

        private string txtUserName = "input[name='loginfmt']";
        private string btnNext = "input[id*='idSIButton']";
        private string txtPassword = "input[name='passwd']";
        private string txtPreProdPassword = "#passwordInput";
        private string btnPreProdSignIn = "#submitButton";
        private string btnSignIn = "#cred_sign_in_button";
        private string loadingDiv = "#redirect_dots_animation";
        private string oldLink = "#uxOptOutLink";
        private string addAnotherAccount = "#use_another_account";
        private string btnNoSave = "#idBtn_Back";

        public void Login(string username, string password)
        {
            browserMonkey.Write(txtUserName, username);
            browserMonkey.Click(btnNext);
            browserMonkey.WaitForControlDisappear(loadingDiv);
            browserMonkey.Write(txtPassword, password);
            browserMonkey.Click(btnNext);
            browserMonkey.WaitForControlDisappear(loadingDiv);
            browserMonkey.Sleep(3);

            if(browserMonkey.Exists(btnNoSave))
            {
                browserMonkey.Click(btnNoSave);
                browserMonkey.Sleep(3);
            }

            if (BaseTest.ENV.ToUpper().Equals("PREPROD"))
            {
                browserMonkey.WaitForControlAppear(txtPreProdPassword);
                browserMonkey.Write(txtPreProdPassword, password);
                browserMonkey.Click(btnPreProdSignIn);
            }

            WaitForMainLoading();
        }
    }
}
