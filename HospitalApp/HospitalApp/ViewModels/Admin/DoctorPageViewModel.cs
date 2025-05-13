using System;
using Avalonia;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HospitalApp.Models;
using Avalonia.Controls;
using HospitalApp.Views.HelperWindows;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HospitalApp.ViewModels
{
    public partial class DoctorPageViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;
        private string _searchText = string.Empty;
        private ObservableCollection<Doctor> _filteredDoctors;
        [ObservableProperty]
        private Doctor selectedDoctor;
        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string errorMessage;
        public ObservableCollection<Doctor> Doctors { get; set; }

        // Commands
        // Removed explicit declarations for these commands, RelayCommand will auto-generate them

        public DoctorPageViewModel(ApiService apiService, SignalRService signalRService)
        {
            _apiService = apiService;
            _signalRService = signalRService;

            _signalRService.DoctorAvailabilityUpdated += OnDoctorAvailabilityUpdated;


            // Initialize collections to empty first
            Doctors = new ObservableCollection<Doctor>();
            _filteredDoctors = new ObservableCollection<Doctor>();
            IsEditing = false;            
            // Initialize data after collections are created
            ConnectToSignalR();
            Initialize();

        }

        private async void ConnectToSignalR()
        {
            await _signalRService.ConnectAsync();
        }

        private void OnDoctorAvailabilityUpdated(int doctorId, int isAvailable)
        {
            var doctor = Doctors.FirstOrDefault(d => d.Id == doctorId);
            if (doctor != null)
            {
                doctor.is_available = isAvailable;
                FilterDoctors();
            }
        }

        public ObservableCollection<Doctor> FilteredDoctors
        {
            get => _filteredDoctors;
            private set
            {
                _filteredDoctors = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    FilterDoctors();
                }
            }
        }

        private async void Initialize()
        {
            await LoadDoctors();
        }

        private async Task LoadDoctors()
        {
            var doctorsList = await _apiService.GetDoctorsAsync();
            Doctors = new ObservableCollection<Doctor>(doctorsList);
            FilterDoctors();
        }

        // Automatically creates the AddDoctorCommand
        [RelayCommand]
        private async void AddDoctor()
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

            var addDoctorViewModel = new AddDoctorViewModel(_apiService, _signalRService);

            var dialog = new Window
            {
                Title = "Add New Doctor",
                Content = new AddDoctorView { DataContext = addDoctorViewModel },
                Width = 400,
                Height = 650,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            addDoctorViewModel.SetWindow(dialog);

            var result = await dialog.ShowDialog<Doctor>(parentWindow);

            if (result != null)
            {
                var newDoctor = await _apiService.AddDoctorAsync(result);
                if (newDoctor != null)
                {
                    Doctors.Add(newDoctor);
                    LoadDoctors();
                }
            }
        }

        // Automatically creates the EditDoctorCommand
        [RelayCommand]
        private void EditDoctor()
        {
            if (SelectedDoctor != null)
            {
                IsEditing = !IsEditing;
            }
        }

        private void FilterDoctors()
        {
            if (Doctors == null)
            {
                FilteredDoctors = new ObservableCollection<Doctor>();
                return;
            }

            if (string.IsNullOrWhiteSpace(_searchText))
            {
                FilteredDoctors = new ObservableCollection<Doctor>(Doctors);
            }
            else
            {
                var filtered = Doctors
                    .Where(d => d.Name.ToLower().Contains(_searchText.ToLower()) ||
                                d.specialization.ToLower().Contains(_searchText.ToLower()))
                    .ToList();

                FilteredDoctors = new ObservableCollection<Doctor>(filtered);
            }
        }

        [RelayCommand]
        private async Task DeleteDoctor()
        {
            if (SelectedDoctor != null)
            {
                // Show confirmation popup
                bool confirm = await PopupWindow.ShowConfirmation(
                    owner: App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null,
                    title: "Confirm Deletion",
                    message: $"Are you sure you want to delete Dr. {SelectedDoctor.Name}?",
                    confirmButtonText: "Delete",
                    cancelButtonText: "Cancel"
                );

                if (confirm)
                {
                    var result = await _apiService.DeleteDoctorAsync(SelectedDoctor.Id);
                    if (result)
                    {
                        bool conf = await PopupWindow.ShowConfirmation(
                            owner: App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desk ? desk.MainWindow : null,
                            title: "Success",
                            message: $"Successfully Deleted: {SelectedDoctor.Name}",
                            confirmButtonText: "Okay",
                            cancelButtonText: ""
                        );
                        Doctors.Remove(SelectedDoctor);
                        LoadDoctors();
                    }
                }

                IsEditing = false;
            }
        }


        [RelayCommand]
        private async Task SaveDoctor()
        {
            if (SelectedDoctor != null)
            {
                var updatedDoctor = await _apiService.UpdateDoctorAsync(SelectedDoctor);
                if (updatedDoctor != null)
                {
                    SelectedDoctor = updatedDoctor;
                    LoadDoctors();
                }
            }
            IsEditing = false;
        }
        
    }
}
