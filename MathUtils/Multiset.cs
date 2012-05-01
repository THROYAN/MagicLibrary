using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.MathFunctions;
using MagicLibrary.MathUtils.Functions;
using MagicLibrary.Exceptions;
using System.Text.RegularExpressions;

namespace MagicLibrary.MathUtils
{
    public class MultiSet<T> : FunctionElement, IEnumerable<KeyValuePair<T, int>>
        where T : FunctionElement
    {
        private const string _elementsSeparator = "`";
        private const string _plus = "++";

        private Dictionary<T, int> counts;

        public MultiSet()
        {
            this.counts = new Dictionary<T, int>();
            this.MathFunctions = new List<Tuple<IMathFunction, FunctionElement[]>>();
        }

        public MultiSet(FunctionElement e)
        {
            this.counts = new Dictionary<T, int>();
            this.MathFunctions = new List<Tuple<IMathFunction, FunctionElement[]>>();


            if (e.IsLeaf())
            {
                var e2 = e.ToLeaf();
                if (e2 is MultiSet<T>)
                {
                    var m = e2 as MultiSet<T>;
                    foreach (var item in m.counts)
                    {
                        this.counts[item.Key] = item.Value;
                    }
                    return;
                }
                else
                {
                    if (e2 is T)
                    {
                        this[e2 as T] = 1;
                        return;
                    }
                }
            }
            if (e.GetType() == typeof(T))
            {
                this[e as T] = 1;
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// The number of appearances of the element in the multi-set.
        /// </summary>
        /// <param name="s">Element of the set</param>
        /// <returns>Get the number of s</returns>
        public int this[T s]
        {
            get
            {
                if (this.counts.Count != 0 && this.counts.Keys.FirstOrDefault(k => k.Equals(s)) != null)
                {
                    return this.counts[this.counts.Keys.First(k => k.Equals(s))];
                }
                return 0;
            }
            set
            {
                if (value >= 0)
                {
                    if (this.counts.Count != 0 && this.counts.Keys.FirstOrDefault(k => k.Equals(s)) != null)
                    {
                        var s2 = this.counts.Keys.First(k => k.Equals(s));
                        if (value == 0)
                        {
                            this.counts.Remove(s2);
                        }
                        this.counts[s2] = value;
                    }
                    else
                    {
                        if (value != 0)
                        {
                            this.counts.Add(s, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get element by index
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns></returns>
        public T GetElementByIndex(int index)
        {
#warning Как-то не рационально, нужно где-то в другом месте убедиться, что такие элементы отсеиваются
            return this.counts.Where(item => item.Value > 0).ElementAt(index).Key;
        }

        /// <summary>
        /// Check for appearances of element in the multi-set is not zero
        /// </summary>
        /// <param name="s">Element of the set</param>
        /// <returns></returns>
        public bool IsExists(T s)
        {
            return this.counts.ContainsKey(s) && this.counts[s] != 0;
        }

        /// <summary>
        /// Get count of elements of the multi-set.
        /// </summary>
        public int Count
        {
            get
            {
                return this.counts.Count(item => item.Value > 0);
            }
        }

        public MultiSetEnum<T> GetEnumerator()
        {
            return new MultiSetEnum<T>(this);
        }

        IEnumerator<KeyValuePair<T, int>> IEnumerable<KeyValuePair<T, int>>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            if (this.Count == 0)
                return "<empty set>";
            StringBuilder sb = new StringBuilder();

            foreach (var item in this.counts)
            {
                if (item.Value > 0)
                {
                    string elString = item.Key is Function ? (item.Key as Function).IsNeedBrackets() ? String.Format("({0})", item.Key) : item.Key.ToString() : item.Key.ToString();
                    sb.AppendFormat("{0}{1}{2}{3}", item.Value, MultiSet<T>._elementsSeparator, elString, MultiSet<T>._plus);
                }
            }
            sb.Remove(sb.Length - MultiSet<T>._plus.Length, MultiSet<T>._plus.Length);

            return sb.ToString();
        }

        public int Multiplicity()
        {
            return this.counts.Sum(item => item.Value > 0 ? item.Value : 0);
        }

        #region Operations

        public static MultiSet<T> operator +(MultiSet<T> m1, MultiSet<T> m2)
        {
            MultiSet<T> m = new MultiSet<T>();
            foreach (var item in m1.counts)
            {
                m[item.Key] = item.Value;
            }
            foreach (var item in m2.counts)
            {
                m[item.Key] += item.Value;
            }
            return m;
        }

        public static MultiSet<T> operator +(MultiSet<T> m, T element)
        {
            MultiSet<T> newM = new MultiSet<T>();
            foreach (var item in m.counts)
            {
                newM[item.Key] = item.Value;
            }
            newM[element]++;
            return newM;
        }

        public static MultiSet<T> operator -(MultiSet<T> m1, MultiSet<T> m2)
        {
            if (!(m1 >= m2))
            {
                throw new Exception("Error subtraction");
            }

            MultiSet<T> m = new MultiSet<T>();
            foreach (var item in m1.counts)
            {
                m[item.Key] = item.Value;
            }
            foreach (var item in m2.counts)
            {
                m[item.Key] -= item.Value;
            }
            return m;
        }

        /// <summary>
        /// Scalar-multiplication
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static MultiSet<T> operator *(int n, MultiSet<T> m)
        {
            if (n < 0)
            {
                throw new Exception("Number must be non negative");
            }

            MultiSet<T> newM = new MultiSet<T>();
            foreach (var item in m.counts)
            {
                newM[item.Key] = item.Value * n;
            }
            return newM;
        }

        #endregion

        #region Relations

        public static bool operator <(MultiSet<T> m1, MultiSet<T> m2)
        {
            var elements = m1.counts.Keys.Union(m2.counts.Keys);

            foreach (var item in elements)
            {
                if (!(m1[item] < m2[item]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator <=(MultiSet<T> m1, MultiSet<T> m2)
        {
            var elements = m1.counts.Keys.Union(m2.counts.Keys);

            foreach (var item in elements)
            {
                if (!(m1[item] <= m2[item]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator >(MultiSet<T> m1, MultiSet<T> m2)
        {
            var elements = m1.counts.Keys.Union(m2.counts.Keys);

            foreach (var item in elements)
            {
                if (!(m1[item] > m2[item]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator >=(MultiSet<T> m1, MultiSet<T> m2)
        {
            var elements = m1.counts.Keys.Union(m2.counts.Keys);

            foreach (var item in elements)
            {
                if (!(m1[item] >= m2[item]))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            var m = obj as MultiSet<T>;
            if (m == null)
                return false;

            var elements = this.counts.Keys.Union(m.counts.Keys);

            foreach (var item in elements)
            {
                if (!(this[item] == m[item]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        public static MathOperator SumOperator = new MathOperator("multiset sum", delegate(FunctionElement e1, FunctionElement e2)
            {
                if (!(e1.IsLeaf() && e2.IsLeaf()))
                {
                    throw new InvalidMathFunctionParameters();
                }
                try
                {
                    var m1 = new MultiSet<T>(e1);
                    var m2 = new MultiSet<T>(e2);

                    return m1 + m2;
                }
                catch(Exception e)
                {
                    throw new InvalidMathFunctionParameters();
                }

            }, Regex.Escape(MultiSet<T>._plus));

        public static MathOperator MSElement = new MathOperator("`", delegate(FunctionElement e1, FunctionElement e2)
            {
                if (!e1.IsDouble() || Math.Round(e1.ToDouble()) != e1.ToDouble())
                {
                    throw new InvalidMathFunctionParameters();
                }

                MultiSet<T> m = new MultiSet<T>();
                m[new Function(e2) as T] = (int)e1.ToDouble();

                return m;
            }, Regex.Escape(MultiSet<T>._elementsSeparator));

        public override string Name
        {
            get { return this.ToString(); }
        }

        public override bool IsDouble()
        {
            return false;
        }

        public override FunctionElement Pow(double power)
        {
            throw new NotImplementedException();
        }

        public override FunctionElement SetVariableValue(string name, double value)
        {
            if (this.Count > 0)
            {
                MultiSet<T> m = new MultiSet<T>();
                foreach (var item in this.counts)
                {
                    var f = item.Key.SetVariableValue(name, value);
                    m[f as T] += item.Value;
                }
                return m;
            }
            return this.Clone() as FunctionElement;
        }

        public override FunctionElement SetVariableValue(string name, FunctionElement value)
        {
            if (this.Count > 0)
            {
                MultiSet<T> m = new MultiSet<T>();
                foreach (var item in this.counts)
                {
                    var f = item.Key.SetVariableValue(name, value);
                    m[f as T] += item.Value;
                }
                return m;
            }
            return this.Clone() as FunctionElement;
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
            foreach (var item in this.counts)
            {
                if (item.Key.HasVariable(name))
                {
                    return true;
                }
            }
            return false;
        }

        public override object Clone()
        {
            MultiSet<T> m = new MultiSet<T>();
            
            foreach (var item in this.counts)
            {
                m.counts[item.Key.Clone() as T] = item.Value;
            }
            m.MathFunctions.Clear();
            m.ForceAddFunctions(this);
            return m;
        }

        public override bool IsVariableMultiplication()
        {
            return false;
        }

        public override VariablesMulriplication ToVariableMultiplication()
        {
            return new VariablesMulriplication(this.Clone() as MultiSet<T>);
        }

        public override string ToMathMLShort()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this.counts)
            {
                sb.AppendFormat("{0}++", item.Key.ToMathMLShort());
            }
            if (sb.Length != 0)
            {
                sb = sb.Remove(sb.Length - 2, 2);
            }
            return sb.ToString();
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
            foreach (var item in this.counts)
            {
                if (!item.Key.IsConstant())
                    return false;
            }
            return true;
        }

        public override void ParseFromString(string func)
        {
            throw new NotImplementedException();
        }
    }
}
