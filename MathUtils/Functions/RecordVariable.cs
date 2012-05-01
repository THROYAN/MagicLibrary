using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.PetriNetsUtils;
using System.Text.RegularExpressions;

namespace MagicLibrary.MathUtils.Functions
{
    public class RecordVariable : FunctionElement
    {
        public Dictionary<string, FunctionElement> Values { get; private set; }
        private string _value;

        public RecordVariable(string func)
        {
            this._value = func;
            this.Values = new Dictionary<string, FunctionElement>();
            this.MathFunctions = new List<Tuple<MathFunctions.IMathFunction, FunctionElement[]>>();

            this.ParseFromString(func);
        }

        public override string Name
        {
            get { return this._value; }
        }

        public override bool IsDouble()
        {
            return false;
        }

        public override bool IsConstant()
        {
            foreach (var item in this.Values)
            {
                if (!item.Value.IsConstant())
                {
                    return false;
                }
            }
            return true;
        }

        public override FunctionElement Pow(double power)
        {
            throw new NotImplementedException();
        }

        public override FunctionElement SetVariableValue(string name, double value)
        {
            return this.SetVariableValue(name, new Function(value));
        }

        public override FunctionElement SetVariableValue(string name, FunctionElement value)
        {
            var r = this.Clone() as RecordVariable;
            foreach (var item in this.Values)
            {
                r.Values[item.Key] = item.Value.SetVariableValue(name, value);
            }
            return r;
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
            throw new NotImplementedException();
        }

        public override bool HasVariable(string name)
        {
            foreach (var item in this.Values)
            {
                if (item.Value.HasVariable(name))
                {
                    return true;
                }
            }
            return false;
        }

        public override object Clone()
        {
            RecordVariable r = new RecordVariable(this._value);
            r.CopyFunctions(this);
            return r;
        }

        public override bool IsVariableMultiplication()
        {
            return true;
        }

        public override VariablesMulriplication ToVariableMultiplication()
        {
            return new VariablesMulriplication(this.Clone() as FunctionElement);
        }

        public override string ToMathMLShort()
        {
            return this.ToString();
        }

        public override bool IsLeaf()
        {
            return true;
        }

        public override FunctionElement ToLeaf()
        {
            return this.Clone() as FunctionElement;
        }

        public override void ParseFromString(string func)
        {
            var m = Regex.Match(func, @"^\s*\{(?<body>.*)\}\s*$");

            if (!m.Success)
            {
                throw new Exception();
            }

            var parsedToken = Function.ParseAttributes(m.Groups["body"].Value);

            foreach (var item in parsedToken)
            {
                this.Values[item.Key] = new Function(item.Value);
            }
        }
    }
}
