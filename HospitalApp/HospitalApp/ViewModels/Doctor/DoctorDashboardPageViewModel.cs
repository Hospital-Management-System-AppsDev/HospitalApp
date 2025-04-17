using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;
using HospitalApp.Services;

namespace HospitalApp.ViewModels;

public partial class DoctorDashboardPageViewModel:ViewModelBase
{
    private readonly UserSessionService _session = UserSessionService.Instance;
    private readonly MainWindowViewModel _mainViewModel; // Reference to main window view model

    [ObservableProperty]
private Appointment? _selectedAppointment;
    
    [ObservableProperty]
    private User _currentUser;

    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;

    [ObservableProperty]
    private ObservableCollection<Appointment> appointments = new();
    [ObservableProperty]
    private ObservableCollection<Appointment> appointmentsToday = new();

    [ObservableProperty]
    private bool msgIsVisible = false;
    

    public DoctorDashboardPageViewModel(ApiService api, SignalRService signalR, MainWindowViewModel mainViewModel){
        _mainViewModel = mainViewModel;
        _apiService = api;
        _signalRService = signalR;
        _currentUser = _session.CurrentUser;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await ConnectToSignalR();
        await LoadPatientsAppointment();
    }
    private async Task ConnectToSignalR()
    {
        await _signalRService.ConnectAsync();
    }

    private async Task LoadPatientsAppointment()
    {
        var appointmentsList = await _apiService.GetAppointmentsByDoctorAsync(CurrentUser.Id);
        Appointments = new ObservableCollection<Appointment>(appointmentsList);

        Console.WriteLine("Appointments for Doctor ID: " + CurrentUser.Id);
        foreach (var appointment in appointmentsList)
        {
            Console.WriteLine($"Appointment ID: {appointment.PkId}, Patient: {appointment.PatientName}, Date: {appointment.AppointmentDateTime}, Status: {appointment.Status}");
        }

        var today = Appointments.Where(a => a.Status != 1).Where(a => a.AppointmentDateForPicker == DateTime.UtcNow.Date)
                .OrderBy(a => a.AppointmentDateTime)
                .ToList();

        AppointmentsToday = new ObservableCollection<Appointment>(today);

        if(AppointmentsToday.Count == 0){
            MsgIsVisible = true;
        }else{
            MsgIsVisible = false;
        }
        
    }

    [RelayCommand]
    private void AppointmentClicked(Appointment? selectedAppointment)
    {
        if (selectedAppointment is null)
            return;

        Console.WriteLine($"Appointment clicked: {selectedAppointment.PatientName}");
        _mainViewModel.NavigateToAppointmentDetails(selectedAppointment);
    }


}
