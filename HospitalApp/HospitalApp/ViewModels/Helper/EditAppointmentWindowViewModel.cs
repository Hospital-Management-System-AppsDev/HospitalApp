using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;

namespace HospitalApp.ViewModels
{
    public partial class EditAppointmentWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private Appointment appointment;

        public DateTime Today => DateTime.Now;

        private ApiService _apiService = new ApiService();

        public ObservableCollection<Doctor> Doctors { get; set; }
        public ObservableCollection<string> AppointmentTypes { get; set; } = new() { "Check up", "Consultation", "Surgery" };

        [ObservableProperty]
        private DateTime? selectedAppointmentDate;

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

        public EditAppointmentWindowViewModel(Appointment appointmentToEdit, ObservableCollection<Doctor> doctors)
        {
            Doctors = doctors;

            Appointment = new Appointment
            {
                pkId = appointmentToEdit.pkId,
                PatientID = appointmentToEdit.PatientID,
                PatientName = appointmentToEdit.PatientName,
                AssignedDoctor = appointmentToEdit.AssignedDoctor,
                AppointmentType = appointmentToEdit.AppointmentType,
                Status = appointmentToEdit.Status,
                AppointmentDateTime = appointmentToEdit.AppointmentDateTime
            };

            // Find the matching doctor in the Doctors collection
            Doc = Doctors.FirstOrDefault(d => d.Id == appointmentToEdit.AssignedDoctor.Id);
            SelectedAppointmentType = appointmentToEdit.AppointmentType;
            SelectedAppointmentDate = appointmentToEdit.AppointmentDateTime;

            // Add the current time slot to available slots
            if (SelectedAppointmentTime != default)
            {
                AvailableTimeSlots.Add(SelectedAppointmentTime);
            }

            // Load initial time slots
            if (SelectedAppointmentDate.HasValue && Doc != null && !string.IsNullOrEmpty(SelectedAppointmentType))
            {
                _ = LoadInitialTimeSlots();
            }

            // Add property change handlers
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
                var doctorId = Doc.Id;
                var date = SelectedAppointmentDate.Value;
                var type = SelectedAppointmentType.ToLower();

                var availableSlots = await _apiService.GetAvailableTime(doctorId, date, type);

                if (availableSlots != null && availableSlots.Any())
                {
                    AvailableTimeSlots.Clear();
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

                    // Ensure current time slot is in the list
                    if (SelectedAppointmentTime != default && !AvailableTimeSlots.Contains(SelectedAppointmentTime))
                    {
                        AvailableTimeSlots.Add(SelectedAppointmentTime);
                    }
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
            AvailableTimeSlots.Clear();

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
                    // Only show slots after current time for today's date
                    foreach (var dt in availableSlots)
                    {
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

                    // Ensure current time slot is in the list
                    if (SelectedAppointmentTime != default && !AvailableTimeSlots.Contains(SelectedAppointmentTime))
                    {
                        AvailableTimeSlots.Add(SelectedAppointmentTime);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching slots: {ex.Message}");
            }
        }

        public void ApplySelectedChanges()
        {
            if (SelectedAppointmentDate.HasValue)
            {
                Appointment.AppointmentDateTime = SelectedAppointmentDate.Value.Date + SelectedAppointmentTime;
            }

            Appointment.AppointmentType = SelectedAppointmentType;
            Appointment.AssignedDoctor = Doc;
        }

        partial void OnSelectedAppointmentDateChanged(DateTime? value)
        {
            if (value == null)
            {
                AvailableTimeSlots.Clear();
                SelectedAppointmentTime = default;
            }
        }
    }
}
