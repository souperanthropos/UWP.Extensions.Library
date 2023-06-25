using SampleApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace SampleApp.Services.Navigation
{
    public class ViewModelBinder : IViewModelBinder
    {
        public ViewModelBinder() { }

        public void Bind(FrameworkElement view, object viewModel, NavigationMode navigationMode)
        {
            var context = viewModel as NavigationContext;

            if (view.DataContext != null)
            {
                (view.DataContext as ViewModelBase)?.OnNavigationCompleted(context?.Parameter, navigationMode);
                return;
            }
        }
    }
}
