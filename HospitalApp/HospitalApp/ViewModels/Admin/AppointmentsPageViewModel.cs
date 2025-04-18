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
    public DateTime Today => DateTime.Now;

    public Window ParentWindow { get; set; }

    [ObservableProperty]
    private string appointmentSearchText;
    public ObservableCollection<Appointment> FilteredAppointments { get; set; } = new();

    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;

    public ObservableCollection<Appointment> UpcomingAppointments { get; set; } = new();
    public ObservableCollection<Appointment> DoneAppointments { get; set; } = new();

    public ObservableCollection<Doctor> DoctorsList { get; set; } = new();
    public ObservableCollection<string> AppointmentTypes { get; set; } = new() { "Check up", "Consultation", "Surgery" };
    public ObservableCollection<int> StatusOptions { get; set; } = new() { 0, 1, 2 };

    public ObservableCollection<TimeSpan> AvailableTimeSlots { get; set; } = new();
    
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

    [ObservableProperty]
    private string errorMsgCreate;

    [ObservableProperty]
    private bool errorMsgCreateVisible = false;

    [ObservableProperty]
    private string errorMsgFind;

    [ObservableProperty]
    private bool errorMsgFindVisible = false;

    public AppointmentsPageViewModel(ApiService api, SignalRService signalR)
    {
        _apiService = api;
        _signalRService = signalR;

        _signalRService.AppointmentUpdated += OnAppointmentUpdated;
        _signalRService.AppointmentAdded += OnAppointmentAdded; // Add this line

        ConnectToSignalR();
        DoctorsList = new ObservableCollection<Doctor>();
    }

    
    private async void ConnectToSignalR()
    {
        await _signalRService.ConnectAsync();
    }

    public async Task LoadDataAsync()
    {
        IsLoading = true;
        
        try
        {
            // Load doctors
            var doctors = await _apiService.GetDoctorsAsync();
            
            DoctorsList.Clear();
            foreach (var doctor in doctors)
            {
                DoctorsList.Add(doctor);
            }

            // Load appointments
            var appointments = await _apiService.GetAppointmentsAsync();
            
            var upcoming = appointments
                .Where(a => a.Status != 1)
                .OrderBy(a => a.AppointmentDateTime)
                .ToList();

            var done = appointments
                .Where(a => a.Status == 1)
                .OrderBy(a => a.AppointmentDateTime)
                .ToList();

            // Update collections on UI thread
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                UpcomingAppointments.Clear();
                foreach (var apt in upcoming)
                {
                    UpcomingAppointments.Add(apt);
                }

                DoneAppointments.Clear();
                foreach (var apt in done)
                {
                    DoneAppointments.Add(apt);
                }
                FilteredAppointments.Clear();
                foreach (var apt in upcoming)
                    FilteredAppointments.Add(apt);

            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
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
        // Clear any previous error messages
        ErrorMsgFind = "";
        ErrorMsgFindVisible = false;

        if (string.IsNullOrWhiteSpace(pId))
        {
            ErrorMsgFind = "Please enter a patient ID.";
            ErrorMsgFindVisible = true;
            return;
        }

        if (!Regex.IsMatch(pId, @"^\d+$"))
        {
            ErrorMsgFind = "Please enter a valid numeric ID.";
            ErrorMsgFindVisible = true;
            return;
        }

        try
        {
            IsLoading = true;
            PatientId = int.Parse(pId);
            Patient patient = await _apiService.GetPatientAsync(PatientId);

            if (patient == null)
            {
                ErrorMsgFind = $"Patient with ID {PatientId} not found.";
                ErrorMsgFindVisible = true;
                newPatient = null; // Clear any previous patient data
                return;
            }

            newPatient = patient;
            Console.WriteLine($"Patient loaded: {newPatient.Name}");
            ErrorMsgFindVisible = false;
        }
        catch (Exception ex)
        {
            ErrorMsgFind = $"Error: Patient not Found";
            ErrorMsgFindVisible = true;
            Console.WriteLine($"Error fetching patient: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task OnGetAvailableSlotsCommand()
    {
        AvailableTimeSlots.Clear();

        ErrorMsgCreateVisible = false;

        // Validate inputs
        if (Doc == null)
        {
            ErrorMsgCreate = "Please select a doctor before checking slots.";
            ErrorMsgCreateVisible = true;
            return;
        }

        if (!SelectedAppointmentDate.HasValue)
        {
            ErrorMsgCreate = "Please select a date.";
            ErrorMsgCreateVisible = true;
            return;
        }

        if (string.IsNullOrEmpty(SelectedAppointmentType))
        {
            ErrorMsgCreate = "Please select an appointment type.";
            ErrorMsgCreateVisible = true;
            return;
        }

        try
        {
            var doctorId = Doc.Id;
            var date = SelectedAppointmentDate.Value;
            var type = SelectedAppointmentType.ToLower();

            var availableSlots = await _apiService.GetAvailableTime(doctorId, date, type);

            if(Doc.is_available == 0 && SelectedAppointmentDate.Value.Date == DateTime.Now.Date){
                ErrorMsgCreate = "Doctor is unavailable on the date selected.";
                ErrorMsgCreateVisible = true;
                return;
            }

            if (availableSlots == null || !availableSlots.Any())
            {
                ErrorMsgCreate = "No available slots found for the selected date and type.";
                ErrorMsgCreateVisible = true;
                return;
            }

            // Convert DateTime list to TimeSpan list (only time part for UI)
            if(SelectedAppointmentDate == Today.Date){
                foreach (var dt in availableSlots)
                {
                    if(dt.TimeOfDay > Today.TimeOfDay){
                        AvailableTimeSlots.Add(dt.TimeOfDay);
                    }
                }
            }else{
                foreach (var dt in availableSlots)
                {
                    AvailableTimeSlots.Add(dt.TimeOfDay);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMsgCreate = $"Error fetching slots: {ex.Message}";
            ErrorMsgCreateVisible = true;
        }
    }



    [RelayCommand]
    public async Task OnCreateAppointment()
    {
        if (newPatient == null)
        {
            ErrorMsgCreate = "Error: No patient selected.";
            ErrorMsgCreateVisible = true;
            return;
        }

        if (Doc == null)
        {
            ErrorMsgCreate = "Error: No doctor assigned.";
            ErrorMsgCreateVisible = true;
            return;
        }

        if (!SelectedAppointmentDate.HasValue)
        {
            ErrorMsgCreate = "Error: No date selected.";
            ErrorMsgCreateVisible = true;
            return;
        }

        // Add this validation for time slot
        if (SelectedAppointmentTime == default(TimeSpan))
        {
            ErrorMsgCreate = "Error: No time slot selected.";
            ErrorMsgCreateVisible = true;
            return;
        }

        ErrorMsgCreateVisible = false;

    newAppointment = new Appointment
    {
        PatientID = newPatient.PatientID,
        PatientName = newPatient.Name,
        AssignedDoctor = Doc,
        AppointmentType = SelectedAppointmentType,
        Status = 0,
        AppointmentDateTime = SelectedAppointmentDate.Value.Date + SelectedAppointmentTime
    };

    try
    {
        // Send to the API
        var createdAppointment = await _apiService.AddAppointmentAsync(newAppointment);
        
        if (createdAppointment != null)
        {
            if (createdAppointment.AppointmentDateTime == default)
            {
                createdAppointment.AppointmentDateTime = SelectedAppointmentDate.Value.Date + SelectedAppointmentTime;
            }

            // Preserve other data as before...
            if (string.IsNullOrEmpty(createdAppointment.AppointmentType))
                createdAppointment.AppointmentType = SelectedAppointmentType;

            if (string.IsNullOrEmpty(createdAppointment.PatientName))
                createdAppointment.PatientName = newPatient.Name;

            if (createdAppointment.PatientID == 0)
                createdAppointment.PatientID = newPatient.PatientID;

            if (createdAppointment.AssignedDoctor == null || string.IsNullOrEmpty(createdAppointment.AssignedDoctor.Name))
                createdAppointment.AssignedDoctor = Doc;

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                UpcomingAppointments.Add(createdAppointment);
                FilteredAppointments.Add(createdAppointment);
            });

            ClearAppointmentForm();
        }
        else
        {
            ErrorMsgCreate = "Error creating appointment. Please try again.";
            ErrorMsgCreateVisible = true;
        }
    }
    catch (Exception ex)
    {
        ErrorMsgCreate = $"Error: {ex.Message}";
        ErrorMsgCreateVisible = true;
    }
    }
    private void ClearAppointmentForm()
    {
        // Reset form fields
        SelectedAppointmentType = null;
        SelectedAppointmentDate = null;
        Doc = null;
        // Don't clear patient info if you want to add multiple appointments for the same patient
    }
    
    private async void OnAppointmentUpdated(int appointmentId, Appointment updatedAppointment)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            // Make sure the doctor reference is complete
            if (updatedAppointment.AssignedDoctor == null || string.IsNullOrEmpty(updatedAppointment.AssignedDoctor.Name))
            {
                var doctor = DoctorsList.FirstOrDefault(d => d.Id == updatedAppointment.AssignedDoctor?.Id);
                if (doctor != null)
                {
                    updatedAppointment.AssignedDoctor = doctor;
                }
            }
            
            var existing = UpcomingAppointments.FirstOrDefault(a => a.pkId == appointmentId);
            if (existing != null)
            {
                existing.AppointmentDateTime = updatedAppointment.AppointmentDateTime;
                existing.Status = updatedAppointment.Status;
                existing.AssignedDoctor = updatedAppointment.AssignedDoctor;
                existing.AppointmentType = updatedAppointment.AppointmentType;
                existing.PatientName = updatedAppointment.PatientName ?? existing.PatientName;
                existing.PatientID = updatedAppointment.PatientID > 0 ? updatedAppointment.PatientID : existing.PatientID;

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
        }
    );


    }

   private void OnAppointmentAdded(Appointment appointment)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            // If the appointment doesn't have a doctor assigned but we have a match in our local doctor list
            if (appointment.AssignedDoctor == null || string.IsNullOrEmpty(appointment.AssignedDoctor.Name))
            {
                if (newAppointment != null && newAppointment.AssignedDoctor != null)
                {
                    appointment.AssignedDoctor = newAppointment.AssignedDoctor;
                }
                else if (appointment.AssignedDoctor?.Id > 0)
                {
                    var doctor = DoctorsList.FirstOrDefault(d => d.Id == appointment.AssignedDoctor.Id);
                    if (doctor != null)
                    {
                        appointment.AssignedDoctor = doctor;
                    }
                }
            }
            
            // Make sure appointment type is not empty
            if (string.IsNullOrEmpty(appointment.AppointmentType) && newAppointment != null)
            {
                appointment.AppointmentType = newAppointment.AppointmentType;
            }
            
            // Make sure patient name is not empty
            if (string.IsNullOrEmpty(appointment.PatientName) && newAppointment != null)
            {
                appointment.PatientName = newAppointment.PatientName;
            }
            
            // Make sure patient ID is not zero
            if (appointment.PatientID == 0 && newAppointment != null)
            {
                appointment.PatientID = newAppointment.PatientID;
            }
            
            // Add to the appropriate collection based on status
            if (appointment.Status == 1)
            {
                DoneAppointments.Add(appointment);
            }
            else
            {
                UpcomingAppointments.Add(appointment);
            }
        });
    }

    public void FilterAppointments()
    {
        if (string.IsNullOrWhiteSpace(AppointmentSearchText))
        {
            // No search input, show all upcoming appointments
            FilteredAppointments.Clear();
            foreach (var apt in UpcomingAppointments)
                FilteredAppointments.Add(apt);
            return;
        }

        var searchText = AppointmentSearchText.Trim().ToLower();

        var filtered = UpcomingAppointments.Where(apt =>
            apt.PatientName?.ToLower().Contains(searchText) == true ||
            apt.AppointmentType?.ToLower().Contains(searchText) == true ||
            apt.AssignedDoctor?.Name?.ToLower().Contains(searchText) == true
        );

        FilteredAppointments.Clear();
        foreach (var apt in filtered)
            FilteredAppointments.Add(apt);
    }

    partial void OnAppointmentSearchTextChanged(string value)
    {
        FilterAppointments();
    }




}
}


