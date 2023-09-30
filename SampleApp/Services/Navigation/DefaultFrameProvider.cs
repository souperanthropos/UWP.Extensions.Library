using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SampleApp.Services.Navigation
{
    public class DefaultFrameProvider : IFrameProvider
    {
        public Frame CurrentFrame
        {
            get
            {
                return (Window.Current.Content as Frame);
            }
        }
    }
}
