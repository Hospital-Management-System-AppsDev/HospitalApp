using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HospitalApp.Models; // Ensure this matches your project namespace
using System.Text.Json;
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

    public async Task<Doctor> AddDoctorAsync(Doctor doctor)
    {
        try 
        {
            var response = await _httpClient.PostAsJsonAsync("doctors/add", doctor);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error adding doctor: {error}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<Doctor>();
            if (result != null)
            {
                Console.WriteLine($"Doctor added successfully: {result.Name}");
                return result;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while adding doctor: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteDoctorAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"doctors/delete/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<Doctor> UpdateDoctorAsync(Doctor doctor)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"doctors/update/{doctor.Id}", doctor);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Doctor>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating doctor: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> LogoutAsync(int id)
    {
        var response = await _httpClient.PatchAsJsonAsync($"doctors/{id}/availability", 0);
        return response.IsSuccessStatusCode;
    }


    public async Task<Doctor> GetDoctorAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Doctor>($"doctors/{id}"); //Endpoint
    }

    public async Task<User> LoginAsync(string username)
    {
        return await _httpClient.GetFromJsonAsync<User>($"users/by-username/{username}");
    }


    public async Task<bool> UpdateDoctorAvailabilityAsync(int doctorId, int isAvailable)
    {
        var response = await _httpClient.PatchAsJsonAsync($"doctors/{doctorId}/availability", isAvailable);
        return response.IsSuccessStatusCode;
    }

    public async Task<Patient> GetPatientAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Patient>($"patients/{id}");
    }

    public async Task<Patient> AddPatientAsync(Patient patient)
    {
        var response = await _httpClient.PostAsJsonAsync("patients/add", patient);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Patient>();
        }
        return null;
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"patients/delete/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Patient>> GetPatientsAsync()
    {
        var response = await _httpClient.GetAsync("patients/all"); //Endpoint
        if (!response.IsSuccessStatusCode) return new List<Patient>();

        var data = await response.Content.ReadFromJsonAsync<List<Patient>>();
        return data ?? new List<Patient>();
    }

    public async Task<List<Appointment>> GetAppointmentsAsync()
    {
        var response = await _httpClient.GetAsync("appointments");
        if (!response.IsSuccessStatusCode) return new List<Appointment>();

        var data = await response.Content.ReadFromJsonAsync<List<Appointment>>();
        return data ?? new List<Appointment>();
    }
    public async Task<Appointment> AddAppointmentAsync(Appointment appointment)
    {
        var response = await _httpClient.PostAsJsonAsync("appointments/add", appointment);

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var result = await response.Content.ReadFromJsonAsync<Appointment>();

                // Debug - check what's being returned
                Console.WriteLine($"API returned - ID: {result?.pkId}, PatientID: {result?.PatientID}, " +
                    $"PatientName: {result?.PatientName}, Doctor: {result?.AssignedDoctor?.Name}");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing appointment response: {ex.Message}");
                return null;
            }
        }

        return null;
    }

    public async Task<bool> UpdateAppointment(int id, Appointment appointment)
    {
        try
        {
            Console.WriteLine($"[API] Sending PUT request to api/appointments/{id}");
            Console.WriteLine($"[API] Request payload: {JsonSerializer.Serialize(appointment)}");
            
            var jsonSerializerOptions = new JsonSerializerOptions { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var json = JsonSerializer.Serialize(appointment, jsonSerializerOptions);
            Console.WriteLine($"[API] Serialized appointment: {json}");
            
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            // Direct HttpClient approach to see raw request data
            var response = await _httpClient.PutAsync($"appointments/{id}", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"[API] Response status: {response.StatusCode}");
            Console.WriteLine($"[API] Response body: {responseBody}");
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API] Appointment update successful for ID: {id}");
                return true;
            }
            
            Console.WriteLine($"[API] Error updating appointment: {responseBody}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API] Exception in UpdateAppointment: {ex.Message}");
            Console.WriteLine($"[API] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<bool> CancelAppointment(int id)
    {
        var response = await _httpClient.DeleteAsync($"appointments/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<DateTime>> GetAvailableTime(int doctorId, DateTime date, string appointmentType)
    {
        try
        {
            var endpoint = $"appointments/available-slots/{doctorId}/{date:yyyy-MM-dd}/{appointmentType.ToLower()}";
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to get available slots: {response.StatusCode}");
                return new List<DateTime>();
            }

            // Define a response wrapper class to match the API response shape
            var result = await response.Content.ReadFromJsonAsync<AvailableSlotResponse>();
            return result?.availableSlots ?? new List<DateTime>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching available time: {ex.Message}");
            return new List<DateTime>();
        }
    }

    public async Task<List<Appointment>> GetAppointmentsByDoctorAsync(int id)
    {
        var response = await _httpClient.GetAsync($"appointments/by-doctor/{id}");
        if (!response.IsSuccessStatusCode) return new List<Appointment>();

        var data = await response.Content.ReadFromJsonAsync<List<Appointment>>();
        return data ?? new List<Appointment>();
    }

    public async Task<bool> UpdateAppointmentStatus(int appointmentID)
    {
        var response = await _httpClient.PatchAsync($"appointments/update-status/{appointmentID}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<Records> AddRecordAsync(Records record)
    {
        var response = await _httpClient.PostAsJsonAsync("records/add-record", record);

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var result = await response.Content.ReadFromJsonAsync<Records>();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing appointment response: {ex.Message}");
                return null;
            }
        }

        return null;
    }

    public async Task<List<Records>> GetRecordByPatientId(int patientid)
    {
        var response = await _httpClient.GetAsync($"records/{patientid}");
        if (!response.IsSuccessStatusCode) return new List<Records>();

        var data = await response.Content.ReadFromJsonAsync<List<Records>>();
        return data ?? new List<Records>();
    }

    public async Task<PatientMedicalInfo> GetMedicalInfoAsync(int patientId)
    {
        var response = await _httpClient.GetAsync($"records/get-medical-info/{patientId}");
        if (!response.IsSuccessStatusCode) return null;

        var data = await response.Content.ReadFromJsonAsync<PatientMedicalInfo>();
        return data;

    }

    public async Task<List<int>> GetNumAppointmentsAsyc(int year, int month)
    {
        var response = await _httpClient.GetAsync($"appointments/getNumAppointments/{year}/{month}");

        if (!response.IsSuccessStatusCode) return new List<int>();

        var data = await response.Content.ReadFromJsonAsync<List<int>>();
        return data ?? new List<int>();
    }

    public async Task<List<int>> GetNumPatientsAsyc(int year, int month)
    {
        var response = await _httpClient.GetAsync($"patients/getnumpatientspermonth/{year}/{month}");

        if (!response.IsSuccessStatusCode) return new List<int>();

        var data = await response.Content.ReadFromJsonAsync<List<int>>();
        return data ?? new List<int>();
    }

    public async Task<int> GetTotalDoctors()
    {
        var response = await _httpClient.GetAsync($"doctors/getnumdoctors");
        if (!response.IsSuccessStatusCode) return 0;

        var data = await response.Content.ReadFromJsonAsync<int>();
        return data;
    }
    public async Task<int> GetNewDoctors(int year, int month)
    {
        var response = await _httpClient.GetAsync($"doctors/getnewdoctors/{year}/{month}");
        if (!response.IsSuccessStatusCode) return 0;

        var data = await response.Content.ReadFromJsonAsync<int>();
        return data;
    }

    public async Task<int> GetTotalPatients()
    {
        var response = await _httpClient.GetAsync($"patients/getnumpatients");
        if (!response.IsSuccessStatusCode) return 0;

        var data = await response.Content.ReadFromJsonAsync<int>();
        return data;
    }
    public async Task<int> GetNewPatients(int year, int month)
    {
        var response = await _httpClient.GetAsync($"patients/getnewpatients/{year}/{month}");
        if (!response.IsSuccessStatusCode) return 0;

        var data = await response.Content.ReadFromJsonAsync<int>();
        return data;
    }

    public async Task<List<int>> GetNumAppointmentsByDoctor(int year, int month, int doctorId)
    {
        var response = await _httpClient.GetAsync($"appointments/by-doctor-count/{year}/{month}/{doctorId}");
        if (!response.IsSuccessStatusCode) return new List<int>();

        var data = await response.Content.ReadFromJsonAsync<List<int>>();
        return data ?? new List<int>();
    }

    public async Task<Dictionary<string, int>> GetAppointmentsByType(int doctorId)
    {
        var response = await _httpClient.GetAsync($"appointments/by-type/{doctorId}");
        if (!response.IsSuccessStatusCode) return new Dictionary<string, int>();

        var data = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return data ?? new Dictionary<string, int>();
    }

    public async Task<List<Medicines>> GetPharmacy()
    {
        try
        {
            var response = await _httpClient.GetAsync("pharmacy");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to get pharmacy: {response.StatusCode}");
                return new List<Medicines>();
            }

            var data = await response.Content.ReadFromJsonAsync<List<Medicines>>();
            return data ?? new List<Medicines>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting pharmacy: {ex.Message}");
            return new List<Medicines>();
        }
    }

    public async Task<Medicines> AddMedicine(Medicines medicine)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("pharmacy", medicine);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to add medicine: {response.StatusCode}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<Medicines>();
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding medicine: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateMedicine(Medicines medicine)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"pharmacy/{medicine.Id}", medicine);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating medicine: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteMedicine(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"pharmacy/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting medicine: {ex.Message}");
            return false;
        }
    }

    public async Task<string> UpdatePatient(Patient patient)
    {
        var response = await _httpClient.PutAsJsonAsync($"patients/update/{patient.PatientID}", patient);

        if (response.IsSuccessStatusCode)
            return "Success";

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var json = JsonDocument.Parse(content);
            var errors = json.RootElement.GetProperty("errors");

            // Get the first error message from the dictionary
            foreach (var prop in errors.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Array && prop.Value.GetArrayLength() > 0)
                {
                    return prop.Value[0].GetString(); // Return the first error message
                }
            }
        }
        catch
        {
            return "An error occurred while parsing the server response.";
        }

        return "Unknown error occurred.";
    }

}

