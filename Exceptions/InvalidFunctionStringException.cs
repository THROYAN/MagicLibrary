using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.Exceptions
{
    public class InvalidFunctionStringException : Exception
    {
        public InvalidFunctionStringException(string fString) : base(String.Format("Invalid funtion string - '{0}'", fString)) { }
    }
}
