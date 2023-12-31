﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace UWP.Extensions.Library.Services.Navigation.Interfaces
{
    public interface IViewModelBase
    {
        void OnNavigationCompleted(Dictionary<string, object> parameter, NavigationMode navigationMode);
        Task InitializeAsync();
    }
}
