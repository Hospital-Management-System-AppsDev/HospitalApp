using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HospitalApp.Models;
using HospitalApp.Services;

namespace HospitalApp.ViewModels
{
    public partial class DashboardPageViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;

        [ObservableProperty]
        private ObservableCollection<Doctor> doctors = new();

        [ObservableProperty]
        private ObservableCollection<Doctor> filteredDoctors = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        private readonly UserSessionService _session = UserSessionService.Instance;


        [ObservableProperty]
        private User _currentUser;

        // [Observable Property]

        // private Admin admin = 

        public DashboardPageViewModel(ApiService apiService, SignalRService signalRService)
        {
            _apiService = apiService;
            _signalRService = signalRService;

            _signalRService.DoctorAvailabilityUpdated += OnDoctorAvailabilityUpdated;
            _signalRService.DoctorAdded += OnDoctorAdded;
            
            _currentUser = _session.CurrentUser;

            LoadDoctors();
            ConnectToSignalR();
            Console.WriteLine($"CurrentUser: {_currentUser?.Name}");

        }

        private async void ConnectToSignalR()
        {
            await _signalRService.ConnectAsync();
        }

        private async void LoadDoctors()
        {
            var doctorsList = await _apiService.GetDoctorsAsync();
            Doctors = new ObservableCollection<Doctor>(doctorsList);
            FilterDoctors();
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

        partial void OnSearchTextChanged(string value)
        {
            FilterDoctors();
        }

        private void FilterDoctors()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredDoctors = new ObservableCollection<Doctor>(
                    Doctors.OrderByDescending(d => d.is_available)
                );
            }
            else
            {
                var filteredList = Doctors
                    .Where(d => d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(d => d.is_available)
                    .ToList();

                FilteredDoctors = new ObservableCollection<Doctor>(filteredList);
            }
        }
        private void OnDoctorAdded(Doctor doctor)
        {
            if (!Doctors.Any(d => d.Id == doctor.Id)) // Prevent duplicate entries
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    Doctors.Add(doctor);
                    FilterDoctors(); // Refresh filtered list
                });
            }
        }


    }
}
