using System;

namespace BlackBoxTests.WebAutomation.Exceptions
{
    public class NoSuchWindowException : Exception
    {
        public NoSuchWindowException() : base() { }
        public NoSuchWindowException(string message) : base(message) { }
        public NoSuchWindowException(string message, Exception innerException) : base(message, innerException) { }
    }
}