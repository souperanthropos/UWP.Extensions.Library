## Controls
- IncrementalGroupedGridViewControl
- IncrementalGroupedListViewControl

## Collections
- IncrementalLoadingCollection
- IncrementalLoadingGroupCollection

## Navigation Service
This service supports multiple Frame elements in one application.

**How-To: Navigate Between Pages With Root Frame**
```c#
//call Navigate method without parameters to navigate to the SecondPage
private void GoToSecondPageClick(object sender, RoutedEventArgs e)
{
    _navigationService.Navigate<SecondPage>();
}

//or call Navigate method with parameters to navigate to the SecondPage
private void GoToSecondPageClick(object sender, RoutedEventArgs e)
{
    var parameters = new Dictionary<string, object>
    {
        ["foo"] = "bar"
    };
    _navigationService.Navigate<SecondPage>(parameters);
}
```

**How-To: Navigate Between Pages With Other Frame**
