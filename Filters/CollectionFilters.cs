using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.Filters
{
    public class CollectionFilters<T> : IEnumerable<KeyValuePair<string, Func<IFilterableList<T>, T, FilterStatus, FilterStatus>>>
    {
        protected Dictionary<string, Func<IFilterableList<T>, T, FilterStatus, FilterStatus>> filters;

        public CollectionFilters()
        {
            this.filters = new Dictionary<string, Func<IFilterableList<T>, T, FilterStatus, FilterStatus>>();
        }

        public void AddFilter(string name, Func<IFilterableList<T>, T, bool> filter, FilterStatus trueResult, FilterStatus falseResult)
        {
            this.AddFilter(name, (array, vertexValue, curStatus) => filter(array, vertexValue) ? trueResult : falseResult);
        }

        public void AddFilter(string name, Func<IFilterableList<T>, T, bool> filter, FilterStatus trueResult)
        {
            this.AddFilter(name, (array, vertexValue, curStatus) => filter(array, vertexValue) ? trueResult : curStatus);
        }

        public void AddFilter(string name, Func<IFilterableList<T>, T, FilterStatus, FilterStatus> func)
        {
            if (name == null)
            {
                name = this.filters.Count.ToString();
            }
            if (this.filters.ContainsKey(name))
                return;

            this.filters.Add(name, func);
        }

        public void AddFilterWithResponseCriterior(
            string name,
            Func<IFilterableList<T>, T, bool> filter,
            FilterStatus trueResult,
            FilterStatus criterior = FilterStatus.Successful,
            bool hasFlag = true
        )
        {
            this.AddFilter(
                name,
                (array, vertexValue, curStatus) =>
                (curStatus == criterior) == hasFlag
                    ? filter(array, vertexValue)
                        ? trueResult
                        : curStatus
                    : curStatus
            );
        }

        public FilterStatus ExecuteAll(IFilterableList<T> array, T item, FilterStatus status = FilterStatus.Successful)
        {
            //ModificationStatus status = this.CurrentStatus;
            foreach (var filter in this.filters)
            {
                status = filter.Value(array, item, status);
            }
            return status;
        }

        public static CollectionFilters<T> operator +(CollectionFilters<T> filter1, CollectionFilters<T> filter2)
        {
            CollectionFilters<T> result = new CollectionFilters<T>();
            foreach (var filter in filter1.filters)
            {
                result.AddFilter(filter.Key, filter.Value);
            }
            foreach (var filter in filter2.filters)
            {
                result.AddFilter(filter.Key, filter.Value);
            }

            return result;
        }

        public static CollectionFilters<T> operator +(CollectionFilters<T> filters, Func<IFilterableList<T>, T, FilterStatus, FilterStatus> filter)
        {
            CollectionFilters<T> result = new CollectionFilters<T>();
            foreach (var f in filters.filters)
            {
                result.AddFilter(f.Key, f.Value);
            }
            result.AddFilter("NoNameFilter", filter);
            return result;
        }

        public static CollectionFilters<T> UniqueProperty<TResult>(Func<T, TResult> func)
        {
            CollectionFilters<T> result = new CollectionFilters<T>();

            List<int> s = new List<int>();

            result.AddFilterWithResponseCriterior(
                String.Format("Unique-{0}", func.Method.Name),
                    (array, item) =>
                        array.Select(func).Contains(func(item)),
                FilterStatus.AlreadyExist
            );

            return result;
        }

        public static CollectionFilters<T> UniqueProperties<TResult>(params Func<T, TResult>[] funcs)
        {
            CollectionFilters<T> result = new CollectionFilters<T>();

            StringBuilder fNameSB = new StringBuilder("");
            foreach (var item in funcs)
            {
                fNameSB.AppendFormat("{0}, ", item.Method.Name);
            }
            fNameSB.Remove(fNameSB.Length - 2, 2);

            result.AddFilterWithResponseCriterior(
                String.Format("Unique-[{0}]", fNameSB.ToString()),(array, item) =>
                    array.Exists(delegate(T i){
                        foreach (var f in funcs)
	                    {
                            if (!f(i).Equals(f(item)))
                                return false;
	                    }
                        return true;
                    }),
                FilterStatus.AlreadyExist
            );

            return result;
        }

        public static CollectionFilters<T> ForeignKey<T2>(IFilterableList<T2> list2, Func<T, T2> func)
        {
            CollectionFilters<T> result = new CollectionFilters<T>();

            result.AddFilterWithResponseCriterior(
                String.Format("FK[{0}]", func.Method.Name), (array, item) =>
                    !list2.Contains(func(item)),
                FilterStatus.NotExist
            );

            return result;
        }

        public static CollectionFilters<T> ForeignKey<T2, TProp>(IFilterableList<T2> list2, Func<T, TProp> func1, Func<T2, TProp> func2)
        {
            CollectionFilters<T> result = new CollectionFilters<T>();

            result.AddFilterWithResponseCriterior(
                String.Format("FK[{0}-{1}]", func1.Method.Name, func2.Method.Name), (array, item) =>
                    !list2.Select(func2).Contains(func1(item)),
                FilterStatus.NotExist
            );

            return result;
        }

        public IEnumerator<KeyValuePair<string, Func<IFilterableList<T>, T, FilterStatus, FilterStatus>>> GetEnumerator()
        {
            return this.filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }

    public enum FilterStatus { Successful, AlreadyExist, NotExist, Error, InvalidParameters, Canceled }
}
