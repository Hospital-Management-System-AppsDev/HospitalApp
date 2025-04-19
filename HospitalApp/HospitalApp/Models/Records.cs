using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public class Records
{   
    public int id{ get; set; }
    public Patient patient { get; set; }
    public Appointment appointment { get; set; }
    public string medicalCertificatePath { get; set; }
    public string prescriptionPath { get; set; }
    public string diagnosisPath { get; set; } // 0 = pending, 1 = completed, etc.
}
