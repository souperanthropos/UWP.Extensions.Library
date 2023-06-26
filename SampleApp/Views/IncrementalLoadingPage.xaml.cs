using SampleApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace SampleApp.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class IncrementalLoadingPage : Page
    {
        public IncrementalLoadingPage()
        {
            this.InitializeComponent();
        }

        private void GrouppingCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is IncrementalLoadingViewModel viewModel)
            {
                viewModel.GrouppingCheckBoxClickedCommand?.Execute(GrouppingCheckBox.IsChecked);
            }
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (DataContext is IncrementalLoadingViewModel viewModel)
            {
                viewModel.SearchTextChangedCommand?.Execute(sender.Text);
            }
        }
    }
}
