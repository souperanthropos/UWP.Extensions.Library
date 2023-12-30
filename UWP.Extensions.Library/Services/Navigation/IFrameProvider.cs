using System;
using Windows.UI.Xaml.Controls;

namespace UWP.Extensions.Library.Services.Navigation
{
    public interface IFrameProvider
    {
        Frame CurrentFrame { get; }
    }
}
