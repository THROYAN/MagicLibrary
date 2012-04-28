using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.MathFunctions
{
    public class MathFunction : IMathFunction
    {
        public MathFunction(string name, Func<double[], double> func, Func<double[], double> reverseFunc, string formatString = "")
        {
            this.FunctionName = name;
            this.Function = func;
            this.ReverseFunction = reverseFunc;
            if (formatString == "")
                this.ToStringFormat = this.FunctionName + "({0})";
            else
                this.ToStringFormat = formatString;
        }

        public string FunctionName { get; protected set; }

        public double Calculate(params double[] parameters)
        {
            return this.Function(parameters);
        }

        public string ToStringFormat { get; protected set; }


        public Func<double[], double> Function { get; protected set; }

        public Func<double[], double> ReverseFunction { get; protected set; }

        public override string ToString()
        {
            return base.ToString();
        }

        public virtual string FormatString(string varName)
        {
            return String.Format(this.ToStringFormat, varName);
        }

        public virtual object Clone()
        {
            return new MathFunction(this.FunctionName, this.Function, this.ReverseFunction, this.ToStringFormat);
        }


        public virtual string FormatStringML(string varName)
        {
            return String.Format(this.ToStringFormat, varName);
        }
    }
}
