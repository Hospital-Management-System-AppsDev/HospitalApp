using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Avalonia.Layout;
using Avalonia.Controls.ApplicationLifetimes;
using HospitalApp.Services;

namespace HospitalApp.ViewModels;

public partial class DoctorMainMenuPageViewModel : ViewModelBase
{
    private readonly UserSessionService _session = UserSessionService.Instance;
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
            Type t when t == typeof(DoctorAppointmentPageViewModel) => new DoctorAppointmentPageViewModel(_apiService, _signalRService),
            _ => null
        };

        if (instance is AppointmentsPageViewModel apptVM)
        {
            await apptVM.LoadDataAsync();
        }
        else if (instance is DoctorDashboardPageViewModel dashboardVM)
        {
            // If we're returning to the dashboard, refresh the charts
            await dashboardVM.RefreshChartsAsync();
        }

        if (instance is not null)
        {
            CurrentPage = instance;
        }
    }


    public ObservableCollection<ListItemTemplate> Items { get; } = new()
    {
        new ListItemTemplate(typeof(DoctorDashboardPageViewModel), "Dashboard", "Dashboard"),
        new ListItemTemplate(typeof(DoctorAppointmentPageViewModel), "Appointments", "Appointments"),
    };

    [RelayCommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    [RelayCommand]
    private async Task Logout()
    {
        Window messageWindow = new Window
        {
            Title = "Confirm Logout",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var yesButton = new Button
        {
            Content = "Yes",
            Width = 80
        };

        var noButton = new Button
        {
            Content = "No",
            Width = 80
        };

        yesButton.Command = new RelayCommand(async () =>
        {
            messageWindow.Close();
            await _apiService.LogoutAsync(_session.CurrentUser.Id);
            _mainViewModel.NavigateToLogin();
        });

        noButton.Command = new RelayCommand(() => messageWindow.Close());

        messageWindow.Content = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 15,
            Children =
            {
                new TextBlock
                {
                    Text = "Are you sure you want to logout?",
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 16,
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 20,
                    Children = { yesButton, noButton }
                }
            }
        };

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var parentWindow = desktop.MainWindow;
            await messageWindow.ShowDialog(parentWindow);
        }
    }
}



