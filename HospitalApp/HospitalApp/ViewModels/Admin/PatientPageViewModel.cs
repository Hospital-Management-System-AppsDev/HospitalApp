using System;
using Avalonia;
using System.Collections.ObjectModel;
using HospitalApp.Models;
using HospitalApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace HospitalApp.ViewModels;

public partial class PatientPageViewModel : ViewModelBase
{
    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;

    public ObservableCollection<Patient> Patients { get; } = new();

    [ObservableProperty]
    private Patient selectedPatient;

    [ObservableProperty]
    private bool showPatient;

    [ObservableProperty]
    private bool isEditing;

    public PatientPageViewModel(ApiService apiService, SignalRService signalRService)
    {
        _apiService = apiService;
        _signalRService = signalRService;
        InitializeAsync();
    }

    public async void InitializeAsync()
    {
        await LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        var patients = await _apiService.GetPatientsAsync();
        Patients.Clear();
        foreach (var patient in patients)
        {
            Patients.Add(patient);
        }
    }

    partial void OnSelectedPatientChanged(Patient? value)
    {
        ShowPatient = value != null;
        IsEditing = false;
    }

    [RelayCommand]
    private void EditButton()
    {
        IsEditing = !IsEditing;
    }

    [RelayCommand]
    private void SaveChanges()
    {
        IsEditing = false;
    }

    [RelayCommand]
    private void DeletePatient()
    {
        IsEditing = false;
    }
}
