using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using UWP.Extensions.Library.Services.Navigation.Interfaces;
using Windows.UI.Xaml.Navigation;

namespace SampleApp.ViewModels
{
    public class ViewModelBase : ObservableRecipient, IViewModelBase
    {
        public ViewModelBase() { }

        public virtual void OnNavigationCompleted(Dictionary<string, object> parameter, NavigationMode navigationMode) { }
    }
}
