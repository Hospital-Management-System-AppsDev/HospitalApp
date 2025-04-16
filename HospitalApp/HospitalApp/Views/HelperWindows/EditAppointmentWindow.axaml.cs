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
                var success = await _apiService.UpdateAppointment(editedAppointment.pkId, editedAppointment);
                if (success)
                {
                    // Don't try to update collections manually here
                    // The SignalR event will handle the update
                    Close(editedAppointment);
                }
            };


            this.FindControl<Button>("CancelButton").Click += (_, _) =>
            {
                Close(null);
            };
        }
    }