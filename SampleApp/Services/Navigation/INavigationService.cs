using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace SampleApp.Services.Navigation
{
    public interface INavigationService
    {
        void SetAdditionalFrame(Frame frame);
        bool NavigateFromChildFrame<TView>() where TView : Page;
        bool NavigateFromChildFrame<TView>(Dictionary<string, object> parameter = null) where TView : Page;
        bool NavigateFromAdditionalFrame<TView>() where TView : Page;
        bool NavigateFromAdditionalFrame<TView>(Dictionary<string, object> parameter = null) where TView : Page;
        void NavigateBackFromChildFrame();
        bool Navigate<TView>() where TView : Page;
        bool Navigate<TView>(Dictionary<string, object> parameter = null) where TView : Page;
        void NavigateBack();
        void NavigateBackFromAdditionalFrame();
    }
}
