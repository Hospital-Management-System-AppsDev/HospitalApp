using System;
using Avalonia;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using HospitalApp.Models;
using HospitalApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Avalonia.Controls;
using HospitalApp.Views.HelperWindows;
using Avalonia.Controls.ApplicationLifetimes;
namespace HospitalApp.ViewModels;

public partial class PatientPageViewModel : ViewModelBase
{
    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;

    public ObservableCollection<Patient> Patients { get; } = new();
    
    [ObservableProperty]
    private ObservableCollection<Patient> filteredPatients = new();

    [ObservableProperty]
    private Patient selectedPatient;

    [ObservableProperty]
    private bool showPatient;

    [ObservableProperty]
    private bool isEditing;
    
    [ObservableProperty]
    private string searchText = string.Empty;

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
        FilterPatients();
    }
    
    public void FilterPatients()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredPatients = new ObservableCollection<Patient>(Patients);
        }
        else
        {
            var filtered = Patients
                .Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
                
            FilteredPatients = new ObservableCollection<Patient>(filtered);
        }
    }
    
    partial void OnSearchTextChanged(string value)
    {
        FilterPatients();
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

    [RelayCommand]
    private async void AddPatient()
    {
        Window parentWindow = null;
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            parentWindow = desktop.MainWindow;
        }

        if (parentWindow == null)
        {
            Console.WriteLine("Error: Parent window not available");
            return;
        }

        var addPatientViewModel = new AddPatientViewModel();

        var dialog = new Window
        {
            Title = "Add New Patient",
            Content = new AddPatientView { DataContext = addPatientViewModel },
            Width = 800,
            Height = 700,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        addPatientViewModel.SetWindow(dialog);

        var result = await dialog.ShowDialog<PatientWithHealth>(parentWindow);

        if (result != null)
        {
            Patients.Add(result.Patient);
            FilterPatients();
        }
    }
}