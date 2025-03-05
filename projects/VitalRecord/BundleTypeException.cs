using System;

public class BundleTypeException : Exception
{
    public BundleTypeException() {}

    public BundleTypeException(string message) : base(message) {}

    public BundleTypeException(string message, Exception inner) : base(message, inner) {}
}