using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SampleApp.Services.Navigation
{
    public class DefaultFrameProvider : IFrameProvider
    {
        public event EventHandler NavigationFrameInitialized;

        public Frame CurrentFrame
        {
            get
            {
                return (Window.Current.Content as Frame);
            }
        }

        private Frame _navigationFrame;
        public Frame NavigationFrame
        {
            get
            {
                if (_navigationFrame == null && CurrentFrame.Content is Page page && page.Content is Microsoft.UI.Xaml.Controls.NavigationView nv)
                {
                    _navigationFrame = nv.Content as Frame;
                    NavigationFrameInitialized?.Invoke(this, EventArgs.Empty);
                }
                return _navigationFrame;
            }
        }
    }
}
