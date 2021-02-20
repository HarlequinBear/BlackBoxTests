namespace BlackBoxTests.WebAutomation
{
    public class BbtWebElement : IBbtWebElement
    {
        public string Key { get; }
        public BbtByType Type { get; }
        public IBbtWebElement Child { get; }

        public BbtWebElement(BbtByType type, string key, IBbtWebElement child = null)
        {
            Type = type;
            Key = key;
            Child = child;
        }

        public string GetDescription()
        {
            return $"{Type} {Key}";
        }
    }
}