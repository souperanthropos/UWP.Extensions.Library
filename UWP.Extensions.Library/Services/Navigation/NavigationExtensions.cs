using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace UWP.Extensions.Library.Services.Navigation
{
    public class NavigationContext
    {
        public NavigationContext(Dictionary<string, object> parameter = null)
        {
            Parameter = parameter;
        }
        public Dictionary<string, object> Parameter { get; private set; }
    }

    public static class NavigationExtensions
    {
        public static bool Navigate<TView>(this Frame frame) where TView : Page
        {
            return frame.Navigate(typeof(TView));
        }

        public static bool Navigate<TView>(this Frame frame, Dictionary<string, object> parameter = null) where TView : Page
        {
            var context = new NavigationContext(parameter);
            return frame.Navigate(typeof(TView), context);
        }
    }
}
