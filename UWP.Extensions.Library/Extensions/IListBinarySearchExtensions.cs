using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UWP.Extensions.Library.Extensions
{
    public static class IListBinarySearchExtensions
    {
        public static int BinarySearch<T>(this IList<T> source, int index, int count, T item, IComparer<T> comparer)
        {
            if (index < 0) throw new Exception("Need non negative number of index.");
            if (count < 0) throw new Exception("Need non negative number of count.");
            if (source.Count - index < count) throw new Exception("Invalid offset length of count.");
            Contract.Ensures(Contract.Result<int>() <= index + count);
            Contract.EndContractBlock();

            return Array.BinarySearch<T>(source.Cast<T>().ToArray(), index, count, item, comparer);
        }

        public static int BinarySearch<T>(this IList<T> source, T item)
        {
            Contract.Ensures(Contract.Result<int>() <= source.Count);
            return BinarySearch(source, 0, source.Count, item, null);
        }

        public static int BinarySearch<T>(this IList<T> source, T item, IComparer<T> comparer)
        {
            Contract.Ensures(Contract.Result<int>() <= source.Count);
            return BinarySearch(source, 0, source.Count, item, comparer);
        }
    }
}
