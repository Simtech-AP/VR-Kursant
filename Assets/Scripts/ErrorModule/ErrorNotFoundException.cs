using System;

/// <summary>
/// Error specific exception
/// </summary>
public class ErrorOperationException : Exception
{
    /// <summary>
    /// Constructs object with message
    /// </summary>
    /// <param name="message">Message to specifiy error with</param>
    public ErrorOperationException(string message)
        : base(message)
    { }
}
