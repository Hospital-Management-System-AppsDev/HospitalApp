using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public class PatientWithHealth
    {
        public Patient Patient { get; set; }
        public HealthInformation HealthInfo { get; set; }
    }
