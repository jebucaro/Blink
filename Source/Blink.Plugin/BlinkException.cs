using System;

namespace Blink.Plugin
{
    public class BlinkException : Exception
    {
        public BlinkException(string message) : base(message) {}
        public BlinkException(string message, Exception innerException) : base(message, innerException) { }
    }
}