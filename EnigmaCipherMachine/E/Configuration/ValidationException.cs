using System;
using System.Collections.Generic;

namespace Enigma.Configuration
{
    public class ValidationException : Exception
    {
        public ValidationException() : base() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base() { }

        public List<BrokenRule> BrokenRules { get; internal set; }
    }
}
