namespace HospitalApp.Models
{
    public class Doctor : User
    {
        public string specialization{get; set;}
        public int is_available{get; set;}
    }
}
