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
