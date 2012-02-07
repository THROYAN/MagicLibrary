using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.MathFunctions;

namespace MagicLibrary.MathUtils
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
        public Variable(string name, double degree = 1)
        {
            this.Functions = new List<MathFunctions.IMathFunction>();
            this.name = name;
            this.Degree = degree;
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
            if (value.IsConstant())
            {
                return this.SetVariableValue(name, value.ToDouble());
            }

            if (this.Name.Equals(name))
            {
                var temp = value.Clone() as FunctionElement;
                //this.Functions.ForEach(f => temp.Functions.Add(f));
                temp.Degree *= this.Degree;
                return temp;
            }
            return this.Clone() as FunctionElement;
        }

        public override string ToString()
        {
            if (this.Name == "" || this.Degree == 0)
                return "";

            if (this.Degree < 0)
                return String.Format("1/{0}{1}",
                   this.ShowFunctions(this.Name), this.Degree == -1 ? "" : String.Format("^{0}", Math.Abs(this.Degree)));

            return String.Format("{0}{1}",
                this.ShowFunctions(this.Name), this.Degree == 1 ? "" : String.Format("^{0}", this.Degree));
        }

        /// <summary>
        /// Involution variable
        /// </summary>
        /// <param name="degree"></param>
        public override FunctionElement Pow(double degree)
        {
            return new Variable(this.Name, this.Degree * degree);
        }

        public override object Clone()
        {
            return new Variable(this.name, this.Degree);
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
                return
                    ((v.Degree == this.Degree && v.Name.Equals(v.Name))) ||
                        (this.IsConstant() &&
                        (v == null || v.IsConstant()));
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsConstant()
        {
            return this.Degree == 0 || this.Name.Equals("");
        }

        public override VariablesMulriplication Derivative()
        {
            if (this.IsConstant())
                return new VariablesMulriplication(0);
            return new VariablesMulriplication(this.Name, this.Degree - 1, this.Degree);
        }

        public override double ToDouble()
        {
            if (this.IsConstant())
            {
                return this.Calculate(1);
            }
            else
            {
                throw new NotImplementedException();
                //return 0;
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
            return true;
        }

        public override VariablesMulriplication ToVariableMultiplication()
        {
            return new VariablesMulriplication(this);
        }

        public override string ToMathMLShort()
        {
            return this.ShowMLFunctions(this.Name);
        }
    }
}
