using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SampleApp.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IFrameProvider _frameProvider;
        private readonly Frame _appFrame;
        private readonly IViewModelBinder viewModelBinder;

        private Frame _additionalFrame;
        private Frame _navigationFrame => _frameProvider.NavigationFrame;
        //private Type _currentTypePage;

        public NavigationService(IFrameProvider frameProvider, IViewModelBinder viewModelBinder)
        {
            _frameProvider = frameProvider;
            _frameProvider.NavigationFrameInitialized += _frameProvider_NavigationFrameInitialized;

            _appFrame = frameProvider.CurrentFrame;
            _appFrame.Navigating += OnNavigating;
            _appFrame.Navigated += OnNavigated;
            this.viewModelBinder = viewModelBinder;
        }

        private void _frameProvider_NavigationFrameInitialized(object sender, EventArgs e)
        {
            _frameProvider.NavigationFrame.Navigating += OnNavigating;
            _frameProvider.NavigationFrame.Navigated += OnNavigated;
        }

        protected virtual void OnNavigating(object sender, NavigatingCancelEventArgs e) { }

        protected virtual void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Content == null)
                return;

            if (!(e.Content is Page view))
                throw new ArgumentException("View '" + e.Content.GetType().FullName +
                    "' should inherit from Page or one of its descendents.");

            viewModelBinder.Bind(view, e.Parameter, e.NavigationMode);
        }

        public void SetAdditionalFrame(Frame frame)
        {
            if(_additionalFrame != null)
            {
                _additionalFrame.Navigating -= OnNavigating;
                _additionalFrame.Navigated -= OnNavigated;
            }
            _additionalFrame = frame;
            _additionalFrame.Navigating += OnNavigating;
            _additionalFrame.Navigated += OnNavigated;
        }

        public bool NavigateFromAdditionalFrame<TView>() where TView : Page
        {
            /*if (_currentTypePage == null || _currentTypePage.FullName != typeof(TView).FullName)
            {
                _currentTypePage = typeof(TView);
            }
            else
            {
                return false;
            }*/
            return _additionalFrame.Navigate(typeof(TView));
        }

        public bool NavigateFromAdditionalFrame<TView>(Dictionary<string, object> parameter = null) where TView : Page
        {
            /*if (_currentTypePage == null || _currentTypePage.FullName != typeof(TView).FullName)
            {
                _currentTypePage = typeof(TView);
            }
            else
            {
                return false;
            }*/
            var context = new NavigationContext(parameter);
            return _additionalFrame.Navigate(typeof(TView), context);
        }

        public void NavigateBackFromAdditionalFrame()
        {
            if (_additionalFrame.CanGoBack) _additionalFrame.GoBack();
        }

        public bool NavigateFromChildFrame<TView>() where TView : Page
        {
            /*if(_currentTypePage == null || _currentTypePage.FullName != typeof(TView).FullName)
            {
                _currentTypePage = typeof(TView);
            }
            else
            {
                return false;
            }*/
            return _navigationFrame.Navigate(typeof(TView));
        }

        public bool NavigateFromChildFrame<TView>(Dictionary<string, object> parameter = null) where TView : Page
        {
            /*if (_currentTypePage == null || _currentTypePage.FullName != typeof(TView).FullName)
            {
                _currentTypePage = typeof(TView);
            }
            else
            {
                return false;
            }*/
            var context = new NavigationContext(parameter);
            return _navigationFrame.Navigate(typeof(TView), context);
        }

        public void NavigateBackFromChildFrame()
        {
            if (_navigationFrame.CanGoBack) _navigationFrame.GoBack();
        }

        public bool Navigate<TView>() where TView : Page
        {
            return _appFrame.Navigate(typeof(TView));
        }

        public bool Navigate<TView>(Dictionary<string, object> parameter = null) where TView : Page
        {
            var context = new NavigationContext(parameter);
            return _appFrame.Navigate(typeof(TView), context);
        }

        public void NavigateBack()
        {
            if (_appFrame.CanGoBack) _appFrame.GoBack();
        }
    }
}
