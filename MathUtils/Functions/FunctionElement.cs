using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.MathFunctions;
using MagicLibrary.Exceptions;

namespace MagicLibrary.MathUtils.Functions
{
    [Serializable]
    public abstract class FunctionElement: ICloneable
    {
        public List<Tuple<IMathFunction, FunctionElement[]>> MathFunctions { get; set; }

        public abstract string Name { get; }

        public abstract string[] Variables { get; }

        protected bool _isModified;

        public abstract void ParseFromString(string func);

        public abstract bool IsDouble();

        public abstract bool IsConstant();

        public FunctionElement Pow(double power)
        {
            return this.ApplyFunction("power", new Function(power));
        }

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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public double Calculate(double d)
        {
            double result = d;
            foreach (var mf in this.MathFunctions)
            {
                FunctionElement[] _params = new FunctionElement[mf.Item2.Length + 1];
                _params[0] = new Function(result);

                for (int i = 0; i < mf.Item2.Length; i++)
                {
                    _params[i + 1] = mf.Item2[i];
                }

                result = mf.Item1.Calculate(_params).ToDouble();
            }

            return result;
        }

        public FunctionElement Recalc()
        {
            var res = this.Clone() as FunctionElement;
            res.MathFunctions.Clear();

            foreach (var mf in this.MathFunctions)
            {
                res = res.ApplyFunction(mf.Item1.FunctionName, mf.Item2);
            }
            return res;
        }

        public bool SameMathFunctionsWith(FunctionElement e)
        {
            if (this.MathFunctions.Count != e.MathFunctions.Count)
            {
                return false;
            }
            for (int i = 0; i < this.MathFunctions.Count; i++)
            {
                if (this.MathFunctions[i].Item1 != e.MathFunctions[i].Item1 ||
                    this.MathFunctions[i].Item2.Length != e.MathFunctions[i].Item2.Length)
                {
                    return false;
                }
                for (int j = 0; j < this.MathFunctions[i].Item2.Length; j++)
                {
                    if (!this.MathFunctions[i].Item2[j].Equals(e.MathFunctions[i].Item2[j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Check if this function can be calculated and turned to double
        /// </summary>
        /// <returns></returns>
        public bool CanBeCalculated()
        {
            if (!this.IsDouble())
            {
                return false;
            }
            foreach (var mf in this.MathFunctions)
            {
                foreach (var f in mf.Item2)
                {
                    if (!f.CanBeCalculated())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected string showFunctions(string varName = null)
        {
            if (varName == null)
            {
                varName = this.Name;
            }
            if (this.MathFunctions.Count == 0)
            {
                return varName;
            }

            var temp = varName;

            for (int i = 0; i < this.MathFunctions.Count - 1; i++)
            {
                temp = this.MathFunctions[i].Item1.ToString(temp, this.MathFunctions[i].Item2);
            }

            return temp;
        }

        public FunctionElement ApplyFunction(string funcName, params FunctionElement[] parameters)
        {
            //this.MathFunctions.Add(new Tuple<IMathFunction, FunctionElement[]>(Function.GetMathFunction(funcName), parameters));
            List<FunctionElement> p = new List<FunctionElement>();
            p.Add(this);
            p.AddRange(parameters);
            try
            {
                var res = Function.GetMathFunction(funcName).Calculate(p.ToArray());
                return res;
            }
            catch(InvalidMathFunctionParameters)
            {
                throw new InvalidMathFunctionParameters(funcName);
            }
        }

        public FunctionElement ApplyFunction(string funcName)
        {
            return this.ApplyFunction(funcName, null);
        }

        /// <summary>
        /// It's work like this
        /// x^2 = y
        /// this = x^2
        /// other = y
        /// this.ReverseAllFunctionsTo(other)
        /// => this = x
        /// other = y ^ -2
        /// It change current instance!
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Item1 - modified original object, Item2 - modified e param</returns>
        public Tuple<FunctionElement, FunctionElement> ReverseAllFunctionsTo(FunctionElement e)
        {
            FunctionElement e1;
            if (this.IsLeaf())
            {
                e1 = this.ToLeaf();
            }
            else
            {
                e1 = this.Clone() as FunctionElement;
            }

            var e2 = e.Clone() as FunctionElement;

            for (int i = e1.MathFunctions.Count - 1; i >= 0; i--)
            {
                if (!e1.MathFunctions[i].Item1.HasReverse())
                {
                    break;
                }
                List<FunctionElement> pars = new List<FunctionElement>();
                pars.Add(e2);
                pars.AddRange(e1.MathFunctions[i].Item2);
                e2 = e1.MathFunctions[i].Item1.CalculateReverse(pars.ToArray());
                //e2 = e2.ApplyFunction(e1.MathFunctions[i].Item1.FunctionName, e1.MathFunctions[i].Item2);
                e1.MathFunctions.RemoveAt(i);
            }
            Function e11 = new Function(e1);
            
            return new Tuple<FunctionElement,FunctionElement>(new Function(e1), e2);
        }

        /// <summary>
        /// Force adding function (without calculating)
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public void ForceAddFunction(string funcName, params FunctionElement[] parameters)
        {
            this._isModified = true;
            this.MathFunctions.Add(new Tuple<IMathFunction, FunctionElement[]>(Function.GetMathFunction(funcName), parameters));
        }

        public abstract bool IsVariableMultiplication();
        public abstract VariablesMulriplication ToVariableMultiplication();

        public virtual string ToMathML()
        {
            string s = this.ToMathMLShort();

            var last = this.GetLastFunction();
            if (last != null && last.Item1.Equals(Function.GetMathFunction("power")))
            {
                s = last.Item1.ToStringML(s, last.Item2);
            }

            return String.Format("<math xmlns='http://www.w3.org/1998/Math/MathML'><mrow>{0}</mrow></math>", s);
        }
        public abstract string ToMathMLShort();
        public string ShowMLFunctions(string varName = null)
        {
            if (varName == null)
            {
                varName = this.Name;
            }
            if (this.MathFunctions.Count == 0)
            {
                return varName;
            }

            var temp = varName;

            int powers = 0;
            if (this.MathFunctions.Last().Item1.Equals(Function.GetMathFunction("power")))
            {
                powers = 1;
            }

            for (int i = 0; i < this.MathFunctions.Count - powers; i++)
            {
                temp = this.MathFunctions[i].Item1.ToStringML(temp, this.MathFunctions[i].Item2);
            }

            return temp;
        }

        public void CopyFunctions(FunctionElement e)
        {
            this._isModified = true;
            this.MathFunctions.Clear();
            this.ForceAddFunctions(e);
        }

        public void OverrideFunction(FunctionElement e)
        {
            this.MathFunctions.Clear();
            this.OverrideFunction(this.ApplyFunctions(e));
        }

        public FunctionElement ApplyFunctions(FunctionElement e)
        {
            var f = this.Clone() as FunctionElement;
            foreach (var mf in e.MathFunctions)
            {
                FunctionElement[] _params = new FunctionElement[mf.Item2.Length];
                for (int i = 0; i < mf.Item2.Length; i++)
                {
                    _params[i] = mf.Item2[i].Clone() as FunctionElement;
                }
                f = f.ApplyFunction(mf.Item1.FunctionName, _params);
            }
            return f;
        }

        /// <summary>
        /// Force append of function of anouther function element
        /// </summary>
        /// <param name="e"></param>
        public void ForceAddFunctions(FunctionElement e)
        {
            this._isModified = true;
            foreach (var mf in e.MathFunctions)
            {
                FunctionElement[] _params = new FunctionElement[mf.Item2.Length];
                for (int i = 0; i < mf.Item2.Length; i++)
                {
                    _params[i] = mf.Item2[i].Clone() as FunctionElement;
                }
                this.ForceAddFunction(mf.Item1.FunctionName, _params);
            }
        }

        public Tuple<IMathFunction, FunctionElement[]> GetLastFunction()
        {
            if (this.MathFunctions.Count == 0)
                return null;
            return this.MathFunctions.Last();
        }

        public override string ToString()
        {
            string s = this.Name;
            bool b = this is Function ? (this as Function).IsNeedBrackets() : false;
            if (b && this.MathFunctions.Count > 0)
            {
                s = String.Format("({0})", s);
            }
            foreach (var mf in this.MathFunctions)
            {
                s = mf.Item1.ToString(s, mf.Item2);
            }
            return s;
        }

        public abstract bool IsLeaf();

        public abstract FunctionElement ToLeaf();

        public abstract Dictionary<string, FunctionElement> GetVariablesByConstant(FunctionElement e);
    }
}
