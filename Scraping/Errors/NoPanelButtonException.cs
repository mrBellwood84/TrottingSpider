namespace Scraping.Errors;

public class NoPanelButtonException : Exception
{
    public NoPanelButtonException() {}
    public NoPanelButtonException(string message) : base(message) {}
    public NoPanelButtonException(string message, Exception inner) : base(message, inner) {}
}