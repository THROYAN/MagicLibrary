using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.MathFunctions
{
    public class SliceFunction : MathFunction
    {
        private SliceFunction(string name, Func<double[], double> func, Func<double[], double> reverseFunc, string formatString)
            : base(name, func, reverseFunc, formatString) { }

        public SliceFunction()
            : this("slice", d => d[0] > 0 ? d[0] : 0, null, "({0})[+]")
        {

        }
    }
}
