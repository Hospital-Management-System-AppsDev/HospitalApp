using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;
using System.Threading.Tasks;
using HospitalApp.Services;
using System;
using HospitalApp.Views;
using System.IO;

namespace HospitalApp.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel;
        private readonly ApiService _apiService = new ApiService();
        private readonly SignalRService _signalRService = new SignalRService();
        private readonly UserSessionService _session = UserSessionService.Instance;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private bool _showPassword;

        [ObservableProperty]
        private string errorMsg;

        [ObservableProperty]
        private bool errorMsgVisible = false;

        public char ShowPasswordChar => ShowPassword ? '\0' : '●';

        public LoginViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            ConnectToSignalR();
        }

        partial void OnShowPasswordChanged(bool value)
        {
            OnPropertyChanged(nameof(ShowPasswordChar));
        }

        private async void ConnectToSignalR()
        {
            await _signalRService.ConnectAsync();
        }

        [RelayCommand]
        private async Task Login()
        {
            try{
                var user = await _apiService.LoginAsync(_username);
                if (user == null)
                {
                    ErrorMsg = "Invalid Username or Password";
                    ErrorMsgVisible = true;
                    return;
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(_password, user.Password);
                if (!isPasswordValid)
                {
                    ErrorMsg = "Invalid Username or Password";
                    ErrorMsgVisible = true;
                    return;
                }

                _session.SetUser(user); // ✅ Store globally
                Console.WriteLine($"Logged in as: {_session.CurrentUser.Username} ({_session.CurrentUser.Role})");

                if (user.Role == "admin")
                {
                    _mainViewModel.NavigateToAdminMainMenu();
                }
                else if (user.Role == "doctor")
                {
                    await _apiService.UpdateDoctorAvailabilityAsync(user.Id, 1);
                    _mainViewModel.NavigateToDoctorsMainMenu();
                }
                else
                {
                    ErrorMsg = "Unauthorized Access.";
                    ErrorMsgVisible = true;
                    return;
                }
                var basePath = AppContext.BaseDirectory;
                Console.WriteLine("Original Base Path: " + basePath);

                // Traverse up the directory structure to get the third "HospitalApp"
                var thirdHospitalAppPath = basePath;

                // Go up 3 levels (you can adjust the number of levels if needed)
                for (int i = 0; i < 4; i++)
                {
                    thirdHospitalAppPath = Directory.GetParent(thirdHospitalAppPath).FullName;
                }                
            }catch(Exception ex){
                ErrorMsg = "Invalid Username or Password";
                ErrorMsgVisible = true;
            }
        }
    }
}