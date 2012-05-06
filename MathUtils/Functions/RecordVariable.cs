using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.PetriNetsUtils;
using System.Text.RegularExpressions;
using MagicLibrary.MathUtils.MathFunctions;
using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.Functions
{
    [Serializable]
    public class RecordVariable : FunctionElement
    {
        public Dictionary<string, FunctionElement> Values { get; private set; }

        public RecordVariable(string func)
        {
            this.Values = new Dictionary<string, FunctionElement>();
            this.MathFunctions = new List<Tuple<MathFunctions.IMathFunction, FunctionElement[]>>();

            this.ParseFromString(func);
        }

        public override string[] Variables
        {
            get
            {
                List<string> vars = new List<string>();
                foreach (var item in this.Values)
                {
                    var vars2 = item.Value.Variables;
                    foreach (var item2 in vars2)
                    {
                        if (!vars.Contains(item2))
                        {
                            vars.Add(item2);
                        }
                    }
                }

                return vars.ToArray();
            }
        }

        public override string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                
                foreach (var value in this.Values)
                {
                    if (this.Values.ContainsKey(Function.KeyOfIndex(1)))
                    {
                        sb.AppendFormat("{0}, ", value.Value.ToString());
                    }
                    else
                    {
                        sb.AppendFormat("{0} = {1}, ", value.Key, value.Value.ToString());
                    }
                }
                sb = sb.Remove(sb.Length - 2, 2);

                return String.Format("{1} {0} {2}", sb.ToString(), "{", "}");
            }
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
            RecordVariable r = new RecordVariable(this.ToString());
            //r.CopyFunctions(this);
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
                throw new InvalidFunctionStringException(func);
            }

            var parsedToken = Function.ParseAttributes(m.Groups["body"].Value);

            foreach (var item in parsedToken)
            {
                this.Values[item.Key] = new Function(item.Value);
            }
        }

        public static MathOperator RecordField = new MathOperator("record field", delegate(FunctionElement e1, FunctionElement e2)
            {
                var r = e1.ToLeaf();
                var s = e2.ToLeaf() as StringVariable;

                if (r == null || !(r is RecordVariable) || s == null)
                {
                    if (r is Variable && s != null)
                    {
                        var temp = new Function(e1);
                        temp.ForceAddFunction("record field", e2);
                        return temp;
                    }
                    throw new InvalidMathFunctionParameters();
                }

                var r1 = r as RecordVariable;
                if (!r1.Values.ContainsKey(s.Value))
                {
                    throw new InvalidMathFunctionParameters();
                }

                return new Function(r1.Values[s.Value]);
                
            }, "\\.");

        public override Dictionary<string, FunctionElement> GetVariablesByConstant(FunctionElement e)
        {
            var r = e.ToLeaf() as RecordVariable;

            if (r == null || r.Values.Count != this.Values.Count)
            {
                return null;
            }

            Dictionary<string, FunctionElement> d = new Dictionary<string, FunctionElement>();

            for (int i = 0; i < this.Values.Count; i++)
            {
                //var f = this.Values.ElementAt(i).Value.ToLeaf();
                if (!this.Values.ElementAt(i).Value.IsConstant())
                {
                    //if (f is Variable)
                    //{
                    //    d[f.ToString()] = r.ApplyFunction("record field", new StringVariable(r.Values.ElementAt(i).Key));
                    //}
                    //else
                    {
                        var d1 = this.Values.ElementAt(i).Value.GetVariablesByConstant(r.Values.ElementAt(i).Value);
                        if (d1 == null)
                        {
                            return null;
                        }

                        foreach (var item in d1)
                        {
                            d[item.Key] = item.Value;
                        }
                    }
                }
                else
                {
                    if (!this.Values.ElementAt(i).Value.Equals(r.Values.ElementAt(i).Value))
                    {
                        return null;
                    }
                }
            }
            return d;
        }
    }
}
