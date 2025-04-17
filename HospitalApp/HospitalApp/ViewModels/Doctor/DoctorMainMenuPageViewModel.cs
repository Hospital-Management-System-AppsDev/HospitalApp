using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HospitalApp.ViewModels;

public partial class DoctorMainMenuPageViewModel : ViewModelBase
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

    public DoctorMainMenuPageViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        // Ensure LINQ is available for FirstOrDefault()
        var dashboardItem = Items.FirstOrDefault(item => item.ModelType == typeof(DoctorDashboardPageViewModel));
        if (dashboardItem is not null)
        {
            SelectedListItem = dashboardItem; // This will trigger OnSelectedListItemChanged
        }
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
        Type t when t == typeof(DoctorDashboardPageViewModel) => new DoctorDashboardPageViewModel(_apiService, _signalRService, _mainViewModel),
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
        new ListItemTemplate(typeof(DoctorDashboardPageViewModel), "Dashboard"),
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



