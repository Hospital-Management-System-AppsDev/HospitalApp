using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HospitalApp.Models;

public partial class AvailableSlotResponse
{
    public int doctorId { get; set; }
    public string date { get; set; }
    public string appointmentType { get; set; }
    public string slotDuration { get; set; }
    public List<DateTime> availableSlots { get; set; }
}
