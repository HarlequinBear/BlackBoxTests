namespace BlackBoxTests.WebAutomation
{
    public interface IBbtWebElement
    {
        string Key { get; }

        BbtByType Type { get; }

        IBbtWebElement Child { get; }

        string GetDescription();
    }
}