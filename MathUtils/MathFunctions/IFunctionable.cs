using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.MathFunctions
{
    interface IFunctionable
    {
        IMathFunction[] Functions { get; set; }

        IFunctionable Calculate();

        IFunctionable SetFunction(IMathFunction func);
    }
}
