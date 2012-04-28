using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.MathFunctions
{
    public class PowerFunction : MathFunction
    {
        private PowerFunction(string name, Func<double[], double> func, Func<double[], double> reverseFunc, string formatString)
            : base(name, func, reverseFunc, formatString) { }

        public PowerFunction(double value)
            : this("power", null, null, "{0}^{1}")
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
            return new PowerFunction(this.Value);
        }
    }
}
