using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HospitalApp.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        public LoginViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        [RelayCommand]
        private void Login()
        {
            if (Username == "admin" && Password == "password") // Replace with real authentication logic
            {
                _mainViewModel.NavigateToAdminMainMenu();
            }else if(Username == "doc" && Password == "password"){
                _mainViewModel.NavigateToDoctorsMainMenu();
            }
        }
    }
}
