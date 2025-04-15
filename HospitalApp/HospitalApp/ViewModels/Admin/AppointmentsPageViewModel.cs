using System.Collections.ObjectModel;

namespace HospitalApp.ViewModels
{
    public class Appointment
    {
        public string PatientId { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string TransactionType { get; set; } = "";
        public string Doctor { get; set; } = "";
        public string Status { get; set; } = "Pending";
    }


    public class AppointmentsPageViewModel : ViewModelBase
    {
        public ObservableCollection<Appointment> Appointments { get; set; } = new()
        {
            new Appointment { PatientId = "00000000", PatientName = "Juan Dela Cruz", TransactionType = "XXXXXXXXXX", Doctor = "Dr. Adamusa Pingay", Status = "Selected Option" },
            new Appointment { PatientId = "00000000", PatientName = "Meoewmoew", TransactionType = "XXXXXXXXXX", Doctor = "Dr. Theo Pondar", Status = "Selected Option" },
            new Appointment { PatientId = "00000000", PatientName = "Nigs", TransactionType = "XXXXXXXXXX", Doctor = "Dr. Enya Moncayo", Status = "Selected Option" },
            new Appointment { PatientId = "00000000", PatientName = "Adam", TransactionType = "XXXXXXXXXX", Doctor = "Dr. Jc Atillo", Status = "Selected Option" },
            new Appointment { PatientId = "00000000", PatientName = "Michael", TransactionType = "XXXXXXXXXX", Doctor = "Dr. Theo Pondar", Status = "Selected Option" },
            new Appointment { PatientId = "00000000", PatientName = "Kem", TransactionType = "XXXXXXXXXX", Doctor = "Dr. Enya Moncayo", Status = "Selected Option" },
            new Appointment { PatientId = "00000000", PatientName = "DEez", TransactionType = "XXXXXXXXXX", Doctor = "Dr. Jc Atillo", Status = "Selected Option" }
        };
    }
}
