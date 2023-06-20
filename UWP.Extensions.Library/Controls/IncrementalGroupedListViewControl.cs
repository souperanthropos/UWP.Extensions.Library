using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace UWP.Extensions.Library.Controls
{
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public sealed class IncrementalGroupedListViewControl : ListView
    {
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(object),
            typeof(IncrementalGroupedListViewControl),
            new PropertyMetadata(null, OnSourcePropertyChanged));

        public bool IsSourceGrouped
        {
            get { return (bool)GetValue(IsSourceGroupedProperty); }
            set { SetValue(IsSourceGroupedProperty, value); }
        }
        public static readonly DependencyProperty IsSourceGroupedProperty = DependencyProperty.Register(
            nameof(IsSourceGrouped),
            typeof(bool),
            typeof(IncrementalGroupedListViewControl),
            new PropertyMetadata(false));

        private static async void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IncrementalGroupedListViewControl viewControl
                && viewControl.Source is ISupportIncrementalLoading incrementalLoading)
            {
                if (viewControl.IsSourceGrouped)
                {
                    viewControl._supportIncrementalLoading = incrementalLoading;
                    var result = await viewControl._supportIncrementalLoading.LoadMoreItemsAsync(0);
                }
                else
                {
                    viewControl._supportIncrementalLoading = null;
                }
            }
        }

        private ScrollViewer _scrollViewer;
        private Panel _rootPanel;
        private ISupportIncrementalLoading _supportIncrementalLoading;
        public IncrementalGroupedListViewControl()
        {
            this.DefaultStyleKey = typeof(IncrementalGroupedListViewControl);

            this.Loaded += (s, e) =>
            {
                if (_rootPanel == null)
                {
                    _rootPanel = ItemsPanelRoot;
                }
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");

            _scrollViewer.ViewChanged += async (s, e) =>
            {
                if (!IsSourceGrouped || _rootPanel == null || _supportIncrementalLoading == null || e.IsIntermediate) return;
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
