using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HospitalApp.Models;

namespace HospitalApp.ViewModels
{
    public partial class EditAppointmentWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private Appointment appointment;

        public ObservableCollection<Doctor> Doctors { get; set; }
        public ObservableCollection<string> AppointmentTypes { get; set; } = new() { "Check-up", "Consultation", "Surgery" };

        [ObservableProperty]
        private DateTime? selectedAppointmentDate;

        [ObservableProperty]
        private TimeSpan selectedAppointmentTime;

        public ObservableCollection<TimeSpan> AvailableTimeSlots { get; set; } = new()
        {
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            new TimeSpan(11, 0, 0),
            new TimeSpan(14, 0, 0),
            new TimeSpan(15, 0, 0),
        };

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

            Doctors = doctors;
        }
    }
}
