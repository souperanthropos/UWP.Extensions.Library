using System;
using Windows.UI.Xaml.Controls;

namespace SampleApp.Services.Navigation
{
    public interface IFrameProvider
    {
        event EventHandler NavigationFrameInitialized;

        Frame CurrentFrame { get; }
        Frame NavigationFrame { get; }
    }
}
