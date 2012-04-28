using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.MathFunctions
{
    public interface IMathFunction : ICloneable
    {
        string FunctionName { get; }
        double Calculate(params double[] d);
        Func<double[], double> Function { get; }
        Func<double[], double> ReverseFunction { get; }
        string ToStringFormat { get; }
        string FormatString(string varName);
        string FormatStringML(string varName);
    }
}
