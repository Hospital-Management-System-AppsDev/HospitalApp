using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;

namespace HospitalApp.ViewModels;

public partial class AppointmentDetailsViewModel : ViewModelBase
{
    public Appointment Appointment { get; }
    private readonly MainWindowViewModel _mainWindowViewModel;

    public AppointmentDetailsViewModel(MainWindowViewModel mainWindowViewModel, Appointment appointment)
    {
        _mainWindowViewModel = mainWindowViewModel;
        Appointment = appointment;
    }
    [RelayCommand]
    private void BackToDashboard()
    {
        _mainWindowViewModel.NavigateToDoctorsMainMenu();
    }
}
