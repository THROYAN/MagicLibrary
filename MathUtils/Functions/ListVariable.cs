using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicLibrary.Exceptions;
using System.Text.RegularExpressions;
using MagicLibrary.MathUtils.MathFunctions;

namespace MagicLibrary.MathUtils.Functions
{
    [Serializable]
    public class ListVariable : FunctionElement
    {
        public const string OpenBracer = "[";
        public const string CloseBracer = "]";
        public FunctionElement[] Values { get; private set; }
        
        public ListVariable(FunctionElement[] values)
            : base()
        {
            this.MathFunctions = new List<Tuple<MathFunctions.IMathFunction, FunctionElement[]>>();
            this.Values = values;
        }

        public ListVariable(string func)
        {
            this.MathFunctions = new List<Tuple<MathFunctions.IMathFunction, FunctionElement[]>>();
            this.ParseFromString(func);
        }

        public override string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                string separator = ", ";
                foreach (var v in this.Values)
                {
                    sb.AppendFormat("{0}{1}", v, separator);
                }
                if (sb.Length > 0)
                {
                    sb = sb.Remove(sb.Length - separator.Length, separator.Length);
                }
                return String.Format("{1}{0}{2}", sb.ToString(), ListVariable.OpenBracer, ListVariable.CloseBracer);
            }
        }

        public override void ParseFromString(string func)
        {
            var m = Regex.Match(func, String.Format(@"^\s*{0}(?<body>.*){1}\s*$", Regex.Escape(ListVariable.OpenBracer), Regex.Escape(ListVariable.CloseBracer)));

            if (!m.Success)
            {
                throw new InvalidFunctionStringException(func);
            }

            var attrs = Function.ParseAttributes(m.Groups["body"].Value);

            this.Values = new FunctionElement[attrs.Count];

            if (Function.IsArrayAttributes(attrs))
            {
                for (int i = 0; i < attrs.Count; i++)
                {
                    this.Values[i] = new Function(attrs[Function.KeyOfIndex(i + 1)]);
                }
                return;
            }
            throw new InvalidFunctionStringException(func);
        }

        public override bool IsDouble()
        {
            return false;
        }

        public override bool IsConstant()
        {
            foreach (var v in this.Values)
            {
                if (!v.IsConstant())
                {
                    return false;
                }
            }
            return true;
        }

        public override FunctionElement SetVariableValue(string name, double value)
        {
            List<FunctionElement> fs = new List<FunctionElement>();

            foreach (var v in this.Values)
            {
                fs.Add(v.SetVariableValue(name, value));
            }
            return new ListVariable(fs.ToArray());
        }

        public override FunctionElement SetVariableValue(string name, FunctionElement value)
        {
            List<FunctionElement> fs = new List<FunctionElement>();

            foreach (var v in this.Values)
            {
                fs.Add(v.SetVariableValue(name, value));
            }
            return new ListVariable(fs.ToArray());
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
            foreach (var v in this.Values)
            {
                if (v.HasVariable(name))
                {
                    return true;
                }
            }
            return false;
        }

        public override object Clone()
        {
            List<FunctionElement> fs = new List<FunctionElement>();

            foreach (var v in this.Values)
            {
                fs.Add(v.Clone() as FunctionElement);
            }
            return new ListVariable(fs.ToArray());
        }

        public override bool IsVariableMultiplication()
        {
            return false;
        }

        public override VariablesMulriplication ToVariableMultiplication()
        {
            return new VariablesMulriplication(this.Clone() as ListVariable);
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

        public override Dictionary<string, FunctionElement> GetVariablesByConstant(FunctionElement e)
        {
            var l = e.ToLeaf() as ListVariable;

            if (l == null || l.Values.Length != this.Values.Length)
            {
                return null;
            }

            Dictionary<string, FunctionElement> d = new Dictionary<string, FunctionElement>();
            for (int i = 0; i < this.Values.Length; i++)
            {
                if (!this.Values[i].IsConstant())
                {
                    var d1 = this.Values[i].GetVariablesByConstant(l.Values[i]);
                    if (d1 == null)
                    {
                        return null;
                    }
                    foreach (var item in d1)
                    {
                        if (d.ContainsKey(item.Key))
                        {
                            if (!d[item.Key].Equals(item.Value))
                            {
                                return null;
                            }
                        }
                        else
                        {
                            d.Add(item.Key, item.Value);
                        }
                    }
                }
                else
                {
                    if (!this.Values[i].Equals(l.Values[i]))
                    {
                        return null;
                    }
                }
            }
            return d;
        }

        public static MathOperator ListsConcat = new MathOperator("lists concat", delegate(FunctionElement e1, FunctionElement e2)
            {
                var v1 = e1.ToLeaf() as FunctionElement;
                var v2 = e2.ToLeaf() as FunctionElement;

                if (v1 is Variable || v2 is Variable)
                {
                    if ((v1 is Variable && v2 is Variable) || v1 is ListVariable || v2 is ListVariable)
                    {
                        var temp = e1.Clone() as FunctionElement;
                        temp.ForceAddFunction("lists concat", e2.Clone() as FunctionElement);
                        return temp;
                    }
                }

                if (!(v1 is ListVariable))
                {
                    throw new InvalidMathFunctionParameters();
                }

                if (!(v2 is ListVariable) && e2.IsConstant())
                {
                    var l1 = v1 as ListVariable;
                    List<FunctionElement> newList = new List<FunctionElement>(l1.Values.Length + 1);

                    foreach (var item in l1.Values)
                    {
                        newList.Add(item.Clone() as FunctionElement);
                    }
                    newList.Add(e2);
                    return new Function(new ListVariable(newList.ToArray()));
                }

                if (e1.IsConstant() && e2.IsConstant())
                {
                    var l1 = v1 as ListVariable;
                    var l2 = v2 as ListVariable;
                    List<FunctionElement> newList = new List<FunctionElement>(l1.Values.Length + l2.Values.Length);

                    foreach (var item in l1.Values)
                    {
                        newList.Add(item);
                    }

                    foreach (var item in l2.Values)
                    {
                        newList.Add(item);
                    }

                    return new Function(new ListVariable(newList.ToArray()));
                }

                var temp2 = e1.Clone() as FunctionElement;
                temp2.ForceAddFunction("lists concat", e2.Clone() as FunctionElement);
                return temp2;

            }, "\\^\\^", true);

        public static MathOperator ListsSubtract = new MathOperator("lists sub", delegate(FunctionElement e1, FunctionElement e2)
        {
            var v1 = e1.ToLeaf() as FunctionElement;
            var v2 = e2.ToLeaf() as FunctionElement;

            if (v1 is Variable || v2 is Variable)
            {
                if ((v1 is Variable && v2 is Variable) || v1 is ListVariable || v2 is ListVariable)
                {
                    var temp = e1.Clone() as FunctionElement;
                    temp.ForceAddFunction("lists sub", e2.Clone() as FunctionElement);
                    return temp;
                }
            }

            if (!(v1 is ListVariable))
            {
                throw new InvalidMathFunctionParameters();
            }

            if (!(v2 is ListVariable) && e2.IsConstant())
            {
                var l1 = v1 as ListVariable;
                List<FunctionElement> newList = new List<FunctionElement>(l1.Values.Length);

                foreach (var item in l1.Values)
                {
                    newList.Add(item.Clone() as FunctionElement);
                }
                newList.Remove(newList.Find(e => e.Equals(e2)));
                return new Function(new ListVariable(newList.ToArray()));
            }

            if (e1.IsConstant() && e2.IsConstant())
            {
                var l1 = v1 as ListVariable;
                var l2 = v2 as ListVariable;
                List<FunctionElement> newList = new List<FunctionElement>(l1.Values.Length);

                foreach (var item in l1.Values)
                {
                    newList.Add(item);
                }

                foreach (var item in l2.Values)
                {
                    newList.Remove(newList.Find(e => e.Equals(item)));
                }

                return new Function(new ListVariable(newList.ToArray()));
            }

            var temp2 = e1.Clone() as FunctionElement;
            temp2.ForceAddFunction("lists sub", e2.Clone() as FunctionElement);
            return temp2;

        }, "!\\^", true);

        public static MathFunction RemoveAll = new MathFunction("remove all", 2, delegate(FunctionElement[] d)
        {
            var v1 = d[0].ToLeaf();
            var v2 = d[1].ToLeaf();
            if ((v1 is Variable || (v1 is ListVariable && !v1.IsConstant())) && (v2 is Variable || v2.IsConstant()))
            {   
                var temp = d[0].Clone() as FunctionElement;
                temp.ForceAddFunction("remove all", d[1].Clone() as FunctionElement);
                return temp;
            }

            if (!(v1 is ListVariable))
            {
                throw new InvalidMathFunctionParameters();
            }

            if (d[0].IsConstant() && d[1].IsConstant())
            {
                var l = v1 as ListVariable;
                List<FunctionElement> newList = new List<FunctionElement>(l.Values.Length);

                foreach (var item in l.Values)
                {
                    newList.Add(item);
                }

                newList.RemoveAll(e => e.Equals(d[1]));

                return new Function(new ListVariable(newList.ToArray()));
            }

            var temp2 = d[0].Clone() as FunctionElement;
            temp2.ForceAddFunction("remove all", d[1].Clone() as FunctionElement);
            return temp2;

        }, "rmall\\({0},{1}\\)", true);

        public static MathFunction ListLength = new MathFunction("list length", 1, delegate(FunctionElement[] d)
            {
                var e = d[0].ToLeaf();
                if (e is Variable || (e is ListVariable && e.MathFunctions.Count > 0))
                {
                    var temp = d[0].Clone() as FunctionElement;
                    temp.ForceAddFunction("list length");
                    return temp;
                }
                if (e is ListVariable && e.MathFunctions.Count == 0)
                {
                    return new Function((e as ListVariable).Values.Length);
                }

                throw new InvalidMathFunctionParameters();

            }, "length{0}", true);

        public static MathFunction MapFunction = new MathFunction("map func", 2, delegate(FunctionElement[] d)
        {
            var v2 = d[1].ToLeaf();
            if (!(v2 is StringVariable) || Function.GetMathFunction((v2 as StringVariable).Value) == null)
            {
                throw new InvalidMathFunctionParameters();
            }
            var v = d[0].ToLeaf();

            if (v is Variable)
            {
                var temp = d[0].Clone() as FunctionElement;
                temp.ForceAddFunction("map func", d[1].Clone() as FunctionElement);
                return temp;
            }

            if (v is ListVariable)
            {
                if (d[0].IsConstant())
                {
                    var vars = (v as ListVariable).Values;
                    List<FunctionElement> newVars = new List<FunctionElement>(vars.Length);
                    foreach (var var in vars)
                    {
                        newVars.Add(Function.GetMathFunction((v2 as StringVariable).Value).Calculate(var));
                    }
                    return new Function(new ListVariable(newVars.ToArray()));
                }

                var temp = d[0].Clone() as FunctionElement;
                temp.ForceAddFunction("map func", d[1]);
                return temp;
            }

            throw new InvalidMathFunctionParameters();

        }, "map\\({0},{1}\\)");

        public override string[] Variables
        {
            get
            {
                List<string> vars = new List<string>();

                foreach (var item in this.Values)
                {
                    var vars2 = item.Variables;
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
    }
}
