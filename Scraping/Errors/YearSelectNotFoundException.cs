namespace Scraping.Errors;

public class YearSelectNotFoundException : Exception
{
    public YearSelectNotFoundException() {}
    public YearSelectNotFoundException(string message) : base(message) {}
    public YearSelectNotFoundException(string message, Exception inner) : base(message, inner) {}
}