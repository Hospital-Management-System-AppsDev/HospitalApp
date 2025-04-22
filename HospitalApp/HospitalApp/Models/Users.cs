using System;

namespace HospitalApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Sex { get; set; }
        public string ContactNumber { get; set; }

        public DateTime Birthday { get; set; }

        // Age is calculated from Birthday
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                int age = today.Year - Birthday.Year;
                if (Birthday.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public string Role { get; set; }
    }
}
