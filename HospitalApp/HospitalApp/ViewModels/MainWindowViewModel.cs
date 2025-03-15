using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HospitalApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPaneOpen = false;
    
    [ObservableProperty]
    private ViewModelBase _currentPage = new DashboardPageViewModel(); // Ensure it is initialized

    public ObservableCollection<ListItemTemplate> Items { get; } = new(){
        new ListItemTemplate(typeof(DashboardPageViewModel)),
        new ListItemTemplate(typeof(AppointmentsPageViewModel)), 
    };

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}

public class ListItemTemplate
{
    public ListItemTemplate(Type type)
    {
        ModelType = type;
        Label = type.Name.Replace("PageViewModel", string.Empty);
    }
    
    public string Label { get; set; }
    public Type ModelType { get; set; }
    
}