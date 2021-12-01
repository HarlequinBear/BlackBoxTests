using BlackBoxTests.WebAutomation.Exceptions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace BlackBoxTests.WebAutomation
{
    public class BbtWebDriver : IBbtWebDriver
    {
        protected IWebDriver Browser;

        public BbtWebDriver(BbtWebDriverType driverType)
        {
            switch (driverType)
            {
                case BbtWebDriverType.Chrome:
                    {
                        Browser = new ChromeDriver();
                        break;
                    }
                case BbtWebDriverType.Firefox:
                    {
                        Browser = new FirefoxDriver();
                        break;
                    }
                case BbtWebDriverType.Edge:
                    {
                        Browser = new EdgeDriver();
                        break;
                    }
                default:
                    throw new NotSupportedException($"{nameof(driverType)} {driverType}");
            }
        }

        private By GetLocator(IBbtWebElement webElement)
        {
            var message = $"{nameof(webElement.Type)} {webElement.Type}";
            Debugger.Log(0, "GetLocator", message);
            return webElement.Type switch
            {
                BbtByType.Id => By.Id(webElement.Key),
                BbtByType.LinkText => By.LinkText(webElement.Key),
                BbtByType.Name => By.Name(webElement.Key),
                BbtByType.PartialLinkText => By.PartialLinkText(webElement.Key),
                BbtByType.Tag => By.TagName(webElement.Key),
                BbtByType.XPath => By.XPath(webElement.Key),
                BbtByType.CssSelector => By.CssSelector(webElement.Key),
                _ => throw new ArgumentOutOfRangeException(message),
            };
        }

        private IWebElement FindElement(IBbtWebElement webElement, bool errorIfElementNotFound)
        {
            try
            {
                var locator = GetLocator(webElement);
                Debugger.Log(0, "FindElement", webElement.GetDescription());
                new WebDriverWait(Browser, TimeSpan.FromSeconds(10))
                    .Until(ExpectedConditions.ElementToBeClickable(locator));
                var response = Browser.FindElement(locator);
                if (response == null && errorIfElementNotFound)
                {
                    throw new NotFoundException($"{webElement}");
                }
                return response;
            }
            catch (StaleElementReferenceException)
            {
                return FindElement(webElement, errorIfElementNotFound);
            }
            catch (WebDriverException ex)
            {
                HandleWebDriverException(ex, webElement, errorIfElementNotFound);
                return null;
            }
        }

        public IEnumerable<IWebElement> FindElements(IBbtWebElement webElement, bool errorIfElementNotFound)
        {
            try
            {
                var locator = GetLocator(webElement);
                var message = $"{nameof(webElement.Type)} {webElement.Type}";
                Debugger.Log(0, "FindElements", message);
                var response = Browser.FindElements(locator);
                if ((response == null) && errorIfElementNotFound)
                {
                    throw new NotFoundException($"Could not find {webElement.Type} {webElement.Key}");
                }
                return response;
            }
            catch (WebDriverException ex) { HandleWebDriverException(ex, webElement, errorIfElementNotFound); return null; }
        }

        public void AcceptAlert()
        {
            WaitForAlert(TimeSpan.FromSeconds(2));
            try
            {
                Browser.SwitchTo()?.Alert()?.Accept();
            }
            catch (NoAlertPresentException) { }
        }

        public void FindAndClear(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            response?.Clear();
        }

        public void FindAndClick(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var timeout = DateTime.Now.AddSeconds(10);

            FindAndClick(webElement, timeout, errorIfElementNotFound);
        }

        private void FindAndClick(IBbtWebElement webElement, DateTime timeout, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);

            try
            {
                response?.Click();
            }
            catch (StaleElementReferenceException)
            {
                if (DateTime.Now < timeout)
                {
                    FindAndClick(webElement, timeout, errorIfElementNotFound);
                }
            }
            catch (WebDriverException webDriverException)
            {
                Console.WriteLine(webDriverException.Message);
                if (DateTime.Now < timeout)
                {
                    FindAndClick(webElement, timeout, errorIfElementNotFound);
                }
                else if (errorIfElementNotFound)
                {
                    throw webDriverException;
                }
            }
        }

        public void FindAndCheck(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            var action = new Actions(Browser);
            action.MoveToElement(response).Click().Perform();
        }

        public string FindAndGetAttribute(IBbtWebElement webElement, string key, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            return response?.GetAttribute(key);
        }

        public IEnumerable<string> FindAndGetAttributes(IBbtWebElement webElement, string key, bool errorIfElementNotFound = true)
        {
            var elements = FindElements(webElement, errorIfElementNotFound);
            var response = new List<string>();
            foreach (var element in elements)
            {
                response.Add(element.GetAttribute(key));
            }
            return response;
        }

        public int? FindAndGetCount(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElements(webElement, errorIfElementNotFound);
            return response?.Count();
        }

        public bool? FindAndGetDisplayed(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            try
            {
                var response = FindElement(webElement, errorIfElementNotFound);
                return response?.Displayed;
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public IEnumerable<bool?> FindAndGetDisplayeds(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var elements = FindElements(webElement, errorIfElementNotFound);
            var response = new List<bool?>();
            foreach (var element in elements)
            {
                response.Add(element?.Displayed);
            }
            return response;
        }

        public string FindAndGetLink(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            return response?.GetAttribute("href");
        }

        public IEnumerable<string> FindAndGetLinks(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var elements = FindElements(webElement, errorIfElementNotFound);
            var response = new List<string>();
            foreach (var element in elements)
            {
                response.Add(element?.GetAttribute("href"));
            }
            return response;
        }

        public string FindAndGetOnClick(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            return response?.GetAttribute("onclick");
        }

        public IEnumerable<string> FindAndGetOnClicks(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var elements = FindElements(webElement, errorIfElementNotFound);
            var response = new List<string>();
            foreach (var element in elements)
            {
                response.Add(element?.GetAttribute("onclick"));
            }
            return response;
        }

        public IEnumerable<string> FindAndGetOptions(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            return response?.FindElements(By.TagName("option")).Select(a => a.Text);
        }

        public bool? FindAndGetSelected(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            return response?.Selected;
        }

        public string FindAndGetText(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            return response?.Text;
        }

        public IEnumerable<string> FindAndGetTexts(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var elements = FindElements(webElement, errorIfElementNotFound);
            var response = new List<string>();
            foreach (var element in elements)
            {
                response.Add(element?.Text);
            }
            return response;
        }

        public string FindAndGetValue(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            return response?.GetAttribute("value");
        }

        public IEnumerable<string> FindAndGetValues(IBbtWebElement webElement, string key, bool errorIfElementNotFound = true)
        {
            var elements = FindElements(webElement, errorIfElementNotFound);
            var response = new List<string>();
            foreach (IWebElement x in elements)
            {
                response.Add(x?.GetAttribute(key));
            }
            return response;
        }

        public void FindAndScrollToElement(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            ((IJavaScriptExecutor)Browser)
                .ExecuteScript($"window.scrollTo(0, {response.Location.Y + 150})");
        }

        public void FindAndSendKeys(IBbtWebElement webElement, string keys, bool errorIfNull = true)
        {
            var response = FindElement(webElement, errorIfNull);
            response?.SendKeys(keys);
        }

        public void FindClearAndSendKeys(IBbtWebElement by, string keys, bool errorIfNull = true)
        {
            FindAndClear(by, errorIfNull);
            FindAndSendKeys(by, keys, errorIfNull);
        }

        public void FindAndSubmit(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            response?.Submit();
        }

        public IQueryable<string> FindAndGetSelectOptions(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            var elements = FindElements(webElement, errorIfElementNotFound);
            var response = new List<string>();
            foreach (IWebElement element in elements)
            {
                response.Add(element.FindElement(By.XPath("")).Text);
            }
            return response.AsQueryable<string>();
        }

        public void FindAndSelectIndex(IBbtWebElement webElement, int index, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            var select = new SelectElement(response);
            var matchingOptionsCount = select.Options.Count;
            if (matchingOptionsCount < index + 1 && errorIfElementNotFound)
            {
                throw new InvalidOperationException($"The option {nameof(index)} {index} does not exist in {webElement}");
            }

            if (matchingOptionsCount > index + 1)
            {
                select.SelectByIndex(index);
            }
        }

        public void FindAndSelectText(IBbtWebElement webElement, string text, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, errorIfElementNotFound);
            SelectElement select;
            try
            {
                select = new SelectElement(response);
            }
            catch (StaleElementReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                select = new SelectElement(response);
            }

            var matchingOptions = select.Options.Where(a => a.Text == text);

            if (!matchingOptions.Any() && errorIfElementNotFound)
            {
                throw new InvalidOperationException($"The option {nameof(text)} {text} does not exist in {webElement}");
            }

            if (matchingOptions.Any())
            {
                select.SelectByText(text);
            }
        }

        public void FindAndSelectValue(IBbtWebElement webElement, string value, bool errorIfElementNotFound = true)
        {
            var response = FindElement(webElement, true);
            var select = new SelectElement(response);
            var matchingOptions = select.Options
                .Where(a => a.TagName == value);

            if (!matchingOptions.Any())
            {
                throw new InvalidOperationException($"The option {nameof(value)} {value} does not exist in {webElement}");
            }

            select.SelectByValue(value);
        }

        public void FindAndWaitToBeClickable(IBbtWebElement webElement, TimeSpan timeout, bool errorIfElementNotFound = true)
        {
            var locator = GetLocator(webElement);
            var webWait = new WebDriverWait(Browser, timeout);
            try
            {
                webWait.Until(ExpectedConditions.ElementToBeClickable(locator));
            }
            catch (StaleElementReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                webWait.Until(ExpectedConditions.ElementToBeClickable(locator));
            }
        }

        public void FindAndWaitForElementExists(IBbtWebElement webElement, TimeSpan timeout, bool errorIfElementNotFound = true)
        {
            var locator = GetLocator(webElement);
            var webWait = new WebDriverWait(Browser, timeout);
            webWait.Until(ExpectedConditions.ElementExists(locator));
        }

        public string GetUrl()
        {
            return Browser.Url;
        }

        public string GetHtml()
        {
            return Browser?.PageSource;
        }

        public void Close()
        {
            var windowCount = Browser.WindowHandles.Count();
            for (int counter = 0; counter < windowCount; counter++)
            {
                Browser.Close();
            }
        }
        public void NavigateAndRefresh()
        {
            Browser?.Navigate()?.Refresh();
        }

        public void SwitchToLastWindow()
        {
            Browser?.SwitchTo()?.Window(Browser.WindowHandles.Last());
        }

        public void ScrollToBottomOfPage()
        {
            ((IJavaScriptExecutor)Browser)
                .ExecuteScript("window.scrollTo(0, document.body.scrollHeight - 150)");
        }

        public void NavigateToUrl(string link)
        {
            Browser.Navigate().GoToUrl(link);
        }

        public void HoverOverElement(IBbtWebElement webElement)
        {
            var action = new Actions(Browser);
            action.MoveToElement(Browser.FindElement(GetLocator(webElement))).Perform();
        }


        public void ExecuteScript(string script)
        {
            ((IJavaScriptExecutor)Browser).ExecuteScript(script);
        }

        public IBbtWebElement ChildByXPath(IBbtWebElement parent, string relativeXPath)
        {
            return new BbtWebElement(BbtByType.XPath, $"//*[@id=\"{parent.Key}\"]{relativeXPath}");
        }

        public IBbtWebElement ById(string id, IBbtWebElement child = null)
        {
            return new BbtWebElement(BbtByType.Id, id, child);
        }

        public IBbtWebElement ByLinkText(string linkText)
        {
            return new BbtWebElement(BbtByType.LinkText, linkText);
        }

        public IBbtWebElement ByName(string name)
        {
            return new BbtWebElement(BbtByType.Name, name);
        }

        public IBbtWebElement ByPartialLinkText(string partialLinkText)
        {
            return new BbtWebElement(BbtByType.PartialLinkText, partialLinkText);
        }

        public IBbtWebElement ByTag(string tag)
        {
            return new BbtWebElement(BbtByType.Tag, tag);
        }

        public IBbtWebElement ByXPath(string linkText)
        {
            return new BbtWebElement(BbtByType.XPath, linkText);
        }

        public IBbtWebElement ByCssSelector(string cssSelector)
        {
            return new BbtWebElement(BbtByType.CssSelector, cssSelector);
        }

        public void WaitForAlert(TimeSpan timeOut)
        {
            var wait = new WebDriverWait(Browser, timeOut);
            try
            {
                wait.Until(ExpectedConditions.AlertIsPresent());
            }
            catch (WebDriverTimeoutException) { }
        }

        private void HandleWebDriverException(WebDriverException ex, IBbtWebElement AutoIntegrateWebElement, bool errorIfElementNotFound)
        {
            try
            {
                throw ex;
            }
            catch (OpenQA.Selenium.NoSuchWindowException)
            {
                throw new Exceptions.NoSuchWindowException("The window has been closed");
            }
            catch (NotFoundException)
            {
                if (errorIfElementNotFound)
                {
                    throw new ElementNotFoundException(this, AutoIntegrateWebElement);
                }
            }
        }

        public bool MaximiseWindow()
        {
            var window = Browser.Manage().Window;

            try
            {
                window?.Maximize();
                return true;
            }
            catch (InvalidOperationException ex)
            when (ex.Message.Contains("failed to change window state to maximized, current state is normal"))
            {
                return false;
            }
        }

        public void Dispose()
        {
            Browser?.Quit();
        }

        public IQueryable<IQueryable<string>> GetTableData(IBbtWebElement webElement, bool errorIfElementNotFound = true)
        {
            FindAndWaitToBeClickable(webElement, TimeSpan.FromSeconds(5), true);
            Thread.Sleep(5000);
            var tableElement = FindElement(webElement, errorIfElementNotFound);
            var tableRowElements = tableElement.FindElements(By.TagName("tr"));
            var tabularData = new List<IQueryable<string>>();
            foreach (var tableRowElement in tableRowElements)
            {
                var tableDataElements = tableRowElement.FindElements(By.TagName("td")); // TODO: Do something about StaleElementReferenceExceptions here.
                var listOfData = new List<string>();
                foreach (var tableDataElement in tableDataElements)
                {
                    listOfData.Add(tableDataElement.Text);
                }
                tabularData.Add(listOfData.AsQueryable());
            }
            return tabularData.AsQueryable();
        }

        public bool IsElementPresent(IBbtWebElement webElement, int timeoutSeconds)
        {
            try
            {
                FindAndWaitToBeClickable(webElement, TimeSpan.FromSeconds(timeoutSeconds), true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SwitchToTab(int tabNumber)
        {
            var tab = Browser.WindowHandles[tabNumber];
            Browser.SwitchTo().Window(tab);
        }

        public void SwitchToIFrame(string iFrameId)
        {
            Browser.SwitchTo().Frame(iFrameId);
        }

        public void SwitchToDefaultContent()
        {
            Browser.SwitchTo().DefaultContent();
        }

        public void NavigateBackAPage()
        {
            Browser.Navigate().Back();
        }

        public void CloseTab()
        {
            Browser.Close();
        }
    }
}
