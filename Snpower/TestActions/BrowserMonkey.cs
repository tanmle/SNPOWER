using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Snpower.TestActions
{
    public enum WaitType
    {
        Validation = 0,
        Interaction = 1,
        Element = 2,
        Page = 3
    }

    public enum Browser
    {
        None,
        Chrome,
        Edge,
        IE,
        Firefox,
    }

    public class BrowserMonkeyValidationException : Exception
    {
        public BrowserMonkeyValidationException(string message, Exception ex) : base(message, ex) { }
    }

    public class BrowserMonkeyInteractionException : Exception
    {
        public BrowserMonkeyInteractionException(string message, Exception ex) : base(message, ex) { }
    }

    public class BrowserMonkeyElementException : Exception
    {
        public BrowserMonkeyElementException(string message, Exception ex) : base(message, ex) { }
    }

    public class BrowserMonkeyPageException : Exception
    {
        public BrowserMonkeyPageException(string message, Exception ex) : base(message, ex) { }
    }

    public class BrowserMonkey
    {
        public IWebDriver driver;
        public double ValidationTimeout = 12.0;
        public double InteractionTimeout = 12.0;
        public double ElementTimeout = 12.0;
        public double PageTimeout = 60.0;

        private static BrowserMonkey _instance;

        public static BrowserMonkey Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BrowserMonkey();
                }
                return _instance;
            }
        }

        public By ToBy(string selector)
        {
            //TODO make it smarter
            if (selector.StartsWith("#") ||
                selector.StartsWith(".") ||
                selector.StartsWith("td[") ||
                selector.StartsWith("tr[") ||
                selector.StartsWith("td ") ||
                selector.StartsWith("tr ") ||
                selector.StartsWith("input[") ||
                selector.StartsWith("span[") ||
                selector.StartsWith("div") ||
                selector.StartsWith("h1") ||
                selector.StartsWith("ul") ||
                selector.StartsWith("documents") ||
                selector.StartsWith("a") ||
                selector.StartsWith("button") ||
                selector.StartsWith("li"))// indicates css selector
            {
                return By.CssSelector(selector);
            }
            else if (selector.StartsWith("//") ||
                     selector.StartsWith("(//") ||
                     selector.StartsWith("(//") ||
                    selector.StartsWith("((//") ||
                     selector.StartsWith("id(") ||
                     selector.StartsWith("(id("))
            {
                return By.XPath(selector);
            }
            else
            {
                return ByTextOrValue(selector);
            }
        }

        public By ByTextOrValue(string text)
        {
            return By.XPath(string.Format("//*[text()=\"{0}\"] | //*[@value=\"{0}\"] | //*[normalize-space(text())=\"{0}\"]", text));
        }

        public T Wait<T>(double seconds, WaitType waitType, Func<string> errorMessageFunc, Func<IWebDriver, T> func)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            try
            {
                return wait.Until(func);
            }
            catch (WebDriverTimeoutException ex)
            {
                var message = errorMessageFunc();
                switch (waitType)
                {
                    case WaitType.Validation: throw new BrowserMonkeyValidationException(message, ex);
                    case WaitType.Interaction: throw new BrowserMonkeyInteractionException(message, ex);
                    case WaitType.Element: throw new BrowserMonkeyElementException(message, ex);
                    case WaitType.Page: throw new BrowserMonkeyPageException(message, ex);
                    default: throw;
                }
            }
            catch (BrowserMonkeyValidationException) { throw; }
            catch (BrowserMonkeyInteractionException) { throw; }
            catch (BrowserMonkeyElementException) { throw; }
            catch (BrowserMonkeyPageException) { throw; }
            catch { return default(T); }
        }

        public T Wait<T>(double seconds, WaitType waitType, string message, Func<IWebDriver, T> func)
        {
            return Wait(seconds, waitType, () => message, func);
        }

        public IWebElement GetWebElement(By by, double? seconds = null)
        {
            var message = string.Format("Unable to find element {0}", by);
            return Wait(seconds ?? ElementTimeout,
                        WaitType.Element,
                        message,
                        browser => driver.FindElements(by).FirstOrDefault());
        }

        public IWebElement GetWebElement(string selector)
        {
            return GetWebElement(ToBy(selector));
        }

        public ReadOnlyCollection<IWebElement> GetWebElements(By by)
        {
            var message = string.Format("Unable to find elements {0}", by);
            return Wait(ElementTimeout,
                        WaitType.Element,
                        message,
                        browser => driver.FindElements(by));
        }

        public ReadOnlyCollection<IWebElement> GetWebElements(string selector)
        {
            return GetWebElements(ToBy(selector));
        }

        public IWebElement UnreliableElement(By by)
        {
            return driver.FindElements(by).FirstOrDefault();
        }

        public IWebElement UnreliableElement(string selector)
        {
            return UnreliableElement(ToBy(selector));
        }

        public ReadOnlyCollection<IWebElement> UnreliableElements(By by)
        {
            return driver.FindElements(by);
        }

        public ReadOnlyCollection<IWebElement> UnreliableElements(string selector)
        {
            return UnreliableElements(ToBy(selector));
        }

        public void Quit()
        {
            try
            {
                driver.Quit();
            }
            catch { }
        }

        public void Url(string url)
        {
            driver.Navigate().GoToUrl(url);
            //total desperation
            this.TryWaitForJavascriptIdle();
        }

        private void WriteToSelect(IWebElement element, string text)
        {
            var xpath = string.Format(@"option[text()=""{0}""] | option[@value=""{0}""] | optgroup/option[text()=""{0}""] | optgroup/option[@value=""{0}""]", text);
            var options = element.FindElements(By.XPath(xpath));
            if (options.Count == 0)
            {
                var message = string.Format("Element {0} does not contain value {1}", element, text);
                throw new NoSuchElementException(message);
            }

            options.First().Click();
        }

        private void StartsWithWriteToSelect(IWebElement element, string text)
        {
            var xpath = string.Format(@"option[starts-with(text(),""{0}"")] | option[starts-with(@value,""{0}"")] | optgroup/option[starts-with(text(),""{0}"")] | optgroup/option[starts-with(@value,""{0}"")]", text);
            var options = element.FindElements(By.XPath(xpath));
            if (options.Count == 0)
            {
                var message = string.Format("Element {0} does not contain value {1}", element, text);
                throw new NoSuchElementException(message);
            }

            options.First().Click();
        }

        public void SafeWriteToSelect(By select, By options, string text)
        {
            Wait(
                ValidationTimeout,
                WaitType.Validation,
                "Safe Write to Select failed",
                browser =>
                {
                    StarEq(options, text);
                    var element = GetWebElement(select);
                    WriteToSelect(element, text);
                    if (Read(select) != text)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                });
        }

        public void SafeWriteToSelect(string selectSelector, string optionsSelector, string text)
        {
            SafeWriteToSelect(ToBy(selectSelector), ToBy(optionsSelector), text);
        }

        public void StartsWithWriteToSelect(By select, string fuzzyText)
        {
            Wait(
                ValidationTimeout,
                WaitType.Validation,
                "Fuzzy Write to Select failed",
                browser =>
                {
                    var element = GetWebElement(select);
                    StartsWithWriteToSelect(element, fuzzyText);
                    return true;
                });
        }

        public void StartsWithWriteToSelect(string selectSelector, string fuzzyText)
        {
            StartsWithWriteToSelect(ToBy(selectSelector), fuzzyText);
        }

        private void WriteToElement(IWebElement element, string text, By by, double sleepAfterClear)
        {
            if (element.GetAttribute("readonly") != null)
            {
                throw new Exception(string.Format("Cannot write to read only control: {0}", by));
            }

            if (element.TagName == "select")
            {
                WriteToSelect(element, text);
            }
            else
            {
                Clear(element);
                Sleep(sleepAfterClear);
                element.SendKeys(text);
            }
        }

        public void Write(By by, string text, double sleepAfterClear = 0.0)
        {
            text = text ?? "";
            var message = string.Format("Unable to write value `{0}` to {1}", text, by);
            Wait(InteractionTimeout,
                WaitType.Interaction,
                 message,
                 browser =>
                 {
                     var elements = GetWebElements(by);
                     return elements.Select(element =>
                     {
                         try
                         {
                             WriteToElement(element, text, by, sleepAfterClear);
                             return true;
                         }
                         catch (Exception)
                         {
                             return false;
                         }
                     })
                     .ToList()
                     .Any(predicate => predicate);
                 });
        }

        public void Drag(string selector, string value, double sleepAfterClear = 0.0)
        {
            Drag(ToBy(selector), value, sleepAfterClear);
        }

        public void Drag(By by, string value, double sleepAfterClear = 0.0)
        {
            value = value ?? "";
            var message = string.Format("Unable to drag value `{0}` to {1}", value, by);
            Wait(InteractionTimeout,
                WaitType.Interaction,
                 message,
                 browser =>
                 {
                     var elements = GetWebElements(by);
                     return elements.Select(element =>
                     {
                         try
                         {
                             //int x = 5;
                             //int width = element.Size.Width;

                             new Actions(driver)
                .DragAndDropToOffset(element, 1, 0)
                .Build()
                .Perform();
                             new Actions(driver).DragAndDropToOffset(element, 0, 1)
                .Build()
                .Perform();
                             return true;
                         }
                         catch (Exception)
                         {
                             return false;
                         }
                     })
                     .ToList()
                     .Any(predicate => predicate);
                 });
        }

        public void Write(string selector, string text, double sleepAfterClear = 0.0)
        {
            Write(ToBy(selector), text, sleepAfterClear);
        }

        public void SafeWrite(By by, string text, double sleepAfterClear = 0.0)
        {
            text = text ?? "";
            var message = string.Format("Unable to safe write value `{0}` to {1}", text, by);
            Wait(InteractionTimeout,
                WaitType.Interaction,
                 message,
                 browser =>
                 {
                     var elements = GetWebElements(by);
                     return elements.Select(element =>
                     {
                         try
                         {
                             WriteToElement(element, text, by, sleepAfterClear);
                             return Read(by) == text;
                         }
                         catch (Exception)
                         {
                             return false;
                         }
                     })
                     .ToList()
                     .Any(predicate => predicate);
                 });
        }

        public void SafeWrite(string selector, string text, double sleepAfterClear = 0.0)
        {
            SafeWrite(ToBy(selector), text, sleepAfterClear);
        }

        [DebuggerStepThrough]
        public string Read(IWebElement element)
        {
            string text;

            switch (element.TagName)
            {
                case "input":
                case "textarea":
                    text = element.GetAttribute("value");
                    break;
                case "select":
                    var selectElement = new SelectElement(element);
                    try
                    {
                        text = selectElement.SelectedOption.Text;
                    }
                    catch
                    {
                        text = "";
                    }
                    break;
                default:
                    text = element.Text;
                    break;
            }

            return text;
        }

        public string Read(By by)
        {
            var results =
                Wait(
                    InteractionTimeout,
                    WaitType.Interaction,
                    "Trying to read",
                    browser => Read(GetWebElement(by)));
            return results;
        }

        public string Read(string selector)
        {
            return Read(ToBy(selector));
        }

        public List<string> ReadAvailableSelectOptions(IWebElement element)
        {
            var selectElement = new SelectElement(element);

            try
            {
                return selectElement.Options.Select(_ => _.Text).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public List<string> ReadAvailableSelectOptions(By by)
        {
            var results =
                Wait(
                    InteractionTimeout,
                    WaitType.Interaction,
                    "Trying to read available select options",
                    browser => ReadAvailableSelectOptions(GetWebElement(by)));
            return results;
        }

        public List<string> ReadAvailableSelectOptions(string selector)
        {
            return ReadAvailableSelectOptions(ToBy(selector));
        }

        public void Eq(By by, string expected, Func<string> errorMessageFunc)
        {
            Wait(ValidationTimeout,
                WaitType.Validation,
                errorMessageFunc,
                browser => Read(by) == expected);
        }

        public void Eq(By by, string expected)
        {
            Func<string> errorMessageFunc = () => String.Format("Equality check for `{0}` failed, Expected: `{1}` / Actual: `{2}`", @by, @expected, Read(@by));
            Eq(by, expected, errorMessageFunc);
        }

        public void Eq(string selector, string expected, string errorMessage)
        {
            Func<string> errorMessageFunc = () => String.Format("Equality check for `{0}` failed, Expected: `{1}` / Actual: `{2}`\r\n`{3}`", @selector, @expected, Read(selector), @errorMessage);
            Wait(ValidationTimeout,
                WaitType.Validation,
                errorMessageFunc,
                browser => Read(ToBy(selector)) == expected);
        }

        public void Eq(string selector, string expected)
        {
            Eq(ToBy(selector), expected);
        }

        public void EqOr(By by, string expected, string orExpected, string errorMessage = "")
        {
            Func<string> errorMessageFunc = () => String.Format("Or Equality check for `{0}` failed, Expected: `{1} or {4} ` / Actual: `{2}`\r\n`{3}`", by, @expected, Read(by), @errorMessage, orExpected);
            Wait(ValidationTimeout,
                WaitType.Validation,
                errorMessageFunc,
                browser => Read(by) == expected || Read(by) == orExpected);
        }

        public void EqOr(string selector, string expected, string orExpected)
        {
            EqOr(ToBy(selector), expected, orExpected);
        }

        public void NotEq(By by, string expected, string errorMessage = "")
        {
            var message = String.Format("Not Equal check for {0} failed {1}\r\n", @by, errorMessage);
            Wait(ValidationTimeout,
                WaitType.Validation,
                 message,
                 browser => Read(by) != expected);
        }

        public void NotEq(string selector, string expected, string errorMessage = "")
        {
            NotEq(ToBy(selector), expected, errorMessage);
        }

        public void RegexEq(By by, string expected)
        {
            var message = string.Format("Regex Equality check for {0} failed", by);
            Wait(ValidationTimeout,
                WaitType.Validation,
                 message,
                 browser => Regex.Match(Read(by), expected).Success);
        }

        public void RegexEq(string selector, string expected)
        {
            RegexEq(ToBy(selector), expected);
        }

        public void RegexNotEq(By by, string expected)
        {
            var message = string.Format("Regex Not Equal check for {0} failed", by);
            Wait(ValidationTimeout,
                WaitType.Validation,
                 message,
                 browser => Regex.Match(Read(by), expected).Success == false);
        }

        public void RegexNotEq(string selector, string expected)
        {
            RegexNotEq(ToBy(selector), expected);
        }

        public void Count(By by, int expected, string error)
        {
            Wait(ValidationTimeout,
                WaitType.Validation,
                 () => String.Format("Count check for {0} failed, Expected: {1} Got: {2}\r\n{3}", @by, expected, GetWebElements(@by).Count, error),
                 browser => GetWebElements(by).Count == expected);
        }

        public void Count(string selector, int expected, string error)
        {
            Count(ToBy(selector), expected, error);
        }

        public void Count(By by, int expected)
        {
            Count(by, expected, string.Empty);
        }

        public void Count(string selector, int expected)
        {
            Count(ToBy(selector), expected);
        }

        public void GreaterThanOrEqual(By by, int expected, string errorMessage = "")
        {
            var message = string.Format("GreaterThanOrEqual check for {0} failed {1}\r\n", by, errorMessage);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 message,
                 browser => GetWebElements(by).Count >= expected);
        }

        public void GreaterThanOrEqual(string selector, int expected, string errorMessage = "")
        {
            GreaterThanOrEqual(ToBy(selector), expected, errorMessage);
        }

        public void LessThanOrEqual(By by, int expected, string errorMessage = "")
        {
            var message = string.Format("LessThanOrEqual check for {0} failed {1}\r\n", by, errorMessage);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 message,
                 browser => GetWebElements(by).Count <= expected);
        }

        public void LessThanOrEqual(string selector, int expected, string errorMessage = "")
        {
            LessThanOrEqual(ToBy(selector), expected, errorMessage);
        }

        public void StarEq(By by, string expected, string errorMessage = "")
        {
            Func<string> errorMessageFunc = () => String.Format("Star Equals check for `{0}` failed, Expected: `{1}` / Actual: `{2}`\r\n`{3}`", by, @expected, String.Join(" | ", GetWebElements(by).Select(e => Read(e))), @errorMessage);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 errorMessageFunc,
                 browser => GetWebElements(by).Any(element => Read(element) == expected));
        }

        public void StarEq(string selector, string expected, string errorMessage = "")
        {
            StarEq(ToBy(selector), expected, errorMessage);
        }

        public void NotStarEq(By by, string expected, string errorMessage = "")
        {
            var message = string.Format("Not Star Equals check for {0} failed \r\n {1}", by, errorMessage);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 message,
                 browser => GetWebElements(by).All(element => Read(element) != expected));
        }

        public void NotStarEq(string selector, string expected, string errorMessage = "")
        {
            NotStarEq(ToBy(selector), expected, errorMessage);
        }

        public void StarRegexEq(By by, string expected)
        {
            Func<string> messageFunc = () => string.Format("Star Regex Equals check for {0} failed, Expected {1}, Got {2}", by, expected,
                GetWebElements(by).Select(element => Read(element)).Aggregate((current, next) => current + ", " + next));
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 messageFunc,
                 browser => GetWebElements(by).Any(element => Regex.Match(Read(element), expected).Success));
        }

        public void StarRegexEq(string selector, string expected)
        {
            StarRegexEq(ToBy(selector), expected);
        }

        public bool Exists(By by)
        {
            return UnreliableElements(by).Any();
        }

        public bool Exists(string selector)
        {
            return Exists(ToBy(selector));
        }


        public void ReloadUntilEmailDisplays(string selector, int timeOut=60)
        {
            while (!Exists(selector) && timeOut > 0)
            {
                WaitForControlDisappear("#owaLoading");
                Reload();
                timeOut--;
                Sleep(1);
            }
        }

        public void NotExists(By by, double timeout)
        {
            var message = string.Format("Not Exists failed for {0}", by);
            Wait(
                timeout,
                WaitType.Validation,
                message,
                browser => UnreliableElements(by).Any() == false);
        }

        public void NotExists(string selector, double timeout)
        {
            NotExists(ToBy(selector), timeout);
        }

        public void Selected(By by)
        {
            var message = string.Format("Selected check for {0} failed", by);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 message,
                 browser => GetWebElement(by).Selected);
        }

        public void Selected(string selector)
        {
            Selected(ToBy(selector));
        }

        public void NotSelected(By by)
        {
            var message = string.Format("Not Selected check for {0} failed", by);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 message,
                 browser => GetWebElement(by).Selected == false);
        }

        public void NotSelected(string selector)
        {
            NotSelected(ToBy(selector));
        }
        public void Displayed(By by, double seconds, string error)
        {
            var message = string.Format("Displayed check for {0} failed\r\n{1}", by, error);
            Wait(seconds,
                 WaitType.Validation,
                 message,
                 browser => GetWebElement(by, seconds).Displayed);
        }

        public void Displayed(string selector, double seconds, string error)
        {
            Displayed(ToBy(selector), seconds, error);
        }

        public void Displayed(By by, double seconds)
        {
            Displayed(by, ValidationTimeout, String.Empty);
        }

        public void Displayed(string selector, double seconds)
        {
            Displayed(ToBy(selector), seconds, String.Empty);
        }

        public void Displayed(string selector, string error)
        {
            Displayed(ToBy(selector), ValidationTimeout, error);
        }

        public void Displayed(By by)
        {
            Displayed(by, ValidationTimeout, String.Empty);
        }

        public void Displayed(By by, string error)
        {
            Displayed(by, ValidationTimeout, error);
        }

        public void Displayed(string selector)
        {
            Displayed(ToBy(selector), ValidationTimeout, String.Empty);
        }

        public bool IsVisible(By by)
        {
            var element = UnreliableElement(by);

            if (element == null)
                return false;
            return !element.GetAttribute("style").Contains("display: none;") && element.Displayed;
        }

        public bool IsVisible(string selector)
        {
            return IsVisible(ToBy(selector));
        }

        public void NotDisplayed(By by, double timeout, string error)
        {
            var message = string.Format("Not Displayed check for {0} failed\r\n{1}", by, error);
            Wait(timeout,
                 WaitType.Validation,
                 message,
                 browser =>
                 {
                     var element = UnreliableElement(by);
                     return element == null || element.Displayed == false;
                 });
        }

        public void NotDisplayed(By by, double timeout)
        {
            NotDisplayed(by, timeout, String.Empty);
        }

        public void NotDisplayed(By by)
        {
            NotDisplayed(by, ValidationTimeout, string.Empty);
        }

        public void NotDisplayed(string selector, string error)
        {
            NotDisplayed(ToBy(selector), ValidationTimeout, error);
        }

        public void NotDisplayed(string selector)
        {
            NotDisplayed(ToBy(selector));
        }

        public void NotDisplayed(string selector, double timeout)
        {
            NotDisplayed(ToBy(selector), timeout);
        }

        public void Enabled(By by)
        {
            var message = string.Format("Enabled check for {0} failed", by);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 message,
                 browser => GetWebElement(by).Enabled);
        }

        public void Enabled(string selector)
        {
            Enabled(ToBy(selector));
        }

        public void Disabled(By by)
        {
            var message = string.Format("Disabled check for {0} failed", by);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 message,
                 browser => GetWebElement(by).Enabled == false);
        }

        public void Disabled(string selector)
        {
            Disabled(ToBy(selector));
        }
        public void Deselected(By by)
        {
            var message = string.Format("Deselected check for {0} failed", by);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 message,
                 browser => GetWebElement(by).Selected == false);
        }

        public void Deselected(string selector)
        {
            Deselected(ToBy(selector));
        }

        public void IsLabel(By by)
        {
            var message = string.Format("IsLabel check for {0} failed", by);
            Wait(ValidationTimeout,
                 WaitType.Validation,
                 message,
                 browser => GetWebElement(by).TagName == "label");
        }
        public void IsLabel(string selector)
        {
            IsLabel(ToBy(selector));
        }

        public void Clear(IWebElement element)
        {
            element.Clear();
        }

        public void Clear(By by)
        {
            var element = GetWebElement(by);
            Clear(element);
        }

        public void Clear(string selector)
        {
            Clear(ToBy(selector));
        }

        public void Click(IWebElement element)
        {
            var message = string.Format("Click for {0} failed", element);
            Wait(InteractionTimeout,
                 WaitType.Interaction,
                 message,
                 browser =>
                 {
                     try
                     {
                         element.Click();
                         return true;
                     }
                     catch (Exception)
                     {
                         return false;
                     }
                 });
        }

        public void Click(By by)
        {
            var message = string.Format("Click for {0} failed", by);
            Wait(InteractionTimeout,
                 WaitType.Interaction,
                 message,
                 browser =>
                 {
                     var elements = GetWebElements(by);
                     return elements.Select(element =>
                     {
                         try
                         {
                             element.Click();
                             return true;
                         }
                         catch (Exception)
                         {
                             return false;
                         }
                     }).Any(predicate => predicate);
                 });
        }

        public void Click(string selector)
        {
            Click(ToBy(selector));
        }

        public void JavaScriptClick(string selector)
        {
            var element = GetWebElement(ToBy(selector));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
        }

        public void Check(By by)
        {
            var message = string.Format("Check for {0} failed", by);
            Wait(InteractionTimeout,
                 WaitType.Interaction,
                 message,
                 browser =>
                 {
                     var element = GetWebElement(by);
                     if (element.Selected == false)
                     {
                         Click(by);
                     }
                     return element.Selected;
                 });
        }

        public void Check(string selector)
        {
            Check(ToBy(selector));
        }

        public void CheckAll(By by)
        {
            var message = string.Format("Check All for {0} failed", by);
            Wait(InteractionTimeout,
                 WaitType.Interaction,
                 message,
                 browser =>
                 {
                     var elements = GetWebElements(by).ToList();
                     return elements.Select(element =>
                     {
                         if (element.Selected == false)
                         {
                             element.Click();
                         }
                         return element.Selected;
                     })
                     .All(predicate => predicate);
                 });
        }

        public void CheckAll(string selector)
        {
            CheckAll(ToBy(selector));
        }

        public string ReadPageSource()
        {
            return driver.PageSource;
        }

        public void Uncheck(By by)
        {
            var message = string.Format("Uncheck for {0} failed", by);
            Wait(InteractionTimeout,
                 WaitType.Interaction,
                 message,
                 browser =>
                 {
                     var element = GetWebElement(by);
                     if (element.Selected)
                     {
                         Click(by);
                     }
                     return element.Selected == false;
                 });
        }

        public void Uncheck(string selector)
        {
            Uncheck(ToBy(selector));
        }

        public string CurrentUrl()
        {
            return driver.Url;
        }

        public void On(string url)
        {
            Func<string> messageFunc = () => String.Format("On check failed: Expected: {0} Actual: {1}", url.ToLower(), driver.Url.ToLower());
            Wait(PageTimeout,
                 WaitType.Page,
                 messageFunc,
                 browser => browser.Url.ToLower().Contains(url.ToLower()));
        }

        public void Reload()
        {
            Url(CurrentUrl());
        }

        public void SwitchToTab(int number)
        {
            number = number - 1;
            var message = string.Format("Switching to tab {0} failed", number);
            Wait(InteractionTimeout,
                 WaitType.Interaction,
                 message,
                 browser =>
                 {
                     var tabs = driver.WindowHandles;
                     if (tabs.Count >= number)
                     {
                         driver.SwitchTo().Window(tabs[number]);
                         return true;
                     }
                     {
                         return false;
                     }
                 });
        }

        public void CloseTab(int number)
        {
            SwitchToTab(number);
            driver.Close();
        }

        public object ExecuteJs(string script)
        {
            return ((IJavaScriptExecutor)driver).ExecuteScript(script);
        }

        public void Sleep(double seconds)
        {
            if (seconds > 0.0) Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }

        public void AcceptAlert()
        {
            var message = "Failed to Accept Alert";
            Wait(InteractionTimeout,
                 WaitType.Interaction,
                 message,
                 browser =>
                 {
                     driver.SwitchTo().Alert().Accept();
                     return true;
                 });
        }

        public void TryAcceptAlert()
        {
            try
            {
                Sleep(0.5);
                driver.SwitchTo().Alert().Accept();
            }
            catch
            {
                //swallow on purpose.  May not have an alert, which would throw an exception
            }
        }

        public void Forward()
        {
            driver.Navigate().Forward();
        }

        public void Back()
        {
            driver.Navigate().Back();
        }

        public void Screenshot(string directory, string testName)
        {
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(Path.Combine(directory, testName), ScreenshotImageFormat.Png);
        }

        public List<string> GetColumnContents(By by)
        {
            return Wait(
                InteractionTimeout,
                WaitType.Interaction,
                "Failed to get Column Contents",
                browser =>
                {
                    var elements = GetWebElements(by);

                    List<string> contents = new List<string>();

                    foreach (var element in elements)
                    {
                        contents.Add(Read(element));
                    }

                    return contents;
                });
        }

        public List<string> GetColumnContents(string selector)
        {
            return GetColumnContents(ToBy(selector));
        }

        public void ScrollIntoView(By by)
        {
            var element = GetWebElement(by);

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        public void ScrollIntoView(string selector)
        {
            Sleep(2);
            ScrollIntoView(ToBy(selector));
            Sleep(1);
        }

        public void Red(By by, string errorMessage = "")
        {
            var message = string.Format("{0} was not Red \r\n {1}", by, errorMessage);
            Wait(
                ValidationTimeout,
                WaitType.Validation,
                message,
                browser =>
                {
                    var element = GetWebElement(by);
                    return
                      element.GetAttribute("style").Contains("border-color: red")
                      || element.GetCssValue("border-color").Contains("rgb(255, 0, 0)");
                });
        }

        public void Red(string selector, string errorMessage = "")
        {
            Red(ToBy(selector), errorMessage);
        }

        public void NotRed(By by)
        {
            var message = string.Format("{0} was Red", by);
            Wait(
                ValidationTimeout,
                WaitType.Validation,
                message,
                browser => GetWebElement(by).GetAttribute("style").Contains("border-color: red") == false);
        }

        public void NotRed(string selector)
        {
            NotRed(ToBy(selector));
        }

        public void DeleteCookie(string name)
        {
            var cookies = driver.Manage().Cookies;
            cookies.DeleteCookieNamed(name);
        }

        public void Checked(By by, string errorMessage = "")
        {
            var message = String.Format("{0} was not checked \r\n{1}", @by, errorMessage);
            Wait(ValidationTimeout,
                WaitType.Validation,
                message,
                browser => GetWebElement(by).Selected == true);
        }

        public void Checked(string selector, string errorMessage = "")
        {
            Checked(ToBy(selector), errorMessage);
        }

        public void NotChecked(By by, string errorMessage = "")
        {
            var message = String.Format("{0} was checked\r\n{1}", @by, errorMessage);
            Wait(ValidationTimeout,
                WaitType.Validation,
                message,
                browser => GetWebElement(by).Selected == false);
        }

        public void NotChecked(string selector, string errorMessage = "")
        {
            NotChecked(ToBy(selector));
        }

        public void AllChecked(By by)
        {
            var message = String.Format("All {0} were NOT checked", @by);
            Wait(ValidationTimeout,
                WaitType.Validation,
                message,
                browser => GetWebElements(by).All(element => element.Selected));
        }

        public void AllChecked(string selector)
        {
            AllChecked(ToBy(selector));
        }

        public void AllNotChecked(By by)
        {
            var message = String.Format("All {0} were checked", @by);
            Wait(ValidationTimeout,
                WaitType.Validation,
                message,
                browser => GetWebElements(by).All(element => element.Selected == false));
        }

        public void AllNotChecked(string selector)
        {
            AllNotChecked(ToBy(selector));
        }

        public string GetElementText(By by)
        {
            return GetWebElement(by).Text;
        }

        public string GetElementText(IWebElement element)
        {
            return element.Text;
        }

        public string GetElementText(string selector)
        {
            return GetElementText(ToBy(selector));
        }

        public void Open(Browser browser)
        {
            switch (browser)
            {
                case Browser.Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("test-type");
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArguments("--disable-extensions");
                    chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                    chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
                    driver = new ChromeDriver(chromeOptions);
                    break;
                case Browser.Firefox:
                    driver = new FirefoxDriver();
                    break;
                case Browser.IE:
                    driver = new InternetExplorerDriver();
                    break;
            }

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
        }

        public void PinFullScreen()
        {
            driver.Manage().Window.Maximize();
        }

        public void Enter(By by)
        {
            GetWebElement(by).SendKeys(Keys.Enter);
        }

        public void Enter(string selector)
        {
            Enter(ToBy(selector));
        }

        public int GetCount(string selector)
        {
            return GetWebElements(ToBy(selector)).Count;
        }

        public string GetAttribute(string selector, string attribute)
        {
            return GetWebElement(ToBy(selector)).GetAttribute(attribute);
        }

        public int CountOfTabs()
        {
            return driver.WindowHandles.Count;
        }

        public string ReadAlertMessage()
        {
            var value = driver.SwitchTo().Alert().Text;
            return value;
        }

        public void SwitchToDefaultContent()
        {
            driver.SwitchTo().DefaultContent();
        }

        public void SwitchToWindow(string window)
        {
            driver.SwitchTo().Window(window);
        }

        public string GetCurrentWindow()
        {
            return driver.CurrentWindowHandle;
        }

        public void SwitchToFrame(int frameNumber)
        {
            Wait(
                InteractionTimeout,
                WaitType.Interaction,
                "Trying to switch frames",
                browser =>
                {
                    try
                    {
                        driver.SwitchTo().Frame(frameNumber);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
        }

        public void SwitchToFrame(By by)
        {
            SwitchToDefaultContent();

            Wait(
                InteractionTimeout,
                WaitType.Interaction,
                "Trying to switch frames",
                browser =>
                {
                    try
                    {
                        driver.SwitchTo().Frame(driver.FindElement(by));
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
        }

        public int OpenTab()
        {
            int currentTabCount = driver.WindowHandles.Count;

            //IWebElement body = driver.FindElement(By.TagName("body"));
            //body.SendKeys(Keys.Control + 't');
            ExecuteJs("window.open('about:blank','_blank');");

            int updatedTabCount = driver.WindowHandles.Count;

            if (updatedTabCount <= currentTabCount)
            {
                throw new WebDriverException(string.Format("Failed opening a new tab. Current tab count: {0}, Updated tab count: {1}", currentTabCount, updatedTabCount));
            }

            // update the window handle - SwitchToTab is not used here as it would mistakenly
            // change to the wrong visible tab. It doesn't appear Selenium/WebDriver provides
            // the hooks to tell which tab in a browser is the "visible" tab. 
            driver.SwitchTo().Window(driver.WindowHandles[updatedTabCount - 1]);

            return updatedTabCount; // the tab number that was just opened
        }

        public IAlert Alert()
        {
            return driver.SwitchTo().Alert();
        }

        public void Focus(By by)
        {
            var element = GetWebElement(by);
            new Actions(driver).MoveToElement(element).Perform();
        }

        public void Focus(string selector)
        {
            Focus(ToBy(selector));
        }

        public void TryWaitForJavascriptIdle(int waitTimeoutSeconds = 2)
        {
            //Please dont use this unless there is a really tough spot you need to test with some weird bug you need to try to work around
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitTimeoutSeconds));
                // If an alert dialog is present then this call to javascript will fail out, to avoid some
                // noise when debugging tests we will check for the alert before attempting the wait
                if (null != ExpectedConditions.AlertIsPresent().Invoke(driver))
                    return;

                wait.Until<bool>(webDriver => (bool)((IJavaScriptExecutor)webDriver).ExecuteScript("if (typeof jQuery != 'undefined') { return jQuery.active == 0; } else {  return true; }"));
                wait.Until<bool>(webDriver => (bool)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch
            {
                // don't care
            }
        }

        public void WaitForControlDisappear(string selector, int timeout = 60)
        {
            var message = string.Format("Not Displayed check for {0} failed", selector);
            Wait(timeout,
                 WaitType.Element,
                 message,
                 browser =>
                 {
                     var element = UnreliableElement(selector);
                     return element == null || element.Displayed == false;
                 });
        }

        public void WaitForControlAppear(string selector, int timeout=60)
        {
            var message = string.Format("Displayed check for {0} failed", selector);
            Wait(timeout,
                WaitType.Element,
                message,
                browser => UnreliableElements(selector).Any());
        }


        public void JavaScriptSetAttribute(string selector, string attribute, string value)
        {
            var element = GetWebElement(ToBy(selector));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('" + attribute + "', '" + value + "')", element);
        }

        public void Hover(By by)
        {
            var message = string.Format("Hover for {0} failed", by);
            Wait(InteractionTimeout,
                 WaitType.Interaction,
                 message,
                 browser =>
                 {
                     var elements = GetWebElements(by);
                     return elements.Select(element =>
                     {
                         try
                         {
                             Actions action = new Actions(driver);
                             action.MoveToElement(element).Perform();
                             return true;
                         }
                         catch (Exception)
                         {
                             return false;
                         }
                     }).Any(predicate => predicate);
                 });
        }

        public void Hover(string selector)
        {
            Hover(ToBy(selector));
        }

        public void ClearAllCookies()
        {
            driver.Manage().Cookies.DeleteAllCookies();
        }
    }
}
