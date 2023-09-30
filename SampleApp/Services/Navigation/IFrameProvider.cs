using System;
using Windows.UI.Xaml.Controls;

namespace SampleApp.Services.Navigation
{
    public interface IFrameProvider
    {
        Frame CurrentFrame { get; }
    }
}
