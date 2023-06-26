using Bogus;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Collections;
using SampleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UWP.Extensions.Library.Collections;
using Windows.UI.Xaml.Data;
using static Bogus.DataSets.Name;

namespace SampleApp.ViewModels
{
    public class IncrementalLoadingViewModel : ViewModelBase
    {
        private int _count;
        private IncrementalLoadingCollection<FakeData> _items;
        private IncrementalLoadingGroupCollection<string, FakeData> _groupItems;

        public CollectionViewSource Items { get; }

        public IRelayCommand<bool?> GrouppingCheckBoxClickedCommand { get; }
        public IRelayCommand<string> SearchTextChangedCommand { get; }

        public IncrementalLoadingViewModel()
        {
            _count = 100;
            _groupItems = new IncrementalLoadingGroupCollection<string, FakeData>(itemsPerPage: 20);
            _items = new IncrementalLoadingCollection<FakeData>(itemsPerPage: 20);

            Items = new CollectionViewSource
            {
                IsSourceGrouped = false,
                Source = _items
            };

            FillData(false);

            GrouppingCheckBoxClickedCommand = new RelayCommand<bool?>(b => OnGrouppingCheckBoxClicked(b));
            SearchTextChangedCommand = new RelayCommand<string>(s => OnSearchTextChanged(s));
        }

        private void FillData(bool isGroupEnabled)
        {
            if (isGroupEnabled)
            {
                var testData = new Faker<FakeData>()
                    .CustomInstantiator(f => new FakeData())
                    .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                    .RuleFor(u => u.Name, (f, u) => f.Name.FirstName(u.Gender) + " " + f.Name.LastName(u.Gender))
                    .Generate(_count);

                var grouppingList = new List<ObservableGroup<string, FakeData>>();
                var maleList = from td in testData
                               where td.Gender == Gender.Male
                               select td;
                var femaleList = from td in testData
                                 where td.Gender == Gender.Female
                                 select td;

                grouppingList.Add(new ObservableGroup<string, FakeData>(Gender.Male.ToString(), maleList));
                grouppingList.Add(new ObservableGroup<string, FakeData>(Gender.Female.ToString(), femaleList));

                _groupItems.SetSource(grouppingList);

                Items.IsSourceGrouped = true;
                Items.Source = _groupItems;
            }
            else
            {
                var testData = new Faker<FakeData>()
                    .CustomInstantiator(f => new FakeData())
                    .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                    .RuleFor(u => u.Name, (f, u) => f.Name.FirstName(u.Gender) + " " + f.Name.LastName(u.Gender))
                    .Generate(_count);

                _items.SetSource(testData);

                Items.IsSourceGrouped = false;
                Items.Source = _items;
            }
        }

        private void OnGrouppingCheckBoxClicked(bool? isGroupEnabled)
        {
            FillData(isGroupEnabled.HasValue ? isGroupEnabled.Value : false);
        }

        private void OnSearchTextChanged(string searchText)
        {
            var isSearchStringEmpty = string.IsNullOrEmpty(searchText);
            if (Items.IsSourceGrouped)
            {
                if (isSearchStringEmpty)
                {
                    _groupItems.Filter = null;
                }
                else
                {
                    _groupItems.Filter = item => item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                }
            }
            else
            {
                if (isSearchStringEmpty)
                {
                    _items.Filter = null;
                }
                else
                {
                    _items.Filter = item => item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }
}
