using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HospitalApp.ViewModels;
using HospitalApp.Models;

namespace HospitalApp.Views;

public partial class EditAppointmentWindow : Window
    {
        private readonly ApiService _apiService;
        public EditAppointmentWindowViewModel ViewModel { get; }

        public EditAppointmentWindow(EditAppointmentWindowViewModel viewModel, ApiService api)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;

            _apiService = api;
            var editedAppointment = ViewModel.Appointment;

            this.FindControl<Button>("SaveButton").Click += async (_, _) =>
            {
                ViewModel.ApplySelectedChanges(); // <--- Apply UI changes to model

                var success = await _apiService.UpdateAppointment(ViewModel.Appointment.pkId, ViewModel.Appointment);
                if (success)
                {
                    Close(ViewModel.Appointment);
                }
            };


            this.FindControl<Button>("CancelButton").Click += (_, _) =>
            {
                Close(null);
            };
        }
    }