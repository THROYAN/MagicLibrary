using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MagicLibrary.Filters
{
    public class FilterableList<T> : IFilterableList<T>
    {
        public CollectionFilters<T> AddFilters { get; set; }
        public CollectionFilters<T> RemoveFilters { get; set; }
        private List<T> items;

        public FilterableList(CollectionFilters<T> addFilters = null, CollectionFilters<T> removeFilters = null)
        {
            if (addFilters == null)
            {
                this.AddFilters = new CollectionFilters<T>();
            }
            else
            {
                this.AddFilters = addFilters;
            }
            if (removeFilters == null)
            {
                this.RemoveFilters = new CollectionFilters<T>();
            }
            else
            {
                this.RemoveFilters = removeFilters;
            }
            this.items = new List<T>();
        }

        void FilterableList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public FilterableList(FilterableList<T> existList)
        {
            this.items = new List<T>(existList.items);
            this.AddFilters = new CollectionFilters<T>() + existList.AddFilters;
            this.RemoveFilters = new CollectionFilters<T>() + existList.RemoveFilters;
        }

        public FilterableList(List<T> list)
        {
            this.items = new List<T>(list);
            this.AddFilters = new CollectionFilters<T>();
            this.RemoveFilters = new CollectionFilters<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return this.items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.Insert(index, item, false);
        }

        public void Insert(int index, T item, bool ignoreFilters)
        {
            if (ignoreFilters || this.IsValid(this.AddFilters, item))
            {
                this.items.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt(index, false);
        }

        public void RemoveAt(int index, bool ignoreFilters)
        {
            if(ignoreFilters || this.IsValid(this.RemoveFilters, this[index]))
            {
                this.items.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                return this.items[index];
            }
            set
            {
                if (this.IsValid(this.AddFilters, value))
                {
                    this.items[index] = value;
                }
            }
        }

        public void Add(T item, bool ignoreFilters)
        {
            if (ignoreFilters || this.IsValid(this.AddFilters, item))
            {
                this.items.Add(item);
            }
            else
            {
                object a = 2;
            }
        }

        public void Add(T item)
        {
            this.Add(item, false);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool Contains(T item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return this.Remove(item, false);
        }

        public bool Remove(T item, bool ignoreFilters)
        {
            if(ignoreFilters || this.IsValid(this.RemoveFilters, item))
            {
                return this.items.Remove(item);
            }
            return false;
        }

        public T Find(Predicate<T> match)
        {
            foreach (var item in this.items)
            {
                if (match(item))
                    return item;
            }
            return default(T);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            List<T> list = new List<T>();
            foreach (var item in this.items)
            {
                if (match(item))
                    list.Add(item);
            }
            return list;
        }

        public bool Exists(Predicate<T> match)
        {
            foreach (var item in this.items)
            {
                if (match(item))
                    return true;
            }
            return false;
        }

        public bool IsValid(CollectionFilters<T> filters, T item)
        {
            return filters.ExecuteAll(this, item) == FilterStatus.Successful;
        }


        public void AddAddingFilter(CollectionFilters<T> filter)
        {
            this.AddFilters += filter;
        }

        public void AddRemovingFilter(CollectionFilters<T> filter)
        {
            this.RemoveFilters += filter;
        }


        public void ForEach(Action<T> action)
        {
            foreach (var item in this.items)
            {
                action(item);
            }
        }


        public List<T> ToList()
        {
            return new List<T>(this.items);
        }
    }
}
