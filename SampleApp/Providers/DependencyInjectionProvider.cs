using CommunityToolkit.Mvvm.DependencyInjection;
using UWP.Extensions.Library.Services.Navigation;
using UWP.Extensions.Library.Services.Navigation.Interfaces;

namespace SampleApp.Providers
{
    internal class DependencyInjectionProvider : IDependencyInjectionProvider
    {
        public INavigationService GetNavigationService()
        {
            return Ioc.Default.GetService<INavigationService>();
        }
    }
}
