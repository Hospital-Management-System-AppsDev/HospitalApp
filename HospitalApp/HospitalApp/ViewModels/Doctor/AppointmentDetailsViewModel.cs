using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;

namespace HospitalApp.ViewModels;

public partial class AppointmentDetailsViewModel : ViewModelBase
{
    public Appointment Appointment { get; }

    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;

    [ObservableProperty]
    private string additionalNotes;

    [ObservableProperty]
    private string diagnosis;

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

    [RelayCommand]
    private void SubmitDiagnosis()
    {
        // Example: Save to database or trigger further actions
        Console.WriteLine($"Diagnosis: {Diagnosis}");
        Console.WriteLine($"Notes: {AdditionalNotes}");

        string[] arr = Diagnosis.Split("\n");

        foreach(string token in arr){
            Console.WriteLine($"token: {token}");
        }

        // Optional: Display a message or call a service
    }

   
}
