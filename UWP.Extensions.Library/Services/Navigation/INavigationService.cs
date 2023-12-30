using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace UWP.Extensions.Library.Services.Navigation
{
    public interface INavigationService
    {
        void RegisterRoute(Frame frame, string routeName);
        bool Navigate<TView>(string routeName = "") where TView : Page;
        bool Navigate<TView>(Dictionary<string, object> parameter, string routeName = "") where TView : Page;
        void NavigateBack(string routeName = "");
    }
}
