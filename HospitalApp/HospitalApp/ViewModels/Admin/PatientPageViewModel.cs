using System;
using Avalonia;
using System.Collections.ObjectModel;
using HospitalApp.Models;
using HospitalApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using HospitalApp.Views;

namespace HospitalApp.ViewModels;
public partial class PatientPageViewModel : ViewModelBase
{
    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;

    public ObservableCollection<Patient> Patients { get; } = new();

    public ObservableCollection<string> DietOptions { get; set; } = new() {"Vegetarian", "Vegan", "Omnivore", "Pescatarian", "Keto", "Paleo"};

    public ObservableCollection<string> ExerciseOptions { get; set; } = new() {"Daily", "Occasionally", "Rarely", "Never"};

    public ObservableCollection<string> SleepOptions { get; set; } = new() {"7 hours", "less than 6 hours", "6-7 hours", "8+ hours"};

    public ObservableCollection<string> SmokingOptions { get; set; } = new() {"Yes", "No", "Former smoker"};
    public ObservableCollection<string> AlcoholOptions { get; set; } = new() {"Occasional", "Daily", "Weekly", "Rarely", "Never"};
    

    [ObservableProperty]
    private Patient selectedPatient;

    [ObservableProperty]
    private bool showPatient;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private PatientMedicalInfo patientMedicalInfo;

    [ObservableProperty]
    private string errorMessage = "";

    

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
        ErrorMessage = "";
    }


    [RelayCommand]
    private void EditButton()
    {
        IsEditing = !IsEditing;
    }

    [RelayCommand]
    private async Task SaveChanges()
    {   
        try{
            bool conn = await PopupWindow.ShowConfirmation(
                    owner: App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desk ? desk.MainWindow : null,
                    title: "Confirm Deletion",
                    message: $"Are you sure you want to save these changes?",
                    confirmButtonText: "Yes",
                    cancelButtonText: "No"
                );
            
            if(conn){
                string status = await _apiService.UpdatePatient(selectedPatient);
                if(status == "Success"){
                    bool con = await PopupWindow.ShowConfirmation(
                    owner: App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null,
                    title: "Confirm Deletion",
                    message: $"Saved changes for {selectedPatient.Name}.",
                    confirmButtonText: "Okay",
                    cancelButtonText: ""
                );
                    await LoadDataAsync();
                    IsEditing = false;
                    ErrorMessage = "";
                }
                else{
                    ErrorMessage = status;
                }
            }else{
                IsEditing = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    [RelayCommand]
    private async Task DeletePatient()
    {
        bool confirm = await PopupWindow.ShowConfirmation(
                    owner: App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null,
                    title: "Confirm Deletion",
                    message: $"Are you sure you want to save these changes?",
                    confirmButtonText: "Yes",
                    cancelButtonText: "No"
                );
            
        if(confirm){
            if(SelectedPatient != null){
                bool res = await _apiService.DeletePatientAsync(SelectedPatient.PatientID);

                if(res){
                    bool conf = await PopupWindow.ShowConfirmation(
                    owner: App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desk ? desk.MainWindow : null,
                    title: "Confirm Deletion",
                    message: $"Deleted Successfully",
                    confirmButtonText: "Yes",
                    cancelButtonText: ""
                );
                await LoadDataAsync();
                }

            }
            else{
                ErrorMessage = "Patient not found";
            }
        }
        IsEditing = false;
    }

    [RelayCommand]
    private async Task AddPatient()
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
                Width = 650,
                Height = 650,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            addPatientViewModel.SetWindow(dialog);

            var result = await dialog.ShowDialog<Patient>(parentWindow);

            if (result != null)
            {
                var newPatient = await _apiService.AddPatientAsync(result);
                if (newPatient != null)
                {
                    Patients.Add(newPatient);
                    await LoadDataAsync();
                }
            }
    }
}
