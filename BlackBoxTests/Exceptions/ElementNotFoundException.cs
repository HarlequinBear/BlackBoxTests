using System;
using System.Text;

namespace BlackBoxTests.WebAutomation.Exceptions
{
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(IBbtWebDriver BbtWebDriver, IBbtWebElement BbtWebElement)
            : base(GetMessage(BbtWebDriver, BbtWebElement))
        {

        }

        private static string GetMessage(IBbtWebDriver BbtWebDriver, IBbtWebElement BbtWebElement)
        {
            var builder = new StringBuilder();
            builder.AppendLine("ElementNotFoundException:");
            builder.AppendLine($"Element: {BbtWebElement.GetDescription()}");
            builder.AppendLine($"Html: {BbtWebDriver.GetHtml()}");
            return builder.ToString();
        }
    }
}