using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HospitalApp.ViewModels
{
    public partial class DashboardPageViewModel : ViewModelBase
    {
        [ObservableProperty]
        private DateTime _currentDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _calendarStartDate = DateTime.Today.AddMonths(-1);

        [ObservableProperty]
        private DateTime _calendarEndDate = DateTime.Today.AddMonths(6);

        public DashboardPageViewModel()
        {
            // Initialize any additional properties or commands here
        }
    }
}