using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using HospitalApp.Models;

public class SignalRService
{
    private readonly HubConnection _hubConnection;
    public event Action<int, int>? DoctorAvailabilityUpdated;
    public event Action<Doctor>? DoctorAdded;

    public SignalRService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5271/doctorHub")
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
