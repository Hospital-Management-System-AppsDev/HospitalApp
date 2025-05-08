using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

    public PatientMedicalInfo patientMedicalInfo{get; set;}

    public decimal temperature { get; set; }
    public int pulseRate { get; set; }
    public decimal weight { get; set; }
    public decimal height { get; set; }
    public decimal sugarLevel { get; set; }

    [RegularExpression(@"^\d{2,3}/\d{2,3}$", ErrorMessage = "Blood pressure must be in the format 'mmHg/mmHg'")]
    public string bloodPressure { get; set; }

    public decimal bmi{
        get
        {
            decimal heightInMeters = height / 100; // Convert height from cm to meters
            return weight / (heightInMeters * heightInMeters);
        }
    }

    public string chiefComplaint{get; set;}

}
