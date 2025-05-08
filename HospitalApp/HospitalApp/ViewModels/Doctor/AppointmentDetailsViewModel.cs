using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Email;
using HospitalApp.Models;
using System.IO;
using System.Security.Cryptography;


namespace HospitalApp.ViewModels;

public partial class AppointmentDetailsViewModel : ViewModelBase
{
    private string basePath = GetBasePath();

    [ObservableProperty]
    private bool isLoading;
    public Window ParentWindow { get; set; }

    public Appointment Appointment { get; set; }

    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;

    [ObservableProperty]
    private string symptoms;

    [ObservableProperty]
    private string findings;
    [ObservableProperty]
    private string recommendations;

    [ObservableProperty]
    private string condition;

    [ObservableProperty]
    private string disease;

    [ObservableProperty]
    private string prescription;

    public DateTime Today => DateTime.Today;

    [ObservableProperty]
    private DateTime selectedCertificateDate = DateTime.Today;

    [ObservableProperty]
    public Patient currentPatient;
    private readonly MainWindowViewModel _mainWindowViewModel;

    [ObservableProperty]
    private ObservableCollection<Records> patientRecords = new();

    public AppointmentDetailsViewModel(ApiService api, SignalRService signalR, MainWindowViewModel mainWindowViewModel, Appointment appointment)
    {
        _mainWindowViewModel = mainWindowViewModel;
        Appointment = appointment;
        _apiService = api;
        _signalRService = signalR;

        _signalRService.RecordAdded += OnRecordAdded;

        InitializeAsync();
    }

    private static string GetBasePath(){
        var basePath = AppContext.BaseDirectory;
        // Traverse up the directory structure to get the third "HospitalApp"
        var thirdHospitalAppPath = basePath;

        // Go up 3 levels (you can adjust the number of levels if needed)
        for (int i = 0; i < 4; i++)
        {
            thirdHospitalAppPath = Directory.GetParent(thirdHospitalAppPath).FullName;
        }
        return thirdHospitalAppPath;
    }

    private async void InitializeAsync()
    {
        await ConnectToSignalR();
        await GetPatientAsync();
        await GetPatientRecords();

    }
    private async Task ConnectToSignalR()
    {
        await _signalRService.ConnectAsync();
    }

    private async Task GetPatientRecords()
    {
        try
        {
            var records = await _apiService.GetRecordByPatientId(CurrentPatient.PatientID);
            if (records != null && records.Any())
            {
                var sorted = records
                    .OrderByDescending(r => r.appointment.AppointmentDateTime)
                    .ToList();

                PatientRecords = new ObservableCollection<Records>(sorted);
            }
            else
            {
                PatientRecords = new ObservableCollection<Records>();
            }

            Console.WriteLine($"Loaded {PatientRecords.Count} record(s) for patient {CurrentPatient.Name}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving records: {ex.Message}");
            PatientRecords = new ObservableCollection<Records>();
        }
    }



    private async Task GetPatientAsync()
    {
        var response = await _apiService.GetPatientAsync(Appointment.PatientID);
        CurrentPatient = new Patient
        {
            PatientID = response.PatientID,
            Name = response.Name,
            Bday = response.Bday,
            Sex = response.Sex,
            Address = response.Address,
            BloodType = response.BloodType,
            Email = response.Email,
            ContactNumber = response.ContactNumber,
            ProfilePicture = response.ProfilePicture,
            PatientMedicalInfo = response.PatientMedicalInfo
        };

        Console.WriteLine("Patient Details:");
        Console.WriteLine($"ID: {CurrentPatient.PatientID}");
        Console.WriteLine($"Name: {CurrentPatient.Name}");
        Console.WriteLine($"Birthday: {CurrentPatient.Bday:yyyy-MM-dd}");
        Console.WriteLine($"Age: {CurrentPatient.Age}");
        Console.WriteLine($"Sex: {CurrentPatient.Sex}");
        Console.WriteLine($"Address: {CurrentPatient.Address}");
        Console.WriteLine($"Blood Type: {CurrentPatient.BloodType}");
        Console.WriteLine($"Email: {CurrentPatient.Email}");
        Console.WriteLine($"Contact Number: {CurrentPatient.ContactNumber}");
        Console.WriteLine($"Medical Allergies: {CurrentPatient.PatientMedicalInfo.medicalAllergies}");
        Console.WriteLine($"Latex Allergy: {CurrentPatient.PatientMedicalInfo.latexAllergy}");
        Console.WriteLine($"Food Allergy: {CurrentPatient.PatientMedicalInfo.foodAllergy}");
        Console.WriteLine($"Diet: {CurrentPatient.PatientMedicalInfo.diet}");
        Console.WriteLine($"Exercise: {CurrentPatient.PatientMedicalInfo.exercise}");
        
    }

    [RelayCommand]
    private void BackToDashboard()
    {
        _mainWindowViewModel.NavigateToDoctorsMainMenu();
    }

    private void OnRecordAdded(Records record)
    {
        // Add record to ObservableCollection or show toast/notification
        Console.WriteLine($"New record added for patient {record.patient.Name}");
    }

    private string GenerateDiagnosis()
    {
        Diagnosis diagnosis = new Diagnosis(){
            condition = Condition,
            findings = Findings,
            recommendations = Recommendations,
            symptoms = Symptoms
        };
        return PdfServices.GenerateDiagnosis(Appointment, CurrentPatient, diagnosis);
    }

    [RelayCommand]
    public async void MarkAsDone()
    {
        if (ParentWindow == null)
    {
        Console.WriteLine("ParentWindow not set. Cannot show dialog.");
        return;
    }
        if (string.IsNullOrWhiteSpace(Condition) ||
            string.IsNullOrWhiteSpace(Symptoms) ||
            string.IsNullOrWhiteSpace(Findings) ||
            string.IsNullOrWhiteSpace(Recommendations) ||
            string.IsNullOrWhiteSpace(Prescription) ||
            string.IsNullOrWhiteSpace(Disease))
        {
            await ShowValidationDialogAsync();
            return;
        }

        bool confirmed = await ShowConfirmationDialog(
        "Confirm Completion",
        "Are you sure you want to mark this appointment as done and generate all reports? This cannot be undone"
        );

        if (!confirmed)
            return;

        IsLoading = true;

        try
        {
            string diagnosis = GenerateDiagnosis();
            string mc = GenerateMedicalCertificate();
            string prescription = GeneratePrescription();

            Records record = new Records(){
                patient = CurrentPatient,
                appointment = Appointment,
                medicalCertificatePath = mc,
                prescriptionPath = prescription,
                diagnosisPath = diagnosis
            };

            string jsonRecord = System.Text.Json.JsonSerializer.Serialize(record, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            Console.WriteLine("Sending record to API:");
            Console.WriteLine(jsonRecord);
            Console.WriteLine();
            await _apiService.AddRecordAsync(record);
            await _apiService.UpdateAppointmentStatus(Appointment.PkId);
            await EmailService.SendEmail(mc, prescription, Appointment, CurrentPatient);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            IsLoading = false;
            BackToDashboard();
        }
    }


    private string GeneratePrescription(){
        return PdfServices.GeneratePrescription(Appointment, CurrentPatient, Prescription);
    }

    private string GenerateMedicalCertificate(){
        MedicalCertificate mc = new MedicalCertificate(){
            disease = Disease,
            period = SelectedCertificateDate
        };
        return PdfServices.GenerateMedicalCertificate(Appointment, CurrentPatient, mc);
    }

    private async Task ShowValidationDialogAsync()
    {
        var dialog = new Window
        {
            Width = 400,
            Height = 200,
            Title = "Incomplete Information",
            Content = new TextBlock
            {
                Text = "Please fill out the diagnosis, prescription, and medical certificate before marking the appointment as done.",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Thickness(20)
            },
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow is Window mainWindow)
        {
            await dialog.ShowDialog(mainWindow);
        }
        else
        {
            // As a fallback, you can log or throw an error because dialog can't be shown without a parent
            Debug.WriteLine("MainWindow not available — cannot show validation dialog.");
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
    private void ViewPdf(string pdfPath)
    {
        if (string.IsNullOrEmpty(pdfPath))
        {
            Debug.WriteLine("PDF path is null or empty");
            return;
        }

        try
        {
            // Check if the file exists
            if (!System.IO.File.Exists(pdfPath))
            {
                Debug.WriteLine($"PDF file does not exist: {pdfPath}");
                ShowFileNotFoundDialog(pdfPath);
                return;
            }
            var fullPath = Path.GetFullPath(Path.Combine(basePath, pdfPath));

            // Use the default PDF viewer on the system to open the file
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(fullPath)
                {
                    UseShellExecute = true
                }
            };
            process.Start();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error opening PDF: {ex.Message}");
            ShowErrorDialog($"Error opening PDF: {ex.Message}");
        }
    }

    private async void ShowFileNotFoundDialog(string path)
    {
        var dialog = new Window
        {
            Width = 400,
            Height = 200,
            Title = "File Not Found",
            Content = new TextBlock
            {
                Text = $"The PDF file could not be found at path:\n{path}",
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20)
            },
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (ParentWindow != null)
        {
            await dialog.ShowDialog(ParentWindow);
        }
        else if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow is Window mainWindow)
        {
            await dialog.ShowDialog(mainWindow);
        }
    }

    private async void ShowErrorDialog(string message)
    {
        var dialog = new Window
        {
            Width = 400,
            Height = 200,
            Title = "Error",
            Content = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20)
            },
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (ParentWindow != null)
        {
            await dialog.ShowDialog(ParentWindow);
        }
        else if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow is Window mainWindow)
        {
            await dialog.ShowDialog(mainWindow);
        }
    }
}
