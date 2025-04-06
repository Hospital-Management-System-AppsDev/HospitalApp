using CommunityToolkit.Mvvm.ComponentModel;
using HospitalApp.Views;
using System;

namespace HospitalApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentView;

        public MainWindowViewModel()
        {
            // Start with the login view
            CurrentView = new LoginViewModel(this);
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
    }
}
