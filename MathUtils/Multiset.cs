using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils
{
    public class MultiSet<T> : IEnumerable<KeyValuePair<T, int>>
    {
        private Dictionary<T, int> counts;

        public MultiSet()
        {
            this.counts = new Dictionary<T, int>();
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
                if (this.counts.ContainsKey(s))
                {
                    return this.counts[s];
                }
                return 0;
            }
            set
            {
                if (value >= 0)
                {
                    if (this.counts.ContainsKey(s))
                    {
                        if (value == 0)
                        {
                            this.counts.Remove(s);
                        }
                        this.counts[s] = value;
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
                return "empty";
            StringBuilder sb = new StringBuilder();

            string plus = " + ", separator = "*";

            foreach (var item in this.counts)
            {
                if (item.Value > 0)
                {
                    sb.AppendFormat("{0}{1}{2}{3}", item.Value, separator, item.Key, plus);
                }
            }
            sb.Remove(sb.Length - plus.Length, plus.Length);

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

        #endregion
    }

    public class MultiSetEnum<T> : IEnumerator<KeyValuePair<T, int>>
    {
        private MultiSet<T> m;
        private int curI;

        public MultiSetEnum(MultiSet<T> multiSet)
        {
            this.m = multiSet;
            
            curI = 0;
        }

        public KeyValuePair<T, int> Current
        {
            get
            {
                var temp = this.m.GetElementByIndex(curI);
                return new KeyValuePair<T, int>(temp, this.m[temp]);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            curI++;
            return this.curI < this.m.Count;
        }

        public void Reset()
        {
            this.curI = 0;
        }
    }
}
