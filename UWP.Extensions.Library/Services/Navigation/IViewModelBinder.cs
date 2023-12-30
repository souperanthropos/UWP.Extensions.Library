using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace UWP.Extensions.Library.Services.Navigation
{
    public interface IViewModelBinder
    {
        void Bind(FrameworkElement view, object viewModel, NavigationMode navigationMode);
    }
}
