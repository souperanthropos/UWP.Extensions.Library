using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWP.Extensions.Library.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IViewModelBinder viewModelBinder;
        private readonly Dictionary<string, Frame> _routeMap;

        private const string _defaultFrameRouteName = "AppFrame";

        public NavigationService(IFrameProvider frameProvider, IViewModelBinder viewModelBinder)
        {
            _routeMap = new Dictionary<string, Frame>
            {
                [_defaultFrameRouteName] = frameProvider.CurrentFrame
            };

            var appFrame = frameProvider.CurrentFrame;
            appFrame.Navigating += OnNavigating;
            appFrame.Navigated += OnNavigated;
            this.viewModelBinder = viewModelBinder;
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

        public void RegisterRoute(Frame frame, string routeName)
        {
            if (frame != null)
            {
                if (_routeMap.ContainsKey(routeName))
                {
                    throw new Exception($"Route name {routeName} is already registered");
                }
                else
                {
                    _routeMap.Add(routeName, frame);
                }
                frame.Navigating += OnNavigating;
                frame.Navigated += OnNavigated;
            }
        }

        public bool Navigate<TView>(string routeName = "") where TView : Page
        {
            if (string.IsNullOrEmpty(routeName))
            {
                routeName = _defaultFrameRouteName;
            }

            if (_routeMap.ContainsKey(routeName))
            {
                var frame = _routeMap[routeName];
                return frame.Navigate(typeof(TView));
            }
            throw new Exception($"Route name {routeName} is not registered");
        }

        public bool Navigate<TView>(Dictionary<string, object> parameter, string routeName = "") where TView : Page
        {
            var context = new NavigationContext(parameter);
            if (string.IsNullOrEmpty(routeName))
            {
                routeName = _defaultFrameRouteName;
            }

            if (_routeMap.ContainsKey(routeName))
            {
                var frame = _routeMap[routeName];
                return frame.Navigate(typeof(TView), context);
            }
            throw new Exception($"Route name {routeName} is not registered");
        }

        public void NavigateBack(string routeName = "")
        {
            if (string.IsNullOrEmpty(routeName))
            {
                routeName = _defaultFrameRouteName;
            }

            if (_routeMap.ContainsKey(routeName))
            {
                var frame = _routeMap[routeName];
                if (frame.CanGoBack) frame.GoBack();
                return;
            }
            throw new Exception($"Route name {routeName} is not registered");
        }
    }
}
