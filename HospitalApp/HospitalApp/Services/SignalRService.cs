using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using HospitalApp.Models;
using HospitalApp;

public class SignalRService
{
    private readonly HubConnection _hubConnection;
    public event Action<int, int>? DoctorAvailabilityUpdated;
    public event Action<Doctor>? DoctorAdded;

    public event Action<int, Appointment>? AppointmentUpdated;
    public event Action<Appointment>? AppointmentAdded;
    public event Action<Records>? RecordAdded; // 👈 new event for record
    public event Action<Medicine>? MedicineAdded;
    public event Action<Medicine>? MedicineUpdated;
    public event Action<int>? MedicineDeleted;

    public SignalRService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5271/hospitalHub")
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<int, int>("UpdateDoctorAvailability", (doctorId, isAvailable) =>
        {
            DoctorAvailabilityUpdated?.Invoke(doctorId, isAvailable);
        });

        _hubConnection.On<Doctor>("DoctorAdded", doctor =>
        {
            DoctorAdded?.Invoke(doctor);
        });

        _hubConnection.On<int, Appointment>("UpdateAppointment", (appointmentID, appointment)=>{
            AppointmentUpdated?.Invoke(appointmentID, appointment);
        });

        _hubConnection.On<Appointment>("AppointmentAdded", appointment =>
        {
            AppointmentAdded?.Invoke(appointment);
        });

        _hubConnection.On<Records>("RecordAdded", record =>
        {
            RecordAdded?.Invoke(record);
        });

        _hubConnection.On<Medicine>("MedicineAdded", medicine =>
        {
            MedicineAdded?.Invoke(medicine);
        });

        _hubConnection.On<Medicine>("MedicineUpdated", medicine =>
        {
            MedicineUpdated?.Invoke(medicine);
        });

        _hubConnection.On<int>("MedicineDeleted", id =>
        {
            MedicineDeleted?.Invoke(id);
        });
        
    }

    public async Task ConnectAsync()
    {
        try
        {
            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("SignalR connection error: " + ex.Message);
        }
    }
}
