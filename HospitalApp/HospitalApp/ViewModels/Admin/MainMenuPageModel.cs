using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Services;

namespace HospitalApp.ViewModels;

public partial class MainMenuViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly ApiService _apiService = new();
    private readonly SignalRService _signalRService = new(); // Keep a single instance

    [ObservableProperty]
    private bool _isPaneOpen = false;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private ListItemTemplate? _selectedListItem;



    public MainMenuViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        // Ensure LINQ is available for FirstOrDefault()
        var dashboardItem = Items.FirstOrDefault(item => item.ModelType == typeof(DashboardPageViewModel));
        if (dashboardItem is not null)
        {
            SelectedListItem = dashboardItem; // This will trigger OnSelectedListItemChanged
        }
    }

    public MainMenuViewModel()
    {
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;

        LoadViewModelAsync(value);
    }


    private async void LoadViewModelAsync(ListItemTemplate value)
    {
    ViewModelBase? instance = value.ModelType switch
    {
        Type t when t == typeof(DashboardPageViewModel) => new DashboardPageViewModel(_apiService, _signalRService),
        Type t when t == typeof(AppointmentsPageViewModel) => new AppointmentsPageViewModel(_apiService, _signalRService),
        Type t when t == typeof(PharmacyPageViewModel) => new PharmacyPageViewModel(),
        Type t when t == typeof(SettingsPageViewModel) => new SettingsPageViewModel(),
        _ => null
    };

    if (instance is AppointmentsPageViewModel apptVM)
    {
        await apptVM.LoadDataAsync();
    }

    if (instance is not null)
    {
        CurrentPage = instance;
    }
    }



    public ObservableCollection<ListItemTemplate> Items { get; } = new()
    {
        new ListItemTemplate(typeof(DashboardPageViewModel), "Dashboard"),
        new ListItemTemplate(typeof(AppointmentsPageViewModel), "Appointments"),
        new ListItemTemplate(typeof(PharmacyPageViewModel), "Pharmacy"),
        new ListItemTemplate(typeof(SettingsPageViewModel), "Settings"),
    };

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
    
    [RelayCommand]
        private void Logout()
    {
        _mainViewModel.NavigateToLogin(); // Redirect to Login Page
    }
}

public class ListItemTemplate
{
    public ListItemTemplate(Type type, string iconKey)
    {
        ModelType = type;
        Label = type.Name.Replace("PageViewModel", string.Empty);
        
        Application.Current!.TryFindResource(iconKey, out var res);
        ListItemIcon = (StreamGeometry)res!;
    }

    public string Label { get; set; }
    public Type ModelType { get; set; }
    public StreamGeometry ListItemIcon { get; } // Path to SVG Icon
}