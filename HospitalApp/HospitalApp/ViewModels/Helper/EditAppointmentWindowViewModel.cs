using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;
using Avalonia.Controls;
using System.Text.Json;

namespace HospitalApp.ViewModels
{
    public partial class EditAppointmentWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private Appointment appointment;

        public Window Window { get; set; }

        public DateTime Today => DateTime.Now;

        private ApiService _apiService = new ApiService();

        public ObservableCollection<Doctor> Doctors { get; set; }
        public ObservableCollection<string> AppointmentTypes { get; set; } = new() { "Check up", "Consultation", "Surgery" };

        [ObservableProperty]
        private DateTime? selectedAppointmentDate;

        // Will be initialized in constructor with appointment's time
        [ObservableProperty]
        private TimeSpan selectedAppointmentTime;

        public ObservableCollection<TimeSpan> AvailableTimeSlots { get; set; } = new();

        [ObservableProperty]
        public bool errorMsgCreateVisible = false;

        [ObservableProperty]
        public string errorMsgCreate;

        private Doctor _doc;
        public Doctor Doc
        {
            get => _doc;
            set => SetProperty(ref _doc, value);
        }

        [ObservableProperty]
        private string selectedAppointmentType;

        public EditAppointmentWindowViewModel(Appointment appointmentToEdit, ObservableCollection<Doctor> doctors, Window window)
        {
            Window = window;
            Doctors = doctors;

            Appointment = new Appointment
            {
                pkId = appointmentToEdit.pkId,
                PatientID = appointmentToEdit.PatientID,
                PatientName = appointmentToEdit.PatientName,
                AssignedDoctor = appointmentToEdit.AssignedDoctor,
                AppointmentType = appointmentToEdit.AppointmentType,
                Status = appointmentToEdit.Status,
                AppointmentDateTime = appointmentToEdit.AppointmentDateTime,
                temperature = appointmentToEdit.temperature,
                pulseRate = appointmentToEdit.pulseRate,
                weight = appointmentToEdit.weight,
                height = appointmentToEdit.height,
                sugarLevel = appointmentToEdit.sugarLevel,
                bloodPressure = appointmentToEdit.bloodPressure,
                chiefComplaint = appointmentToEdit.chiefComplaint,
                patientMedicalInfo = appointmentToEdit.patientMedicalInfo
            };

            // Find the matching doctor in the Doctors collection
            Doc = Doctors.FirstOrDefault(d => d.Id == appointmentToEdit.AssignedDoctor.Id);
            SelectedAppointmentType = appointmentToEdit.AppointmentType;
            SelectedAppointmentDate = appointmentToEdit.AppointmentDateTime;

            // Initialize TimeSpan from the appointment's DateTime value
            try
            {
                // Extract the time portion from the appointment's DateTime
                SelectedAppointmentTime = appointmentToEdit.AppointmentDateTime.TimeOfDay;
                Console.WriteLine($"Set SelectedAppointmentTime to: {SelectedAppointmentTime}");
                
                // If we get a default TimeSpan (00:00:00), use 8:00 AM as fallback
                if (SelectedAppointmentTime == default)
                {
                    SelectedAppointmentTime = TimeSpan.FromHours(8);
                    Console.WriteLine($"Using default time 8:00 AM since appointment time was default: {SelectedAppointmentTime}");
                }
            }
            catch (Exception ex)
            {
                // If there's any error, set a default time of 8:00 AM
                SelectedAppointmentTime = TimeSpan.FromHours(8);
                Console.WriteLine($"Error setting appointment time: {ex.Message}. Using default 8:00 AM");
            }

            // Add the current time slot to available slots first
            if (SelectedAppointmentTime != default)
            {
                // Ensure we're using the exact TimeSpan from the appointment
                AvailableTimeSlots.Add(SelectedAppointmentTime);
                Console.WriteLine($"Added original appointment time to slots: {SelectedAppointmentTime}");
            }

            // Load initial time slots - this will happen asynchronously
            if (SelectedAppointmentDate.HasValue && Doc != null && !string.IsNullOrEmpty(SelectedAppointmentType))
            {
                _ = LoadInitialTimeSlots();
            }

            // Property change handlers
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(SelectedAppointmentDate) && SelectedAppointmentDate != null)
                {
                    _ = OnGetAvailableSlotsCommand();
                }
            };

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(SelectedAppointmentType) && SelectedAppointmentType != null && SelectedAppointmentDate != null && Doc != null)
                {
                    _ = OnGetAvailableSlotsCommand();
                }
            };

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Doc) && Doc != null && SelectedAppointmentType != null && SelectedAppointmentDate != null)
                {
                    _ = OnGetAvailableSlotsCommand();
                }
            };
        }

        private async Task LoadInitialTimeSlots()
        {
            try
            {
                // Save the original time so we can maintain it
                var originalTime = SelectedAppointmentTime;
                Console.WriteLine($"[LoadInitialTimeSlots] Original time: {originalTime}");

                var doctorId = Doc.Id;
                var date = SelectedAppointmentDate.Value;
                var type = SelectedAppointmentType.ToLower();

                var availableSlots = await _apiService.GetAvailableTime(doctorId, date, type);

                if (availableSlots != null && availableSlots.Any())
                {
                    AvailableTimeSlots.Clear();
                    
                    // Always ensure the original appointment time is in the list first
                    AvailableTimeSlots.Add(originalTime);
                    Console.WriteLine($"[LoadInitialTimeSlots] Added original time to slots: {originalTime}");

                    foreach (var dt in availableSlots)
                    {
                        // Only add slots that are after current time
                        if (SelectedAppointmentDate.Value.Date == Today.Date)
                        {
                            if (dt.TimeOfDay > Today.TimeOfDay)
                            {
                                AvailableTimeSlots.Add(dt.TimeOfDay);
                            }
                        }
                        else
                        {
                            AvailableTimeSlots.Add(dt.TimeOfDay);
                        }
                    }

                    // Sort the time slots
                    var sortedSlots = AvailableTimeSlots.OrderBy(t => t).ToList();
                    AvailableTimeSlots.Clear();
                    foreach (var slot in sortedSlots)
                    {
                        AvailableTimeSlots.Add(slot);
                    }

                    // Make sure the selected time is still the original appointment time
                    SelectedAppointmentTime = originalTime;
                    Console.WriteLine($"[LoadInitialTimeSlots] Reset selected time to: {SelectedAppointmentTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading initial time slots: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task OnGetAvailableSlotsCommand()
        {
            // Store the current selected time before clearing
            var currentSelectedTime = SelectedAppointmentTime;
            Console.WriteLine($"[GetSlots] Current selected time before fetching: {currentSelectedTime}");

            AvailableTimeSlots.Clear();

            // Always add the original appointment time first
            AvailableTimeSlots.Add(currentSelectedTime);
            Console.WriteLine($"[GetSlots] Added original time to slots: {currentSelectedTime}");

            if (Doc == null || !SelectedAppointmentDate.HasValue || string.IsNullOrEmpty(SelectedAppointmentType))
            {
                return;
            }

            try
            {
                var doctorId = Doc.Id;
                var date = SelectedAppointmentDate.Value;
                var type = SelectedAppointmentType.ToLower();

                var availableSlots = await _apiService.GetAvailableTime(doctorId, date, type);

                if (Doc.is_available == 0 && SelectedAppointmentDate.Value.Date == Today.Date)
                {
                    return;
                }

                if (availableSlots != null && availableSlots.Any())
                {
                    // Add available slots, filtering by today if needed
                    foreach (var dt in availableSlots)
                    {
                        if (SelectedAppointmentDate.Value.Date == Today.Date && dt.TimeOfDay <= Today.TimeOfDay)
                        {
                            continue; // Skip past time slots for today
                        }
                        
                        // Avoid duplicates - only add if not already added
                        if (!AvailableTimeSlots.Any(t => Math.Abs((t - dt.TimeOfDay).TotalMinutes) < 1))
                        {
                            AvailableTimeSlots.Add(dt.TimeOfDay);
                        }
                    }

                    // Sort the time slots
                    var sortedSlots = AvailableTimeSlots.OrderBy(t => t).ToList();
                    AvailableTimeSlots.Clear();
                    foreach (var slot in sortedSlots)
                    {
                        AvailableTimeSlots.Add(slot);
                    }

                    // Reset the selected time to maintain it through the refresh
                    SelectedAppointmentTime = currentSelectedTime;
                    Console.WriteLine($"[GetSlots] Reset selected time to: {SelectedAppointmentTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching slots: {ex.Message}");
            }
        }

        public void ApplySelectedChanges()
        {
            try
            {
                Console.WriteLine("[EditVM] Starting ApplySelectedChanges");
                Console.WriteLine($"[EditVM] Before changes: {JsonSerializer.Serialize(Appointment)}");
                
                if (SelectedAppointmentDate.HasValue)
                {
                    var originalDateTime = Appointment.AppointmentDateTime;
                    Appointment.AppointmentDateTime = SelectedAppointmentDate.Value.Date + SelectedAppointmentTime;
                    Console.WriteLine($"[EditVM] Changed AppointmentDateTime from {originalDateTime} to {Appointment.AppointmentDateTime}");
                }

                var originalType = Appointment.AppointmentType;
                Appointment.AppointmentType = SelectedAppointmentType;
                Console.WriteLine($"[EditVM] Changed AppointmentType from {originalType} to {Appointment.AppointmentType}");
                
                var originalDoctor = Appointment.AssignedDoctor?.Name ?? "null";
                Appointment.AssignedDoctor = Doc;
                Console.WriteLine($"[EditVM] Changed AssignedDoctor from {originalDoctor} to {Appointment.AssignedDoctor?.Name ?? "null"}");

                Console.WriteLine($"[EditVM] After changes: {JsonSerializer.Serialize(Appointment)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditVM] Exception in ApplySelectedChanges: {ex.Message}");
                Console.WriteLine($"[EditVM] Stack trace: {ex.StackTrace}");
            }
        }

        partial void OnSelectedAppointmentDateChanged(DateTime? value)
        {
            if (value == null)
            {
                AvailableTimeSlots.Clear();
                // Keep the existing SelectedAppointmentTime to avoid binding issues
            }
            else
            {
                // Trigger slot refresh when date changes
                _ = OnGetAvailableSlotsCommand();
            }
        }
    }
}