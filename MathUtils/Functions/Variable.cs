using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.MathFunctions;
using System.Text.RegularExpressions;

namespace MagicLibrary.MathUtils.Functions
{
    public class Variable : FunctionElement
    {
        private string name;

        /// <summary>
        /// Identifier of variable.
        /// E.g.: x, y
        /// </summary>
        public override string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of variable</param>
        /// <param name="degree">Power of variable</param>
        public Variable(string name)
        {
            this._initProperties();
            this.name = name;
        }

        public void _initProperties()
        {
            this.MathFunctions = new List<Tuple<IMathFunction, FunctionElement[]>>();
        }

        /// <summary>
        /// Return the result of setting value instead of variable
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override FunctionElement SetVariableValue(string name, double value)
        {
            if (this.Name.Equals(name))
            {
                return new Variable("") * this.Calculate(value);//Math.Pow(value, this.Degree));
            }
            return this.Clone() as FunctionElement;
        }

        public override FunctionElement SetVariableValue(string name, FunctionElement value)
        {
            if (value.IsDouble())
            {
                return this.SetVariableValue(name, value.ToDouble());
            }

            if (this.Name.Equals(name))
            {
                var temp = value.Clone() as FunctionElement;

                temp.ForceAddFunctions(this);

                return temp;
            }
            return this.Clone() as FunctionElement;
        }

        public override string ToString()
        {
            if (this.Name == "")
                return "";

            return base.ToString();
        }

        /// <summary>
        /// Involution variable
        /// </summary>
        /// <param name="degree"></param>
        public override FunctionElement Pow(double degree)
        {
            return this.ApplyFunction("power", new Function(degree));
        }

        public override object Clone()
        {
            Variable v = new Variable(this.name);
            v.CopyFunctions(this);
            return v;
        }
        /*
        public static Function _add(Variable var1, Variable var2)
        {
            List<VariablesMulriplication> vars = new List<VariablesMulriplication>();
            vars.Add(new VariablesMulriplication(var1.Clone() as Variable));
            if (var1.Name == var2.Name && var1.Degree == var2.Degree)
            {
                vars[0].Constant = 2;
            }
            else
            {
                vars.Add(new VariablesMulriplication(var2.Clone() as Variable));
            }
            return new Function(vars.ToArray());
        }

        public static Function _add(Variable v, double d)
        {
            return new VariablesMulriplication(v) + d;
        }

        #region operator +
        public static Function operator +(Variable var1, Variable var2)
        {
            return Variable._add(var1, var2);
        }

        public static Function operator +(Variable v, double d)
        {
            return Variable._add(v, d);
        }

        public static Function operator +(double d, Variable v)
        {
            return v + d;
        }
        #endregion

        #region operator -
        public static Function operator -(Variable var1, Variable var2)
        {
            return Variable._add(var1, -1 * var2);
            if (var1.Name == var2.Name && var1.Degree == var2.Degree)
            {
                return new Function();
            }
            return new Function(new VariablesMulriplication[]{
                new VariablesMulriplication(var1.Clone() as Variable),
                new VariablesMulriplication(var2.Clone() as Variable)
            });
        }

        public static Function operator -(Variable v, double d)
        {
            return v + -d;
        }

        public static Function operator -(double d, Variable v)
        {
            return -1 * v + d;
        }
        #endregion

        #region operator *
        public static Function operator *(Variable var1, Variable var2)
        {
            return new VariablesMulriplication(var1) * new VariablesMulriplication(var2);
        }

        public static Function operator *(Variable var, double d)
        {
            return new VariablesMulriplication(var) * d;
        }

        public static Function operator *(double d, Variable var)
        {
            return new VariablesMulriplication(var) * d;
        }
        #endregion

        #region operator /
        public static Function operator /(Variable var1, Variable var2)
        {
            return new VariablesMulriplication(var1) / var2;
        }

        public static Function operator /(Variable var, double d)
        {
            return new VariablesMulriplication(var) / d;
        }

        public static Function operator /(double d, Variable var)
        {
            return new VariablesMulriplication(d) / var;
        }
        #endregion
        */

        public override bool Equals(object obj)
        {
            if (obj is Variable)
            {
                var v = obj as Variable;
                
                return v.Name.Equals(this.Name) && this.SameMathFunctionsWith(v);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsDouble()
        {
            return this.Name.Equals("");
        }

        public override VariablesMulriplication Derivative()
        {
            if (this.IsDouble())
                return new VariablesMulriplication(0);
            throw new Exception();
#warning варнинг
            //return new VariablesMulriplication(this.Name, this.Degree - 1, this.Degree);
        }

        public override double ToDouble()
        {
            if (this.IsDouble())
            {
                return this.Calculate(1);
            }
            else
            {
                throw new Exception();
            }
        }

        public override VariablesMulriplication Derivative(string name)
        {
            if (this.Name.Equals(name))
                return this.Derivative();
            return new VariablesMulriplication(0);
        }

        public override bool HasVariable(string name)
        {
            return this.Name.Equals(name);
        }

        public override bool IsVariableMultiplication()
        {
            return this.MathFunctions.Count == 0;
        }

        public override VariablesMulriplication ToVariableMultiplication()
        {
            return new VariablesMulriplication(this);
        }

        public override string ToMathMLShort()
        {
            return this.ShowMLFunctions(this.Name);
        }

        public override void ParseFromString(string func)
        {
            var mask = @"^(?<var>([A-Z]|[a-z])([A-Z]|[a-z]|[0-9])*)$";
            var m = Regex.Match(func, mask, RegexOptions.IgnorePatternWhitespace);
            if (!m.Success)
            {
                throw new Exception();
            }
            this.name = m.Groups["var"].Value;
        }

        public override bool IsLeaf()
        {
            return true;
        }

        public override FunctionElement ToLeaf()
        {
            return this;
        }

        public override bool IsConstant()
        {
            return this.Name.Equals("");
        }
    }
}
