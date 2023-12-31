## Controls
- IncrementalGroupedGridViewControl
- IncrementalGroupedListViewControl

## Collections
- IncrementalLoadingCollection
- IncrementalLoadingGroupCollection

## Navigation Service
This service supports multiple Frame elements in one application.

#### How-To: Navigate Between Pages With Root Frame
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


#### How-To: Navigate Between Pages With Second Frame

Add viewmodel with implementation IViewModelBase
```c#
public class ViewModelBase : IViewModelBase
{
    public ViewModelBase() { }

    public virtual void OnNavigationCompleted(Dictionary<string, object> parameter, NavigationMode navigationMode) { }
}
```
Add class with implementation IDependencyInjectionProvider
```c#
internal class DependencyInjectionProvider : IDependencyInjectionProvider
{
    public INavigationService GetNavigationService()
    {
        return Ioc.Default.GetService<INavigationService>();
    }
}
```

Register route and bind IDependencyInjectionProvider implementation for second frame
```xml
<Page xmlns:nuie="using:UWP.Extensions.Library.Services.Navigation.Extensions.UI">

    <Frame x:Name="contentFrame"
           nuie:Navigation.RegisterRoute="contentFrame"
           nuie:Navigation.DIProvider="{x:Bind ViewModel.DIProvider}"/>
</Page>
```

Using second Frame 

```c#
//call Navigate method without parameters to navigate to the SecondPage
private void GoToSecondPageClick(object sender, RoutedEventArgs e)
{
    _navigationService.Navigate<SecondPage>("contentFrame");
}

//or using other Frame call Navigate method with parameters to navigate to the SecondPage
private void GoToSecondPageClick(object sender, RoutedEventArgs e)
{
    var parameters = new Dictionary<string, object>
    {
        ["foo"] = "bar"
    };
    _navigationService.Navigate<SecondPage>(parameters, "contentFrame");
}
```
