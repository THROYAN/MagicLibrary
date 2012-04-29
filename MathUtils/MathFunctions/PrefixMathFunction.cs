using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.MathFunctions
{
    public class PrefixMathFunction : MathFunction
    {
        public PrefixMathFunction(string name, Func<FunctionElement, FunctionElement> func, string formatString = "") :
            base(name, 1, d => func(d[0]), formatString)
        {
            if (formatString == "")
            {
                this.ToStringFormat = this.FunctionName + "{0}";
            }
        }
    }
}
