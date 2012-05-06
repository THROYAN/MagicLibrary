using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MagicLibrary.MathUtils.MathFunctions;
using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.Functions
{
    [Serializable]
    public class StringVariable : FunctionElement
    {
        public override string Name { get { return String.Format("'{0}'", this.Value); } }

        public string Value { get; set; }

        public StringVariable(string value)
        {
            this.Value = value;
            this.MathFunctions = new List<Tuple<MathFunctions.IMathFunction, FunctionElement[]>>();
        }

        public override bool IsDouble()
        {
            //int i;
            //return Int32.TryParse(this.Name, out i);
            return false;
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
            throw new NotImplementedException();
        }

        public override bool HasVariable(string name)
        {
            return false;
        }

        public override object Clone()
        {
            StringVariable s = new StringVariable(this.Value);
            s.CopyFunctions(this);
            return s;
        }

        public override bool IsVariableMultiplication()
        {
            return false;
        }

        public override VariablesMulriplication ToVariableMultiplication()
        {
            return new VariablesMulriplication(this.Clone() as StringVariable);
        }

        public override string ToMathMLShort()
        {
            return this.Name;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override string ToMathML()
        {
            return this.Name;
        }

        public override bool IsLeaf()
        {
            return true;
        }

        public override FunctionElement ToLeaf()
        {
            return this;
        }

        public static MathOperator StringConcat = new MathOperator("string concat", delegate(FunctionElement e1, FunctionElement e2)
            {
                var s1 = e1.ToLeaf() as StringVariable;
                var s2 = e2.ToLeaf() as StringVariable;
                if (s1 != null && s2 != null)
                {
                    return new Function(new StringVariable(String.Concat(s1.Value, s2.Value)));
                }
                if ((e1.ToLeaf() is Variable && s2 != null) || (e2.ToLeaf() is Variable && s1 != null))
                {
                    var temp = e1.Clone() as FunctionElement;
                    temp.ForceAddFunction("string concat", e2.Clone() as FunctionElement);
                    return temp;
                }

                throw new InvalidMathFunctionParameters();

            }, "\\^");

        public override bool IsConstant()
        {
            return true;
        }

        public override void ParseFromString(string func)
        {
            string mask = "^\\'(?<value>.*)\\'$";
            var m = Regex.Match(func, mask);

            if (!m.Success)
            {
                throw new Exception();
            }

            this.Value = m.Groups["value"].Value;
        }

        public override Dictionary<string, FunctionElement> GetVariablesByConstant(FunctionElement e)
        {
            throw new NotImplementedException();
        }

        public override string[] Variables
        {
            get
            {
                return new string[0];
            }
        }
    }
}
