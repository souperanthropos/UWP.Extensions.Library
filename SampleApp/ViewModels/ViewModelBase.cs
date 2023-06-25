using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace SampleApp.ViewModels
{
    public class ViewModelBase : ObservableRecipient
    {
        public ViewModelBase() { }

        public virtual void OnNavigationCompleted(Dictionary<string, object> parameter, NavigationMode navigationMode) { }
    }
}
