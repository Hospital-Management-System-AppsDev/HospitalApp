using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Email;
using HospitalApp.Models;

namespace HospitalApp.ViewModels;

public partial class AppointmentDetailsViewModel : ViewModelBase
{

    [ObservableProperty]
    private bool isLoading;

    public Appointment Appointment { get; }

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

    public AppointmentDetailsViewModel(ApiService api, SignalRService signalR, MainWindowViewModel mainWindowViewModel, Appointment appointment)
    {
        _mainWindowViewModel = mainWindowViewModel;
        Appointment = appointment;
        _apiService = api;
        _signalRService = signalR;

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await ConnectToSignalR();
        await GetPatientAsync();
    }
    private async Task ConnectToSignalR()
    {
        await _signalRService.ConnectAsync();
    }

        private async Task GetPatientAsync()
    {
        var response = await _apiService.GetPatientAsync(Appointment.PatientID);
        CurrentPatient = new Patient
        {
            PatientID = response.PatientID,
            Name = response.Name,
            Bday = response.Bday,
            Age = response.Age,
            Sex = response.Sex,
            Address = response.Address,
            BloodType = response.BloodType,
            Email = response.Email,
            ContactNumber = response.ContactNumber
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
    }

    [RelayCommand]
    private void BackToDashboard()
    {
        _mainWindowViewModel.NavigateToDoctorsMainMenu();
    }

    private void GenerateDiagnosis()
    {
        Diagnosis diagnosis = new Diagnosis(){
            condition = Condition,
            findings = Findings,
            recommendations = Recommendations,
            symptoms = Symptoms
        };
        PdfServices.GenerateDiagnosis(Appointment, CurrentPatient, diagnosis);
    }

    [RelayCommand]
    public async void MarkAsDone()
    {
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

        IsLoading = true;

        try
        {
            GenerateDiagnosis();
            GenerateMedicalCertificate();
            GeneratePrescription();
            
            await _apiService.UpdateAppointmentStatus(Appointment.PkId);
            await EmailService.SendEmail();
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


    private void GeneratePrescription(){
        PdfServices.GeneratePrescription(Appointment, CurrentPatient, Prescription);
    }

    private void GenerateMedicalCertificate(){
        MedicalCertificate mc = new MedicalCertificate(){
            disease = Disease,
            period = SelectedCertificateDate
        };
        PdfServices.GenerateMedicalCertificate(Appointment, CurrentPatient, mc);
        Console.WriteLine("PDF Generated");
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

   
}
