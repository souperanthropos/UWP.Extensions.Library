using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP.Extensions.Library.Services.Navigation.Interfaces;
using Windows.UI.Xaml.Navigation;

namespace SampleApp.ViewModels
{
    public class ViewModelBase : ObservableRecipient, IViewModelBase
    {
        protected bool IsInitialized { get; private set; }

        public ViewModelBase() { }

        public virtual Task InitializeAsync()
        {
            IsInitialized = true;
            return Task.FromResult(true);
        }

        public virtual void OnNavigationCompleted(Dictionary<string, object> parameter, NavigationMode navigationMode) { }
    }
}
