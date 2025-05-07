using System;
using System.Collections.Generic;

namespace HospitalApp.Models;

public class HealthInformation
    {
        public string Diet { get; set; }
        public string Exercise { get; set; }
        public string Sleep { get; set; }
        public string Smoking { get; set; }
        public string Alcohol { get; set; }
        public List<Medication> Medications { get; set; } = new List<Medication>();
        public bool HasMedicationAllergies { get; set; }
        public string MedicationAllergies { get; set; }
        public bool HasLatexAllergy { get; set; }
        public string LatexAllergy { get; set; }
        public bool HasFoodAllergies { get; set; }
        public string FoodAllergies { get; set; }
        public string Conditions { get; set; }
    }