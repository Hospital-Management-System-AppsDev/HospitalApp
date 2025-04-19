using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HospitalApp.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _fullName;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _selectedTheme;

    [ObservableProperty]
    private bool _enableNotifications;

    [ObservableProperty]
    private string _selectedLanguage;

    public SettingsPageViewModel()
    {
    }

    [RelayCommand]
    private async Task SavePersonalInfo()
    {
        // TODO: Implement logic to save personal information
        // Validate inputs
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Username) || 
            string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            // TODO: Show error message
            return;
        }

        // Simulate saving to a service or database
        await Task.Delay(500); // Simulate async operation
        // TODO: Add actual save logic
    }

    [RelayCommand]
    private async Task SaveAppSettings()
    {
        // TODO: Implement logic to save application settings
        // Apply theme
        // Update notification settings
        // Update language
        await Task.Delay(500); // Simulate async operation
        // TODO: Add actual save logic
    }
}