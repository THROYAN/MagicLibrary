using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils
{
    [Serializable]
    public class MultiSetEnum<T> : IEnumerator<KeyValuePair<T, int>>
        where T : FunctionElement
    {
        private MultiSet<T> m;
        private int curI;

        public MultiSetEnum(MultiSet<T> multiSet)
        {
            this.m = multiSet;

            curI = -1;
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
#warning Что тут делать?
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
