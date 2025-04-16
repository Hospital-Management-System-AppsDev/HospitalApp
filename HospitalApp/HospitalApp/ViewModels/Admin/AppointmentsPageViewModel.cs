using System.Collections.ObjectModel;
using HospitalApp.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Input;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Layout;
using HospitalApp.Views;
using Avalonia.Media;


namespace HospitalApp.ViewModels{
public partial class AppointmentsPageViewModel : ViewModelBase
{
    public Window ParentWindow { get; set; }
    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;

    public ObservableCollection<Appointment> UpcomingAppointments { get; set; } = new();
    public ObservableCollection<Appointment> DoneAppointments { get; set; } = new();

    public ObservableCollection<Doctor> DoctorsList { get; set; } = new();
    public ObservableCollection<string> AppointmentTypes { get; set; } = new() { "Check-up", "Consultation", "Surgery" };
    public ObservableCollection<int> StatusOptions { get; set; } = new() { 0, 1, 2 };

    public ObservableCollection<TimeSpan> AvailableTimeSlots { get; set; } = new()
{
    new TimeSpan(9, 0, 0),
    new TimeSpan(10, 0, 0),
    new TimeSpan(11, 0, 0),
    new TimeSpan(14, 0, 0),
    new TimeSpan(15, 0, 0),
};
    [ObservableProperty]
    private string pId;
    private int _patientId;
    public int PatientId
    {
        get => _patientId;
        set => SetProperty(ref _patientId, value); // using ObservableObject or CommunityToolkit
    }

    private Patient _patient;
    public Patient newPatient{
        get => _patient;
        set => SetProperty(ref _patient, value); // using ObservableObject or CommunityToolkit
    }

    private Appointment _appointment;
    public Appointment newAppointment{
        get => _appointment;
        set => SetProperty(ref _appointment, value); // using ObservableObject or CommunityToolkit
    }

    private Doctor _doc;
    public Doctor Doc{
        get => _doc;
        set => SetProperty(ref _doc, value);
    }

    [ObservableProperty]
    private string selectedAppointmentType;

    [ObservableProperty]
    private DateTime? selectedAppointmentDate;

    [ObservableProperty]
    private TimeSpan selectedAppointmentTime;



    public AppointmentsPageViewModel(ApiService api, SignalRService signalR)
    {
        _apiService = api;
        _signalRService = signalR;

        _signalRService.AppointmentUpdated += OnAppointmentUpdated;
        DoctorsList = new ObservableCollection<Doctor>();
    }

    public async Task LoadDataAsync()
    {
        // Load doctors first
        var doctors = await _apiService.GetDoctorsAsync();
        DoctorsList = new ObservableCollection<Doctor>(doctors);

        // Add dummy appointments now that DoctorsList is available
        var appointments = await _apiService.GetAppointmentsAsync();
        var allAppointments = new ObservableCollection<Appointment>(appointments);

        var filtered = allAppointments
            .Where(a => a.Status != 1)
            .OrderBy(a => a.AppointmentDateTime)
            .ToList();

        var done = allAppointments
            .Where(a => a.Status == 1)
            .OrderBy(a => a.AppointmentDateTime)
            .ToList();

        UpcomingAppointments = new ObservableCollection<Appointment>(filtered);
        DoneAppointments = new ObservableCollection<Appointment>(done);
    }

    [ObservableProperty]
    private bool isLoading;

    [RelayCommand]
    public async Task OnEditCommand(Appointment appointment)
    {
        // Get the current window reference from somewhere accessible
        var parentWindow = ParentWindow; // Assign the ParentWindow property or another valid window reference
        if (ParentWindow == null)
        {
            Console.WriteLine("Error: Parent window not available");
            return;
        }
        
        var viewModel = new EditAppointmentWindowViewModel(appointment, DoctorsList);
        var window = new EditAppointmentWindow(viewModel, _apiService);
        var editedAppointment = await window.ShowDialog<Appointment>(parentWindow);

        if (editedAppointment != null)
        {
            bool success = await _apiService.UpdateAppointment(editedAppointment.pkId, editedAppointment);
            if (success) await LoadDataAsync();
        }
    }

    private Task<bool> ShowConfirmationDialog(string title, string message)
{
    var tcs = new TaskCompletionSource<bool>();

    Window confirmationWindow = null; // Declare first

    confirmationWindow = new Window
    {
        Title = title,
        Width = 350,
        Height = 180,
        WindowStartupLocation = WindowStartupLocation.CenterOwner,
        CanResize = false,
        Content = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 15,
            Children =
            {
                new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 16,
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 20,
                    Children =
                    {
                        new Button
                        {
                            Content = "Yes",
                            Width = 80,
                            Command = new RelayCommand<object>(_ =>
                            {
                                tcs.SetResult(true);
                                confirmationWindow.Close();
                            })
                        },
                        new Button
                        {
                            Content = "No",
                            Width = 80,
                            Command = new RelayCommand<object>(_ =>
                            {
                                tcs.SetResult(false);
                                confirmationWindow.Close();
                            })
                        }
                    }
                }
            }
        }
    };

    confirmationWindow.ShowDialog(ParentWindow);

    return tcs.Task;
}

    [RelayCommand]
    public async Task OnCancelCommand(Appointment appointment)
    {   
        try
        {
            var result = await ShowConfirmationDialog("Cancel Appointmet", "Are you sure you want to cancel this appointment?");

            if (!result)
            {
                Console.WriteLine("Cancel operation is cancelled by user.");
                return;
            }

            await _apiService.CancelAppointment(appointment.pkId);
            UpcomingAppointments.Remove(appointment);
            await LoadDataAsync();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }


    [RelayCommand]
    public async Task OnGetPatientByIdCommand()
    {
        if (!Regex.IsMatch(pId, @"^\d+$"))
        {
            Console.WriteLine("Please enter a valid numeric ID.");
            return;
        }

        try
        {
            PatientId = int.Parse(pId);
            Patient patient = await _apiService.GetPatientAsync(PatientId);

            if (patient == null)
            {
                Console.WriteLine("Patient not found.");
                return;
            }

            newPatient = patient;
            Console.WriteLine($"Patient loaded: {newPatient.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching patient: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task OnCreateAppointment()
    {
        if (newPatient == null)
        {
            Console.WriteLine("Error: No patient selected.");
            return;
        }

        if (newAppointment == null)
        {
            newAppointment = new Appointment();
        }

        if (Doc == null)
        {
            Console.WriteLine("Error: No doctor assigned.");
            return;
        }

        if (!selectedAppointmentDate.HasValue)
        {
            Console.WriteLine("Error: No date selected.");
            return;
        }

        // Create new appointment manually
        newAppointment = new Appointment
        {
            PatientID = newPatient.PatientID,
            PatientName = newPatient.Name,
            AssignedDoctor = Doc,
            AppointmentType = SelectedAppointmentType,
            Status = 0,
            AppointmentDateTime = selectedAppointmentDate.Value.Date + SelectedAppointmentTime
        };

        UpcomingAppointments.Add(newAppointment);
        await _apiService.AddAppointmentAsync(newAppointment);
        await LoadDataAsync();

        Console.WriteLine($"Id: {newAppointment.PatientID}\nName: {newAppointment.PatientName}\nAge: {newPatient.Age}\nBday: {newPatient.Bday}\nDoctor: {newAppointment.AssignedDoctor.Name}\nAppointmentType: {newAppointment.AppointmentType}\nStatus: {newAppointment.Status}\nAppointmentDateTime: {newAppointment.AppointmentDateTime}");
    }
    
    private async void OnAppointmentUpdated(int appointmentId, Appointment updatedAppointment)
{
    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
{
    var existing = UpcomingAppointments.FirstOrDefault(a => a.pkId == appointmentId);
    if (existing != null)
    {
        existing.AppointmentDateTime = updatedAppointment.AppointmentDateTime;
        existing.Status = updatedAppointment.Status;
        existing.AssignedDoctor = updatedAppointment.AssignedDoctor;
        existing.AppointmentType = updatedAppointment.AppointmentType;

        if (updatedAppointment.Status == 1)
        {
            UpcomingAppointments.Remove(existing);
            DoneAppointments.Add(updatedAppointment);
        }
    }
    else
    {
        var existingInDone = DoneAppointments.FirstOrDefault(a => a.pkId == appointmentId);
        if (existingInDone != null)
        {
            existingInDone.AppointmentDateTime = updatedAppointment.AppointmentDateTime;
            existingInDone.Status = updatedAppointment.Status;
            existingInDone.AssignedDoctor = updatedAppointment.AssignedDoctor;
            existingInDone.AppointmentType = updatedAppointment.AppointmentType;
        }
    }
});


}



}
}


