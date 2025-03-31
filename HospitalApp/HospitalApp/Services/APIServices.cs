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


    // public async Task<Doctor> GetDoctorAsync(int id)
    // {
    //     return await _httpClient.GetFromJsonAsync<Doctor>($"doctors/{id}"); //Endpoint
    // }

    // public async Task<Doctor> GetDoctorAsync(string name){
    //     return await _httpClient.GetFromJsonAsync<Doctor>($"doctors/{name}"); //Endpoint
    // }

    public async Task<bool> UpdateDoctorAvailabilityAsync(int doctorId, int isAvailable)
    {
        var response = await _httpClient.PatchAsJsonAsync($"doctors/{doctorId}/availability", isAvailable);
        return response.IsSuccessStatusCode;
    }

}
