using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using SampleApp.Providers;
using SampleApp.Views;
using UWP.Extensions.Library.Services.Navigation;
using UWP.Extensions.Library.Services.Navigation.Interfaces;

namespace SampleApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDependencyInjectionProvider _dIProvider;

        public IDependencyInjectionProvider DIProvider => _dIProvider;

        public IRelayCommand<string> OpenNavigationMenuItemCommand { get; }

        public MainPageViewModel()
        {
            _navigationService = Ioc.Default.GetService<INavigationService>();

            _dIProvider = new DependencyInjectionProvider();

            OpenNavigationMenuItemCommand = new RelayCommand<string>(s => OpenNavigationMenuItem(s));
        }

        private void OpenNavigationMenuItem(string s)
        {
            if(s == "IncrementalLoadingPage") _navigationService.Navigate<IncrementalLoadingPage>("contentFrame");
        }
    }
}
