using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using UWP.Extensions.Library.Services.Navigation.Interfaces;

namespace UWP.Extensions.Library.Services.Navigation.Extensions.UI
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

        public static readonly DependencyProperty DIProviderProperty =
            DependencyProperty.RegisterAttached(
                "DIProvider",
                typeof(IDependencyInjectionProvider),
                typeof(Navigation),
                new PropertyMetadata(null, OnRegisterRouteChanged));

        public static void SetDIProvider(UIElement element, IDependencyInjectionProvider provider = null)
        {
            element.SetValue(DIProviderProperty, provider);
        }

        public static IDependencyInjectionProvider GetDIProvider(UIElement element)
        {
            return (IDependencyInjectionProvider)element.GetValue(DIProviderProperty);
        }

        private static void OnRegisterRouteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Frame element && e.NewValue != null)
            {
                var provider = GetDIProvider(element);
                if (provider != null)
                {
                    var navigator = provider.GetNavigationService();
                    navigator.RegisterRoute(element, GetRegisterRoute(element));
                }
            }
        }
    }
}
