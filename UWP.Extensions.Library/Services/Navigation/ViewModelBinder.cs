using UWP.Extensions.Library.Services.Navigation.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace UWP.Extensions.Library.Services.Navigation
{
    public class ViewModelBinder : IViewModelBinder
    {
        public ViewModelBinder() { }

        public void Bind(FrameworkElement view, object viewModel, NavigationMode navigationMode)
        {
            var context = viewModel as NavigationContext;

            if (view.DataContext != null)
            {
                (view.DataContext as IViewModelBase)?.OnNavigationCompleted(context?.Parameter, navigationMode);
                return;
            }
        }
    }
}
