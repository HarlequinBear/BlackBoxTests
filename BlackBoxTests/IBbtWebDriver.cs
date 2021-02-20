using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackBoxTests.WebAutomation
{
    public interface IBbtWebDriver : IDisposable
    {
        IEnumerable<IWebElement> FindElements(IBbtWebElement by, bool errorIfElementNotFound);

        void AcceptAlert();

        void Close();

        void ExecuteScript(string script);

        void FindAndClear(IBbtWebElement by, bool errorIfNull = true);

        void FindAndClick(IBbtWebElement by, bool errorIfNull = true);

        void FindAndCheck(IBbtWebElement by, bool errorIfNull = true);

        string FindAndGetAttribute(IBbtWebElement by, string key, bool errorIfNull = true);

        IEnumerable<string> FindAndGetAttributes(IBbtWebElement by, string key, bool errorIfNull = true);

        int? FindAndGetCount(IBbtWebElement by, bool errorIfNull = true);

        bool? FindAndGetDisplayed(IBbtWebElement by, bool errorIfNull = true);

        IEnumerable<bool?> FindAndGetDisplayeds(IBbtWebElement by, bool errorIfNull = true);

        string FindAndGetLink(IBbtWebElement by, bool errorIfNull = true);

        IEnumerable<string> FindAndGetLinks(IBbtWebElement by, bool errorIfNull = true);

        string FindAndGetOnClick(IBbtWebElement by, bool errorIfNull = true);

        IEnumerable<string> FindAndGetOnClicks(IBbtWebElement by, bool errorIfNull = true);

        IEnumerable<string> FindAndGetOptions(IBbtWebElement by, bool errorIfNull = true);

        bool? FindAndGetSelected(IBbtWebElement by, bool errorIfNull = true);

        string FindAndGetText(IBbtWebElement by, bool errorIfNull = true);

        IEnumerable<string> FindAndGetTexts(IBbtWebElement by, bool errorIfNull = true);

        IQueryable<IQueryable<string>> GetTableData(IBbtWebElement by, bool errorIfElementNotFound = true);

        string FindAndGetValue(IBbtWebElement by, bool errorIfNull = true);

        IEnumerable<string> FindAndGetValues(IBbtWebElement by, string key, bool errorIfNull = true);

        void FindAndScrollToElement(IBbtWebElement by, bool errorIfElementNotFound = true);

        void FindAndSelectIndex(IBbtWebElement by, int index, bool errorIfNull = true);

        void FindAndSelectText(IBbtWebElement by, string text, bool errorIfNull = true);

        void FindAndSelectValue(IBbtWebElement by, string value, bool errorIfNull = true);

        void FindAndSendKeys(IBbtWebElement by, string keys, bool errorIfNull = true);

        void FindAndSubmit(IBbtWebElement by, bool errorIfNull = true);

        void FindAndWaitToBeClickable(IBbtWebElement by, TimeSpan timeOut, bool errorIfNull = true);

        void FindAndWaitForElementExists(IBbtWebElement by, TimeSpan timeOut, bool errorIfNull = true);

        string GetHtml();

        string GetUrl();

        bool MaximiseWindow();

        void NavigateToUrl(string link);

        void NavigateAndRefresh();

        void ScrollToBottomOfPage();

        void SwitchToLastWindow();

        void WaitForAlert(TimeSpan timeOut);

        IBbtWebElement ChildByXPath(IBbtWebElement parent, string relativeXPath);

        IBbtWebElement ById(string id, IBbtWebElement child = null);

        IBbtWebElement ByLinkText(string linkText);

        IBbtWebElement ByName(string id);

        IBbtWebElement ByPartialLinkText(string partialLinkText);

        IBbtWebElement ByTag(string tag);
        IBbtWebElement ByXPath(string linkText);

        IBbtWebElement ByCssSelector(string cssSelector);

        bool IsElementPresent(IBbtWebElement webElement, int timeoutSeconds);

        void SwitchToTab(int tabNumber);

        void CloseTab();

        void HoverOverElement(IBbtWebElement performanceMetricsButton);
    }
}