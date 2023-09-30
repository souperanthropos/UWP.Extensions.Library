using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace SampleApp.Services.Navigation
{
    public interface INavigationService
    {
        void RegisterRoute(Frame frame, string routeName);
        bool Navigate<TView>(string routeName = "") where TView : Page;
        bool Navigate<TView>(string routeName = "", Dictionary<string, object> parameter = null) where TView : Page;
        void NavigateBack(string routeName = "");
    }
}
