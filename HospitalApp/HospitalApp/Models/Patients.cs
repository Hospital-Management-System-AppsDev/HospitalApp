using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalApp.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        public string Name { get; set; }

        public DateTime Bday { get; set; }

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                int age = today.Year - Bday.Year;
                if (Bday.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public string Sex { get; set; }
        public string Address { get; set; }
        public string BloodType { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "Email should not contain spaces")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Contact number must be exactly 11 digits")]
        public string ContactNumber { get; set; }
        
        public string ProfilePicture { get; set; }
        public PatientMedicalInfo PatientMedicalInfo { get; set; }
    }
}
