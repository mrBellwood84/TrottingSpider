﻿namespace Scraping.Errors;

public class NoContentException : Exception
{
    public NoContentException() {}
    public NoContentException(string message) : base(message) { }
    public NoContentException(string message, Exception inner) : base(message, inner) { }
}