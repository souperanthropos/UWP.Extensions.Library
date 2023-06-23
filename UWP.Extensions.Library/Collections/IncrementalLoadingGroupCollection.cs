using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UWP.Extensions.Library.Extensions;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace UWP.Extensions.Library.Collections
{
    public class IncrementalLoadingGroupCollection<TKey, TValue> : ObservableCollection<ObservableGroup<TKey, TValue>>, IComparer<TValue>, ISupportIncrementalLoading
    {
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);
        private readonly IList<TKey> _keys = new List<TKey>();

        private IList<ObservableGroup<TKey, TValue>> _source;
        private ObservableGroup<TKey, TValue> _currentGroup;

        private readonly ObservableCollection<SortDescription> _sortDescriptions;
        private readonly Dictionary<string, PropertyInfo> _sortProperties;

        private int _keyIndex;
        private bool _hasMoreItems;
        private int _itemsPerPage;
        private int _pageIndex;

        public IList<SortDescription> SortDescriptions => _sortDescriptions;

        public bool UseComparer { get; set; }

        private Predicate<TValue> _filter;
        public Predicate<TValue> Filter
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

        public IncrementalLoadingGroupCollection(int itemsPerPage = 20)
        {
            UseComparer = true;
            _itemsPerPage = itemsPerPage;
            _sortDescriptions = new ObservableCollection<SortDescription>();
            _sortProperties = new Dictionary<string, PropertyInfo>();
        }

        public IncrementalLoadingGroupCollection(IList<ObservableGroup<TKey, TValue>> source, int itemsPerPage = 20) : this(itemsPerPage)
        {
            SetSource(source);
        }

        public void SetSource(IList<ObservableGroup<TKey, TValue>> source)
        {
            _source = source;
            this.Clear();
            _keys.Clear();
            _keyIndex = 0;
            _pageIndex = 0;

            foreach (var item in source)
            {
                _keys.Add(item.Key);
            }
            if (_keys.Count > 0)
            {
                HasMoreItems = true;
                _currentGroup = new ObservableGroup<TKey, TValue>(_keys[_keyIndex]);
                this.Add(_currentGroup);
            }
            else
            {
                HasMoreItems = false;
            }
        }

        public void AddNewItem(int idxGroup, TValue item)
        {
            _source[idxGroup].Add(item);
            if (!UseComparer)
            {
                this[idxGroup].Add(item);
            }
            else
            {
                this[idxGroup].Remove(item);
                var newIndex = this[idxGroup].BinarySearch(item, this);
                if (newIndex < 0)
                {
                    newIndex = ~newIndex;
                }
                this[idxGroup].Insert(newIndex, item);
            }
        }

        public void RemoveItem(TValue item)
        {
            foreach (var group in this)
            {
                if (group.Remove(item))
                    break;
            }
        }

        public void MoveItem(int idxGroup, TValue item)
        {
            if (!UseComparer)
            {
                this[idxGroup].Add(item);
            }
            else
            {
                foreach (var group in this)
                {
                    if (group.Remove(item))
                        break;
                }
                foreach (var group in _source)
                {
                    if (group.Remove(item))
                        break;
                }
                _source[idxGroup].Add(item);
                if (idxGroup <= _keyIndex)
                {
                    IList<TValue> items = null;
                    if (Filter != null)
                    {
                        items = _source[idxGroup].ToList().FindAll(Filter);
                    }
                    else
                    {
                        items = _source[idxGroup];
                    }
                    items = items.OrderBy(x => x, this).ToList();
                    var newIndex = items.IndexOf(item);
                    if (this[idxGroup].Count == newIndex - 1 || this[idxGroup].Count >= newIndex)
                    {
                        this[idxGroup].Insert(newIndex, item);
                    }
                }
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
            => LoadMoreItemsAsync(count, new CancellationToken(false)).AsAsyncOperation();

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count, CancellationToken cancellationToken)
        {
            if (_source == null) return new LoadMoreItemsResult { Count = 0 };

            uint resultCount = 0;
            var totalItemsInGroup = 0;
            IEnumerable<TValue> items = null;

            await _mutex.WaitAsync();

            do
            {
                if (_keyIndex >= _keys.Count)
                {
                    break;
                }
                if (Filter != null)
                {
                    items = _source[_keyIndex].ToList().FindAll(Filter);
                }
                else
                {
                    items = _source[_keyIndex];
                }

                totalItemsInGroup = items.Count();

                if (UseComparer)
                {
                    items = items.OrderBy(x => x, this).Skip(_pageIndex * _itemsPerPage).Take(_itemsPerPage);
                }
                else
                {
                    items = items.Skip(_pageIndex * _itemsPerPage).Take(_itemsPerPage);
                }

                if (items.Count() == 0 && _keyIndex >= _keys.Count)
                {
                    break;
                }
                resultCount += (uint)items.Count();
                if (!AddMoreItems(items, totalItemsInGroup))
                {
                    break;
                }
            }
            while (resultCount < _itemsPerPage);

            _mutex.Release();
            return new LoadMoreItemsResult { Count = resultCount };
        }

        private bool AddMoreItems(IEnumerable<TValue> items, int totalItems)
        {
            foreach (var item in items)
            {
                _currentGroup.Add(item);
            }
            _pageIndex++;
            if (items.Count() < _itemsPerPage)
            {
                _keyIndex++;
                if (_keyIndex >= _keys.Count)
                {
                    HasMoreItems = false;
                }
                else
                {
                    HasMoreItems = true;
                    _currentGroup = new ObservableGroup<TKey, TValue>(_keys[_keyIndex]);
                    this.Add(_currentGroup);
                    _pageIndex = 0;
                    return true;
                }
            }
            else if (items.Count() == _itemsPerPage)
            {
                if (items.Count() < totalItems)
                {
                    HasMoreItems = true;
                    return false;
                }

                _keyIndex++;

                if (_keyIndex < _keys.Count)
                {
                    HasMoreItems = true;
                    _currentGroup = new ObservableGroup<TKey, TValue>(_keys[_keyIndex]);
                    this.Add(_currentGroup);
                    _pageIndex = 0;
                    return false;
                }

                HasMoreItems = false;
            }

            return false;
        }

        private async void HandleFilterChanged()
        {
            if (_keys.Count > 0)
            {
                this.Clear();
                _sortProperties.Clear();
                _keyIndex = 0;
                _pageIndex = 0;
                _currentGroup = new ObservableGroup<TKey, TValue>(_keys[_keyIndex]);
                this.Add(_currentGroup);
                await LoadMoreItemsAsync(0);
            }

            /*if (_filter != null)
            {
                foreach(var group in this)
                {
                    for (var index = 0; index < group.Count; index++)
                    {
                        var item = group.ElementAt(index);
                        if (_filter(item))
                        {
                            continue;
                        }

                        group.RemoveAt(index);
                        index--;
                    }
                }
            }
            if(_source != null)
            {
                var keyIndex = 0;
                foreach (var group in this)
                {
                    var viewHash = group.ToHashSet();
                    
                    for (var index = 0; index < _source[keyIndex].Count; index++)
                    {
                        var item = _source[keyIndex][index];
                        if (viewHash.Contains(item))
                        {
                            continue;
                        }
                        HandleItemAdded(keyIndex, item);
                    }
                    keyIndex++;
                }
            }*/
        }

        private bool HandleItemAdded(int keyIndex, TValue newItem)
        {
            if (_filter != null && !_filter(newItem))
            {
                return false;
            }

            if (!UseComparer)
            {
                this[keyIndex].Add(newItem);
            }
            else
            {
                _sortProperties.Clear();
                var newIndex = this[keyIndex].BinarySearch(newItem, this);
                if (newIndex < 0)
                {
                    newIndex = ~newIndex;
                }
                this[keyIndex].Insert(newIndex, newItem);
            }

            return true;
        }

        /// <summary>
        /// IComparer implementation
        /// </summary>
        /// <param name="x">Object A</param>
        /// <param name="y">Object B</param>
        /// <returns>Comparison value</returns>
        int IComparer<TValue>.Compare(TValue x, TValue y)
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
