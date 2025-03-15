using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HospitalApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPaneOpen = false;

    [RelayCommand]
    private void TriggerPane(){
        IsPaneOpen = !IsPaneOpen;
    }
}