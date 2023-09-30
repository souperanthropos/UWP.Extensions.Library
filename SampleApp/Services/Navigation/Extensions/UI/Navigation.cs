using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace SampleApp.Services.Navigation.Extensions.UI
{
    public class Navigation : DependencyObject
    {
        public static readonly DependencyProperty RegisterRouteProperty =
            DependencyProperty.RegisterAttached(
                "RegisterRoute",
                typeof(string),
                typeof(Navigation),
                new PropertyMetadata(null, OnRegisterRouteChanged));

        public static void SetRegisterRoute(UIElement element, string value = "")
        {
            element.SetValue(RegisterRouteProperty, value);
        }

        public static string GetRegisterRoute(UIElement element)
        {
            return (string)element.GetValue(RegisterRouteProperty);
        }

        private static void OnRegisterRouteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Frame element && e.NewValue != null)
            {
                var navigator = Ioc.Default.GetService<INavigationService>();
                navigator.RegisterRoute(element, GetRegisterRoute(element));
            }
        }
    }
}
