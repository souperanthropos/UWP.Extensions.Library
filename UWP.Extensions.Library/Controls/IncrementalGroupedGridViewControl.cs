using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace UWP.Extensions.Library.Controls
{
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public sealed class IncrementalGroupedGridViewControl : GridView
    {
        public object IncrementalLoadingCollection
        {
            get { return GetValue(IncrementalLoadingCollectionProperty); }
            set { SetValue(IncrementalLoadingCollectionProperty, value); }
        }
        public static readonly DependencyProperty IncrementalLoadingCollectionProperty = DependencyProperty.Register(
            nameof(IncrementalLoadingCollection),
            typeof(object),
            typeof(IncrementalGroupedGridViewControl),
            new PropertyMetadata(null, OnIncrementalLoadingCollectionPropertyChanged));

        private static async void OnIncrementalLoadingCollectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IncrementalGroupedGridViewControl viewControl
                && viewControl.IncrementalLoadingCollection is ISupportIncrementalLoading supportIncrementalLoading)
            {
                await supportIncrementalLoading.LoadMoreItemsAsync(0);
            }
        }

        private ScrollViewer _scrollViewer;
        private Panel _rootPanel;

        private ISupportIncrementalLoading _supportIncrementalLoading => IncrementalLoadingCollection as ISupportIncrementalLoading;

        public IncrementalGroupedGridViewControl()
        {
            this.DefaultStyleKey = typeof(IncrementalGroupedGridViewControl);

            this.Loaded += (s, e) =>
            {
                if (_rootPanel == null)
                {
                    _rootPanel = ItemsPanelRoot as Panel;
                }
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");

            _scrollViewer.ViewChanged += async (s, e) =>
            {
                if (!IsGrouping || _rootPanel == null || _supportIncrementalLoading == null || e.IsIntermediate) return;
                double distanceFromBottom = _rootPanel.ActualHeight - _scrollViewer.VerticalOffset - _scrollViewer.ActualHeight;
                if (distanceFromBottom < 100)
                {
                    if (_supportIncrementalLoading.HasMoreItems)
                    {
                        await _supportIncrementalLoading.LoadMoreItemsAsync(0);
                    }
                    Debug.WriteLine("distanceFromBottom < 100");
                }
            };
        }
    }
}
