using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using SampleApp.Services.Navigation;
using SampleApp.Views;
using System;
using System.Reflection.Metadata;

namespace SampleApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public IRelayCommand<string> OpenNavigationMenuItemCommand { get; }

        public MainPageViewModel()
        {
            _navigationService = Ioc.Default.GetService<INavigationService>();

            OpenNavigationMenuItemCommand = new RelayCommand<string>(s => OpenNavigationMenuItem(s));
        }

        private void OpenNavigationMenuItem(string s)
        {
            if(s == "IncrementalLoadingPage") _navigationService.NavigateFromChildFrame<IncrementalLoadingPage>();
        }
    }
}
