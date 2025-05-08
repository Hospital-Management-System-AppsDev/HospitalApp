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
using HospitalApp.Services;
namespace HospitalApp.ViewModels
{
    public partial class DoctorAppointmentPageViewModel : ViewModelBase
    {

        private readonly UserSessionService _session = UserSessionService.Instance;

        public Window ParentWindow { get; set; }

        [ObservableProperty]
        private User _currentUser;


        [ObservableProperty]
        private string appointmentSearchText;

        [ObservableProperty]
        private string missedAppointmentSearchText;

        public ObservableCollection<Appointment> FilteredMissedAppointments { get; set; } = new();


        public ObservableCollection<Appointment> FilteredAppointments { get; set; } = new();

        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;

        public ObservableCollection<Appointment> UpcomingAppointments { get; set; } = new();

        public ObservableCollection<Appointment> MissedAppointments { get; set; } = new();

        public ObservableCollection<Appointment> DoneAppointments { get; set; } = new();

        public DoctorAppointmentPageViewModel(ApiService api, SignalRService signalR)
        {
            _apiService = api;
            _signalRService = signalR;
            _currentUser = _session.CurrentUser;
            LoadDataAsync();

            ConnectToSignalR();
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
                
                // Load appointments
                var appointments = await _apiService.GetAppointmentsByDoctorAsync(CurrentUser.Id);

                var upcoming = appointments
                    .Where(a => a.Status != 1 && a.AppointmentDateTime >= DateTime.Now)
                    .OrderBy(a => a.AppointmentDateTime)
                    .ToList();

                var missed = appointments
                    .Where(a => a.Status == 0 && a.AppointmentDateTime < DateTime.Now)
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
                    
                    MissedAppointments.Clear();
                    foreach (var apt in missed)
                    {
                        MissedAppointments.Add(apt);
                    }

                    FilteredMissedAppointments.Clear();
                    foreach (var apt in missed)
                        FilteredMissedAppointments.Add(apt);
                    

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

        public void FilterMissedAppointments()
        {
            if (string.IsNullOrWhiteSpace(MissedAppointmentSearchText))
            {
                // No search input, show all upcoming appointments
                FilteredMissedAppointments.Clear();
                foreach (var apt in MissedAppointments)
                    FilteredMissedAppointments.Add(apt);
                return;
            }

            var searchText = MissedAppointmentSearchText.Trim().ToLower();

            var filtered = MissedAppointments.Where(apt =>
                apt.PatientName?.ToLower().Contains(searchText) == true ||
                apt.AppointmentType?.ToLower().Contains(searchText) == true ||
                apt.AssignedDoctor?.Name?.ToLower().Contains(searchText) == true
            );

            FilteredMissedAppointments.Clear();
            foreach (var apt in filtered)
                FilteredMissedAppointments.Add(apt);
        }
            
        

        partial void OnAppointmentSearchTextChanged(string value)
        {
            FilterAppointments();
        }

        partial void OnMissedAppointmentSearchTextChanged(string value)
        {
            FilterMissedAppointments();
        }

    }
}


