using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HospitalApp.Models;

public partial class Appointment : ObservableObject
{
    [ObservableProperty] public int pkId;
    [ObservableProperty] public int patientID;
    [ObservableProperty] public string patientName;
    [ObservableProperty] public Doctor assignedDoctor;
    [ObservableProperty] public string appointmentType;
    [ObservableProperty] public int status;
    [ObservableProperty] public DateTime appointmentDateTime;

    public DateTime? AppointmentDateForPicker
    {
        get => AppointmentDateTime.Date;
        set
        {
            if (value.HasValue)
            {
                AppointmentDateTime = new DateTime(
                    value.Value.Year,
                    value.Value.Month,
                    value.Value.Day,
                    AppointmentDateTime.Hour,
                    AppointmentDateTime.Minute,
                    0);
            }
        }
    }


    public TimeSpan AppointmentTime
    {
        get => AppointmentDateTime.TimeOfDay;
        set => AppointmentDateTime = AppointmentDateTime.Date + value;
    }

    public Appointment()
    {

    }

    public Appointment(int patientID, string patientName, Doctor doctor, string appointmentType, int status, DateTime appointmentDateTime)
    {
        PatientID = patientID;
        PatientName = patientName;
        AssignedDoctor = doctor;
        AppointmentType = appointmentType;
        Status = status;
        AppointmentDateTime = appointmentDateTime;
    }




}
