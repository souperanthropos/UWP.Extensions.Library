using Bogus;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Collections;
using SampleApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Extensions.Library.Collections;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using static Bogus.DataSets.Name;

namespace SampleApp.ViewModels
{
    public class IncrementalLoadingViewModel : ViewModelBase
    {
        private IncrementalLoadingCollection<FakeData> _items;
        private IncrementalLoadingGroupCollection<string, FakeData> _groupItems;

        public CollectionViewSource Items { get; }

        public IRelayCommand<bool?> GrouppingCheckBoxClickedCommand { get; }

        public IncrementalLoadingViewModel()
        {
            _groupItems = new IncrementalLoadingGroupCollection<string, FakeData>(itemsPerPage: 20);
            _items = new IncrementalLoadingCollection<FakeData>(itemsPerPage: 20);

            Items = new CollectionViewSource
            {
                IsSourceGrouped = false,
                Source = _items
            };

            var testData = new Faker<FakeData>()
                .CustomInstantiator(f => new FakeData())
                .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                .RuleFor(u => u.Name, (f, u) => f.Name.FirstName(u.Gender) + " " + f.Name.LastName(u.Gender))
                .Generate(100);

            _items.SetSource(testData);

            Items.IsSourceGrouped = false;
            Items.Source = _items;

            GrouppingCheckBoxClickedCommand = new RelayCommand<bool?>(b => OnGrouppingCheckBoxClicked(b));
        }

        private void OnGrouppingCheckBoxClicked(bool? isGroupEnabled)
        {
            if (isGroupEnabled.HasValue && isGroupEnabled.Value)
            {
                var testData = new Faker<FakeData>()
                    .CustomInstantiator(f => new FakeData())
                    .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                    .RuleFor(u => u.Name, (f, u) => f.Name.FirstName(u.Gender) + " " + f.Name.LastName(u.Gender))
                    .Generate(100);

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
                    .Generate(100);

                _items.SetSource(testData);

                Items.IsSourceGrouped = false;
                Items.Source = _items;
            }
        }
    }
}
