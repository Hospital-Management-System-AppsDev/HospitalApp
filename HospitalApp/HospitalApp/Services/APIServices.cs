using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HospitalApp.Models; // Ensure this matches your project namespace

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new System.Uri("http://localhost:5271/api/") //BaseURL
        };
    }

    public async Task<List<Doctor>> GetDoctorsAsync()
    {
    var response = await _httpClient.GetAsync("doctors"); //Endpoint
    if (!response.IsSuccessStatusCode) return new List<Doctor>();
    
    var data = await response.Content.ReadFromJsonAsync<List<Doctor>>();
    return data ?? new List<Doctor>();
    }


    public async Task<Doctor> GetDoctorAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Doctor>($"doctors/{id}"); //Endpoint
    }

    public async Task<User> LoginAsync(string username) {
        return await _httpClient.GetFromJsonAsync<User>($"users/by-username/{username}");
    }


    public async Task<bool> UpdateDoctorAvailabilityAsync(int doctorId, int isAvailable)
    {
        var response = await _httpClient.PatchAsJsonAsync($"doctors/{doctorId}/availability", isAvailable);
        return response.IsSuccessStatusCode;
    }

    public async Task<Patient> GetPatientAsync(int id){
        return await _httpClient.GetFromJsonAsync<Patient>($"patients/{id}");
    }

    public async Task<List<Appointment>> GetAppointmentsAsync(){
        var response = await _httpClient.GetAsync("appointments");
        if (!response.IsSuccessStatusCode) return new List<Appointment>();

        var data = await response.Content.ReadFromJsonAsync<List<Appointment>>();
        return data ?? new List<Appointment>();
    }
    public async Task<bool> AddAppointmentAsync(Appointment appointment)
    {
        var response = await _httpClient.PostAsJsonAsync("appointments/add", appointment);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAppointment(int appointmentID, Appointment appointment)
    {
        var response = await _httpClient.PutAsJsonAsync($"appointments/{appointmentID}", appointment);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CancelAppointment(int id)
    {
        var response = await _httpClient.DeleteAsync($"appointments/{id}");
        return response.IsSuccessStatusCode;
    }
}

