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
        public Doctor Doc{
            get => _doc;
            set => SetProperty(ref _doc, value);
        }

        [ObservableProperty]
        private string selectedAppointmentType;



        public EditAppointmentWindowViewModel(Appointment appointmentToEdit, ObservableCollection<Doctor> doctors)
        {
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
            Doc = appointmentToEdit.AssignedDoctor;
            SelectedAppointmentDate = appointmentToEdit.AppointmentDateTime;
            SelectedAppointmentType = appointmentToEdit.AppointmentType;
            SelectedAppointmentTime = appointmentToEdit.AppointmentTime;

            Doctors = doctors;
        }

        [RelayCommand]
        public async Task OnGetAvailableSlotsCommand()
        {
            AvailableTimeSlots.Clear();

            ErrorMsgCreateVisible = false;

            // Validate inputs
            if (Doc == null)
            {
                ErrorMsgCreate = "Please select a doctor before checking slots.";
                ErrorMsgCreateVisible = true;
                return;
            }

            if (!SelectedAppointmentDate.HasValue)
            {
                ErrorMsgCreate = "Please select a date.";
                ErrorMsgCreateVisible = true;
                return;
            }

            if (string.IsNullOrEmpty(SelectedAppointmentType))
            {
                ErrorMsgCreate = "Please select an appointment type.";
                ErrorMsgCreateVisible = true;
                return;
            }

            try
            {
                var doctorId = Doc.Id;
                var date = SelectedAppointmentDate.Value;
                var type = SelectedAppointmentType.ToLower();

                var availableSlots = await _apiService.GetAvailableTime(doctorId, date, type);

                if(Doc.is_available == 0 && SelectedAppointmentDate.Value.Date == DateTime.Now.Date)
                {
                    ErrorMsgCreate = "Doctor is unavailable on the date selected.";
                    ErrorMsgCreateVisible = true;
                    return;
                }

                if (availableSlots == null || !availableSlots.Any())
                {
                    ErrorMsgCreate = "No available slots found for the selected date and type.";
                    ErrorMsgCreateVisible = true;
                    return;
                }

                // Convert DateTime list to TimeSpan list (only time part for UI)
                if(SelectedAppointmentDate == Today.Date){
                    foreach (var dt in availableSlots)
                    {
                        if(dt.TimeOfDay > Today.TimeOfDay){
                            AvailableTimeSlots.Add(dt.TimeOfDay);
                        }
                    }
                }else{
                    foreach (var dt in availableSlots)
                    {
                        AvailableTimeSlots.Add(dt.TimeOfDay);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMsgCreate = $"Error fetching slots: {ex.Message}";
                ErrorMsgCreateVisible = true;
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

    }
}
