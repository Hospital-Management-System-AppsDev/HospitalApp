using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;

namespace HospitalApp.ViewModels
{
    public partial class DashboardPageViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly HubConnection _hubConnection;

        [ObservableProperty]
        private ObservableCollection<Doctor> doctors = new();

        [ObservableProperty]
        private ObservableCollection<Doctor> filteredDoctors = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        public DashboardPageViewModel()
        {
            _apiService = new ApiService();
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5271/doctorHub") // Update with your API URL
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<int, int>("UpdateDoctorAvailability", (doctorId, isAvailable) =>
            {
                var doctor = Doctors.FirstOrDefault(d => d.Id == doctorId);
                if (doctor != null)
                {
                    doctor.is_available = isAvailable;
                    FilterDoctors(); // Refresh the UI
                }
            });

            ConnectToSignalR();
            LoadDoctors();
        }

        private async void ConnectToSignalR()
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SignalR connection error: " + ex.Message);
            }
        }

        private async void LoadDoctors()
        {
            var doctorsList = await _apiService.GetDoctorsAsync();
            Doctors = new ObservableCollection<Doctor>(doctorsList);
            FilterDoctors(); // Initialize filtered list
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
                    Doctors.OrderByDescending(d => d.is_available) // Sorting online doctors first
                );
            }
            else
            {
                var filteredList = Doctors
                    .Where(d => d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(d => d.is_available) // Sorting online doctors first
                    .ToList();

                FilteredDoctors = new ObservableCollection<Doctor>(filteredList);
            }
        }
    }
}
