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

    public class Power : MathFunction
    {
        private Power(string name, Func<double[], double> func, Func<double[], double> reverseFunc, string formatString)
            : base(name, func, reverseFunc, formatString) { }
        
        public Power(double value):this("power", null, null, "{0}^{1}")
        {
            this.Function = d => Math.Pow(d[0], this.Value);
            this.ReverseFunction = d => Math.Pow(d[0], -this.Value);
            this.Value = value;
        }

        public double Value
        {
            get { return this.power; }
            set
            {
                this.power = value;
            }
        }

        private double power;

        public override string FormatString(string varName)
        {
            string format = "{0}^" + this.Value.ToString();
            if (this.Value == 1)
            {
                format = "{0}";
            }
            return String.Format(format, varName);
        }

        public override string FormatStringML(string varName)
        {
            string format = this.FormatPower();
            if (this.Value < 0)
            {
                format = String.Format("<mrow><mfrac><mi>1</mi><mrow><mi>{0}</mi></mrow></mfrac></mrow>", format);
            }
            return String.Format(format, varName);
        }

        public string FormatPower()
        {
            double d = Math.Abs(this.Value);
            if (d == 1)
            {
                return "<mrow>{0}</mrow>";
            }

            double d2 = 1.0 / d;
            if (Math.Truncate(d2) == d2)
            {
                if (d2 == 2)
                {
                    return "<msqrt><mrow>{0}</mrow></msqrt>";
                }
                return String.Format("<mroot><mrow>{0}</mrow><mn>{1}</mn></mroot>", "{0}", d2);
            }   
            return String.Format("<msup><mrow>{0}</mrow><mn>{1}</mn></msup>", "{0}", d);
        }

        public bool IsNeedBrackets()
        {
            var d = Math.Abs(this.Value);
            if (d == 1)
            {
                return false;
            }

            double d2 = 1.0 / d;
            if (Math.Truncate(d2) == d2 && d2 == 2)
            {
                return false;
            }
            return true;
        }

        public override object Clone()
        {
            return new Power(this.Value);
        }
    }

    public class FunctionSlice : MathFunction
    {
        private FunctionSlice(string name, Func<double[], double> func, Func<double[], double> reverseFunc, string formatString)
            : base(name, func, reverseFunc, formatString) { }

        public FunctionSlice()
            : this("slice", d => d[0] > 0 ? d[0] : 0, null, "({0})[+]")
        {

        }
    }
}
