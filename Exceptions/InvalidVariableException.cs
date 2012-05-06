using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.Exceptions
{
    public class InvalidVariableException : Exception
    {
        public InvalidVariableException(string varName, string colorName) : base(String.Format("Invalid variable '{0}' of '{1}' color.", varName, colorName)) { }
    }
}
