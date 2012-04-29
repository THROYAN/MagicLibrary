using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MagicLibrary.MathUtils.Functions
{
    public class StringVariable : FunctionElement
    {
        public override string Name { get { return String.Format("'{0}'", this.Value); } }

        public string Value { get; set; }

        public StringVariable(string value)
        {
            this.Value = value;
            this.MathFunctions = new List<Tuple<MathFunctions.IMathFunction, FunctionElement[]>>();
        }

        public override bool IsConstant()
        {
            int i;
            //return Int32.TryParse(this.Name, out i);
            return false;
        }

        public override FunctionElement Pow(double power)
        {
            throw new NotImplementedException();
        }

        public override FunctionElement SetVariableValue(string name, double value)
        {
            throw new NotImplementedException();
        }

        public override FunctionElement SetVariableValue(string name, FunctionElement value)
        {
            throw new NotImplementedException();
        }

        public override VariablesMulriplication Derivative(string name)
        {
            throw new NotImplementedException();
        }

        public override VariablesMulriplication Derivative()
        {
            throw new NotImplementedException();
        }

        public override double ToDouble()
        {
            //int i;
            //if (Int32.TryParse(this.Name, out i))
            //  return i;
            throw new Exception();
        }

        public override bool HasVariable(string name)
        {
            return false;
        }

        public override object Clone()
        {
            return new StringVariable(this.Value);
        }

        public override bool IsVariableMultiplication()
        {
            return false;
        }

        public override VariablesMulriplication ToVariableMultiplication()
        {
            throw new NotImplementedException();
        }

        public override string ToMathMLShort()
        {
            return this.Name;
        }

        public static Function ParseFromString(string str)
        {
            string mask = "^\\'(?<string>.*)\\'$";
            var m = Regex.Match(str, mask);

            if (!m.Success)
            {
                throw new Exception();
            }

            return new Function(new StringVariable(m.Groups["string"].Value));
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override string ToMathML()
        {
            return this.Name;
        }
    }
}
