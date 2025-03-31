namespace HospitalApp.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string specialization{get; set;}
        public int is_available{get; set;}
        public string Username{get; set;}
        public string Password{get; set;}
        public string Gender{get; set;}
        public string ContactNumber{get; set;}
        public int Age{get; set;}
    }
}
