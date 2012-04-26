using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.Exceptions
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException() : base() { }
        public InvalidTokenException(string value, string colorName)
            : base(String.Format("Token '{0}' has invalid format for the '{1}' color", value, colorName))
        { }
    }
}
