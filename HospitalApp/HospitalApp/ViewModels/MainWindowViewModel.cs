using CommunityToolkit.Mvvm.ComponentModel;
using HospitalApp.Views;
using System;
using HospitalApp.Services;
using HospitalApp.Models;

namespace HospitalApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentView;

        public UserSessionService SessionService { get; }

        private ApiService _apiService = new ApiService();
        private SignalRService _signalRService = new SignalRService();


        public MainWindowViewModel()
        {
            // Start with the login view
            CurrentView = new LoginViewModel(this);
            SessionService = UserSessionService.Instance;

        }

        public void NavigateToAdminMainMenu()
        {
            Console.WriteLine("Navigating to Main Menu");
            CurrentView = new MainMenuViewModel(this);
            Console.WriteLine($"CurrentView is now: {CurrentView.GetType().Name}");
        }

        public void NavigateToDoctorsMainMenu()
        {
            Console.WriteLine("Navigating to Doctors Main Menu");
            CurrentView = new DoctorMainMenuPageViewModel(this);
            Console.WriteLine($"CurrentView is now: {CurrentView.GetType().Name}");
        }


        public void NavigateToLogin()
        {
            CurrentView = new LoginViewModel(this);
        }

        public void NavigateToAppointmentDetails(Appointment appointment)
        {
            Console.WriteLine($"Navigating to Appointment Details for {appointment.PatientName}");
            CurrentView = new AppointmentDetailsViewModel(_apiService, _signalRService, this, appointment);
            Console.WriteLine($"CurrentView is now: {CurrentView.GetType().Name}");
        }


    }
}
