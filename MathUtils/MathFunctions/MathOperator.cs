using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.MathFunctions
{
    public class MathOperator : MathFunction
    {
        public MathOperator(string name, Func<FunctionElement, FunctionElement, FunctionElement> func,
            string operatorString, bool isNeedBracers = false)
            : base(name, 2, d => func(d[0], d[1]), String.Format("{{0}}{0}{{1}}", operatorString), isNeedBracers)
        { }

        public Func<FunctionElement, FunctionElement, FunctionElement> ReverseOperator { set { this.ReverseFunction = (d) => value(d[0], d[1]); } }

        public FunctionElement Calculate(FunctionElement d1, FunctionElement d2)
        {
            return base.Calculate(d1, d2);
        }
        public FunctionElement CalculateRevers(FunctionElement d1, FunctionElement d2)
        {
            return base.CalculateReverse(d1, d2);
        }
    }
}
