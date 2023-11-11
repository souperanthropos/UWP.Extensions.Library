using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UWP.Extensions.Library.Extensions;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace UWP.Extensions.Library.Collections
{
    public class IncrementalLoadingCollection<T> : ObservableCollection<T>, IComparer<T>, ISupportIncrementalLoading
    {
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);

        private IList<T> _source;

        private readonly ObservableCollection<SortDescription> _sortDescriptions;
        private readonly Dictionary<string, PropertyInfo> _sortProperties;

        private bool _hasMoreItems;
        private int _itemsPerPage;
        private CancellationToken _cancellationToken;
        private int _currentPageIndex;

        public IList<SortDescription> SortDescriptions => _sortDescriptions;

        public bool UseComparer { get; set; }

        private Predicate<T> _filter;
        public Predicate<T> Filter
        {
            get
            {
                return _filter;
            }

            set
            {
                /*if (_filter == value)
                {
                    return;
                }*/

                _filter = value;
                HandleFilterChanged();
            }
        }

        public bool HasMoreItems
        {
            get => _hasMoreItems;

            private set
            {
                if (value != _hasMoreItems)
                {
                    _hasMoreItems = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasMoreItems)));
                }
            }
        }

        public IncrementalLoadingCollection(int itemsPerPage = 20)
        {
            UseComparer = true;
            _itemsPerPage = itemsPerPage;
            _sortDescriptions = new ObservableCollection<SortDescription>();
            _sortProperties = new Dictionary<string, PropertyInfo>();
        }

        public IncrementalLoadingCollection(IList<T> source, int itemsPerPage = 20) : this(itemsPerPage)
        {
            SetSource(source);
        }

        public void SetSource(IList<T> source)
        {
            _source = source;
            _currentPageIndex = 0;
            this.Clear();

            if (_source.Count > 0)
            {
                HasMoreItems = true;
            }
            else
            {
                HasMoreItems = false;
            }

            LoadMoreItemsAsync(0);
        }

        public void MoveItem(T item)
        {
            if (!UseComparer)
            {
                this.Add(item);
            }
            else
            {
                this.Remove(item);
                var newIndex = this.Items.BinarySearch(item, this);
                if (newIndex < 0)
                {
                    newIndex = ~newIndex;
                }
                this.Insert(newIndex, item);
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
            => LoadMoreItemsAsync(count, new CancellationToken(false)).AsAsyncOperation();

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count, CancellationToken cancellationToken)
        {
            if (_source == null) return new LoadMoreItemsResult { Count = 0 };

            uint resultCount = 0;
            _cancellationToken = cancellationToken;

            await _mutex.WaitAsync();
            try
            {
                IEnumerable<T> result;
                if (UseComparer)
                {
                    if (Filter == null)
                    {
                        result = _source.OrderBy(x => x, this).Skip(_currentPageIndex * _itemsPerPage).Take(_itemsPerPage);
                    }
                    else
                    {
                        result = _source.ToList().FindAll(Filter).OrderBy(x => x, this).Skip(_currentPageIndex * _itemsPerPage).Take(_itemsPerPage);
                    }
                }
                else
                {
                    if (Filter == null)
                    {
                        result = _source.Skip(_currentPageIndex * _itemsPerPage).Take(_itemsPerPage);
                    }
                    else
                    {
                        result = _source.ToList().FindAll(Filter).Skip(_currentPageIndex * _itemsPerPage).Take(_itemsPerPage);
                    }
                }

                foreach (T item in result)
                {
                    this.Add(item);
                }

                resultCount = (uint)result.Count();
                if (this.Count >= _source.Count)
                {
                    HasMoreItems = false;
                }
                else
                {
                    HasMoreItems = true;
                }
            }
            finally
            {
                _currentPageIndex += 1;
                _mutex.Release();
            }

            return new LoadMoreItemsResult { Count = resultCount };
        }

        private async void HandleFilterChanged()
        {
            this.Clear();
            _currentPageIndex = 0;
            _sortProperties.Clear();
            await LoadMoreItemsAsync(0);
        }

        /// <summary>
        /// IComparer implementation
        /// </summary>
        /// <param name="x">Object A</param>
        /// <param name="y">Object B</param>
        /// <returns>Comparison value</returns>
        int IComparer<T>.Compare(T x, T y)
        {
            if (!_sortProperties.Any())
            {
                var type = x.GetType();
                foreach (var sd in _sortDescriptions)
                {
                    if (!string.IsNullOrEmpty(sd.PropertyName))
                    {
                        _sortProperties[sd.PropertyName] = type.GetProperty(sd.PropertyName);
                    }
                }
            }

            foreach (var sd in _sortDescriptions)
            {
                object cx, cy;

                if (string.IsNullOrEmpty(sd.PropertyName))
                {
                    cx = x;
                    cy = y;
                }
                else
                {
                    var pi = _sortProperties[sd.PropertyName];

                    cx = pi.GetValue(x);
                    cy = pi.GetValue(y);
                }

                var cmp = sd.Comparer.Compare(cx, cy);

                if (cmp != 0)
                {
                    return sd.Direction == SortDirection.Ascending ? +cmp : -cmp;
                }
            }

            return 0;
        }
    }
}
