using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;

namespace HospitalApp.ViewModels
{
    public partial class PatientPageViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Patient> patients = new();

        [ObservableProperty]
        private ObservableCollection<Patient> filteredPatients = new();

        [ObservableProperty]
        private Patient selectedPatient;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string errorMessage;

        public PatientPageViewModel()
        {
            LoadSampleData();
            FilterPatients();
        }

        private void LoadSampleData()
        {
            Patients.Clear();
            Patients.Add(new Patient
            {
                PatientID = 1,
                Name = "Juan Dela Cruz",
                Sex = "Male",
                Age = 21,
                Bday = new DateTime(1978, 2, 9),
                BloodType = "O+",
                Address = "Cebu City",
                ContactNumber = "442-143-1432",
                Email = "juandela@gmail.com"
            });

            Patients.Add(new Patient
            {
                PatientID = 2,
                Name = "Maria Santos",
                Sex = "Female",
                Age = 45,
                Bday = new DateTime(1980, 5, 15),
                BloodType = "A+",
                Address = "Manila",
                ContactNumber = "555-987-6543",
                Email = "maria.santos@email.com"
            });

            Patients.Add(new Patient
            {
                PatientID = 3,
                Name = "Pedro Reyes",
                Sex = "Male",
                Age = 32,
                Bday = new DateTime(1992, 8, 22),
                BloodType = "B-",
                Address = "Davao City",
                ContactNumber = "555-123-4567",
                Email = "pedro.reyes@email.com"
            });
        }

        public Task InitializeAsync()
        {
            FilterPatients();
            return Task.CompletedTask;
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterPatients();
        }

        private void FilterPatients()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredPatients = new ObservableCollection<Patient>(Patients);
            }
            else
            {
                var filtered = Patients
                    .Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                FilteredPatients = new ObservableCollection<Patient>(filtered);
            }
        }

        [RelayCommand]
        public void EditPatient()
        {
            IsEditing = !IsEditing;
        }

        [RelayCommand]
        public void SaveChanges()
        {
            if (SelectedPatient == null)
            {
                ErrorMessage = "No patient selected";
                return;
            }

            try
            {
                int index = Patients.IndexOf(Patients.FirstOrDefault(p => p.PatientID == SelectedPatient.PatientID));
                if (index >= 0)
                {
                    Patients[index] = SelectedPatient;
                }

                FilterPatients();
                ErrorMessage = string.Empty;
                IsEditing = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating patient: {ex.Message}";
            }
        }

        [RelayCommand]
        public void DeletePatient()
        {
            if (SelectedPatient == null)
            {
                ErrorMessage = "No patient selected";
                return;
            }

            try
            {
                Patients.Remove(SelectedPatient);
                FilterPatients();
                SelectedPatient = null;
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting patient: {ex.Message}";
            }
        }
    }
}