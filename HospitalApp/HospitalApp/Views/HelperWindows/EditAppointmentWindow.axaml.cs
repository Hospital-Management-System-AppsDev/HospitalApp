using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HospitalApp.ViewModels;
using HospitalApp.Models;
using System.Threading.Tasks;
using System;
using System.Text.Json;

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

            this.FindControl<Button>("SaveButton").Click += (_, _) =>
            {
                try
                {
                    Console.WriteLine("[UI] Save button clicked");
                    Console.WriteLine($"[UI] Before ApplySelectedChanges: {JsonSerializer.Serialize(ViewModel.Appointment)}");
                    
                    ViewModel.ApplySelectedChanges(); // Apply UI changes to model
                    
                    Console.WriteLine($"[UI] After ApplySelectedChanges: {JsonSerializer.Serialize(ViewModel.Appointment)}");
                    Console.WriteLine($"[UI] Closing window and returning appointment with ID: {ViewModel.Appointment.pkId}");
                    
                    Close(ViewModel.Appointment);     // Return updated appointment to caller
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UI] Error in Save button click handler: {ex.Message}");
                    Console.WriteLine($"[UI] Stack trace: {ex.StackTrace}");
                }
            };



            this.FindControl<Button>("CancelButton").Click += (_, _) =>
            {
                Close(null);
            };
        }
    }