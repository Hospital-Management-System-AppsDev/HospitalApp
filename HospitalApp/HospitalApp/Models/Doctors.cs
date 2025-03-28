namespace HospitalApp.Models
{
    public class Doctor
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string specialization{get; set;}
        public required int is_available{get; set;}
    }
}
