using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.Exceptions
{
    public class InvalidMathFunctionParameters : Exception
    {
        public InvalidMathFunctionParameters() : base() { }
        public InvalidMathFunctionParameters(string funcName) : base(String.Format("Invalid parameters for function '{0}'", funcName)) { }
    }
}
