using System;

/// <summary>Thrown to indicate that a bundle is of the incorrect type for the current context</summary>
public class BundleTypeException : Exception
{
    /// <summary>Create an exception with a default message</summary>
    public BundleTypeException() { }

    /// <summary>Create an exception with a specified message</summary>
    public BundleTypeException(string message) : base(message) { }

    /// <summary>Create an exception with a specified message and inner (causal) exception</summary>
    public BundleTypeException(string message, Exception inner) : base(message, inner) { }
}