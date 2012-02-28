using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MagicLibrary.Filters
{
    public interface IFilterableList<T> : IList<T>
    {
        CollectionFilters<T> AddFilters { get; set; }
        CollectionFilters<T> RemoveFilters { get; set; }

        bool Exists(Predicate<T> match);
        T Find(Predicate<T> match);
        void ForEach(Action<T> action);
        List<T> FindAll(Predicate<T> match);
        bool IsValid(CollectionFilters<T> filters, T item);

        void AddAddingFilter(CollectionFilters<T> filter);
        void AddRemovingFilter(CollectionFilters<T> filter);

        List<T> ToList();
    }
}
