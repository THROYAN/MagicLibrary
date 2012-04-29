using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.MathFunctions
{
    public interface IMathFunction : ICloneable
    {
        string FunctionName { get; }
        FunctionElement Calculate(params FunctionElement[] d);
        Func<FunctionElement[], FunctionElement> Function { get; }
        IMathFunction ReverseFunction { get; }
        string ToStringFormat { get; }
        string ToStringML(string varName, FunctionElement[] _params);
        string ToString(string name, params FunctionElement[] _params);
        int ParamsCount { get; set; }
    }
}
