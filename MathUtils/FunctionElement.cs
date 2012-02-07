using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.MathFunctions;

namespace MagicLibrary.MathUtils
{
    public abstract class FunctionElement: ICloneable
    {
        public virtual double Degree
        {
            get
            {
                if (this.Functions.Count == 0)
                {
                    return 1;
                }
                var pF = this.Functions.Last() as Power;//.Find(f => f is Power);
                if (pF == null)
                {
                    return 1;
                }
                return (pF as Power).Value;
            }
            set
            {
                if (this.Functions.Count == 0)
                {
                    this.Functions.Add(new Power(value));
                    return;
                }

                var pF = this.Functions.Last() as Power;//.Find(f => f is MathFunctions.Power);
                if (pF == null)
                {
                    this.Functions.Add(new Power(value));
                }
                else
                {
                    (pF as Power).Value = value;
                }
            }
        }

        public abstract string Name { get; }

        public abstract bool IsConstant();

        public abstract FunctionElement Pow(double power);

        public abstract FunctionElement SetVariableValue(string name, double value);

        public abstract FunctionElement SetVariableValue(string name, FunctionElement value);

        public abstract VariablesMulriplication Derivative(string name);

        public abstract VariablesMulriplication Derivative();

        public abstract double ToDouble();

        public abstract bool HasVariable(string name);

        private static Function _add(FunctionElement e1, FunctionElement e2)
        {
            return e1.ToVariableMultiplication() + e2;
        }

        private static Function _add(FunctionElement e, double d)
        {
            return e.ToVariableMultiplication() + d;
        }

        private static Function _mul(FunctionElement e1, FunctionElement e2)
        {
            return e1.ToVariableMultiplication() * e2;
        }

        private static Function _mul(FunctionElement e, double d)
        {
            return e.ToVariableMultiplication() * d;
        }

        private static Function _div(FunctionElement e1, FunctionElement e2)
        {
            return e1.ToVariableMultiplication() / e2;
        }

        private static Function _div(FunctionElement e, double d)
        {
            return e.ToVariableMultiplication() / d;
        }

        private static Function _div(double d, FunctionElement e)
        {
            return new VariablesMulriplication(d) / e;
        }

        #region operator +
        public static Function operator +(FunctionElement e1, FunctionElement e2)
        {
            return FunctionElement._add(e1, e2);
        }

        public static Function operator +(FunctionElement e, double d)
        {
            return FunctionElement._add(e, d);
        }

        public static Function operator +(double d, FunctionElement e)
        {
            return e + d;
        }
        #endregion

        #region operator -
        public static Function operator -(FunctionElement e1, FunctionElement e2)
        {
            return FunctionElement._add(e1, (-1) * e2);
        }

        public static Function operator -(FunctionElement e, double d)
        {
            return FunctionElement._add(e, -d);
        }

        public static Function operator -(double d, FunctionElement e)
        {
            return FunctionElement._add(-1 * e, d);
        }
        #endregion

        #region operator *
        public static Function operator *(FunctionElement e1, FunctionElement e2)
        {
            return FunctionElement._mul(e1, e2);
        }

        public static Function operator *(FunctionElement e, double d)
        {
            return FunctionElement._mul(e, d);
        }

        public static Function operator *(double d, FunctionElement e)
        {
            return FunctionElement._mul(e, d);
        }
        #endregion

        #region operator /
        public static Function operator /(FunctionElement e1, FunctionElement e2)
        {
            return FunctionElement._div(e1, e2);
        }

        public static Function operator /(FunctionElement e, double d)
        {
            return FunctionElement._div(e, d);
        }

        public static Function operator /(double d, FunctionElement e)
        {
            return FunctionElement._div(d, e);
        }
        #endregion

        public abstract object Clone();

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this.Name.Equals((obj as FunctionElement).Name))
            {
                return this.GetType() == obj.GetType();
            }
            return base.Equals(obj);
        }

        public List<IMathFunction> Functions { get; set; }

        public double Calculate(double d)
        {
            double result = d;
            this.Functions.ForEach(f => result = f.Calculate(result));
            return result;
        }

        public string ShowFunctions(string varName = null)
        {
            if (varName == null)
            {
                varName = this.Name;
            }
            if (this.Functions.Count == 0)
            {
                return varName;
            }

            var temp = varName;

            int powers = 0;
            if (this.Functions.Last() is Power)
            {
                powers = 1;
            }

            for (int i = 0; i < this.Functions.Count - powers; i++)
            {
                temp = this.Functions[i].FormatString(temp);
            }

            return temp;
        }

        public FunctionElement AddFunction(IMathFunction func)
        {
            var temp = this.Clone() as FunctionElement;
            temp.Functions.Add(func);
            return temp;
        }

        public abstract bool IsVariableMultiplication();
        public abstract VariablesMulriplication ToVariableMultiplication();

        public virtual string ToMathML()
        {
            string s = this.ToMathMLShort();
            
            Power p = new Power(this.Degree);

            return String.Format("<math xmlns='http://www.w3.org/1998/Math/MathML'><mrow>{0}</mrow></math>", p.FormatStringML(s));
        }
        public abstract string ToMathMLShort();
        public string ShowMLFunctions(string varName = null)
        {
            if (varName == null)
            {
                varName = this.Name;
            }
            if (this.Functions.Count == 0)
            {
                return varName;
            }

            var temp = varName;

            int powers = 0;
            if (this.Functions.Last() is Power)
            {
                powers = 1;
            }

            for (int i = 0; i < this.Functions.Count - powers; i++)
            {
                temp = this.Functions[i].FormatStringML(temp);
            }

            return temp;
        }

        public void OverrideFunctions(FunctionElement e)
        {
            this.Functions.Clear();
            e.Functions.ForEach(f => this.Functions.Add(f.Clone() as IMathFunction));
        }
    }
}
