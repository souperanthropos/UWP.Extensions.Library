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
        public ISupportIncrementalLoading IncrementalLoadingCollection
        {
            get { return (ISupportIncrementalLoading)GetValue(IncrementalLoadingCollectionProperty); }
            set { SetValue(IncrementalLoadingCollectionProperty, value); }
        }
        public static readonly DependencyProperty IncrementalLoadingCollectionProperty = DependencyProperty.Register(
            nameof(IncrementalLoadingCollection),
            typeof(ISupportIncrementalLoading),
            typeof(IncrementalGroupedGridViewControl),
            new PropertyMetadata(null, OnSourcePropertyChanged));

        private static async void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IncrementalGroupedGridViewControl viewControl
                && viewControl.IncrementalLoadingCollection != null)
            {
                await viewControl.IncrementalLoadingCollection.LoadMoreItemsAsync(0);
            }
        }

        private ScrollViewer _scrollViewer;
        private Panel _rootPanel;

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
                if (!IsGrouping || _rootPanel == null || IncrementalLoadingCollection == null || e.IsIntermediate) return;
                double distanceFromBottom = _rootPanel.ActualHeight - _scrollViewer.VerticalOffset - _scrollViewer.ActualHeight;
                if (distanceFromBottom < 100)
                {
                    if (IncrementalLoadingCollection.HasMoreItems)
                    {
                        await IncrementalLoadingCollection.LoadMoreItemsAsync(0);
                    }
                    Debug.WriteLine("distanceFromBottom < 100");
                }
            };
        }
    }
}
