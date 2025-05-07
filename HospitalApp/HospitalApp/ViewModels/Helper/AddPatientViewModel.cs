using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;
using HospitalApp.ViewModels;

namespace HospitalApp.ViewModels
{
    public partial class AddPatientViewModel : ViewModelBase
    {
        private int _currentPanelIndex = 0;
        private Window _window;

        // Patient data
        private string _patientName = "";
        private string _patientEmail = "";
        private DateTime _patientBday = DateTime.Today;
        private string _patientSex = "";
        private string _patientAddress = "";
        private string _patientBloodType = "";
        private string _patientContactNumber = "";

        // Lifestyle data
        private string _diet = "";
        private string _exercise = "";
        private string _sleep = "";
        private string _smoking = "";
        private string _alcohol = "";

        // Medications list
        private List<Medication> _medications = new List<Medication>();

        // Allergies data
        private bool _hasMedicationAllergies = false;
        private string _medicationAllergies = "";
        private bool _hasLatexAllergy = false;
        private string _latexAllergy = "";
        private bool _hasFoodAllergies = false;
        private string _foodAllergies = "";

        // Conditions data
        private string _conditions = "";

        // Dropdown options
        public List<string> SexOptions { get; } = new List<string>();
        public List<string> BloodTypeOptions { get; } = new List<string>();
        public List<string> DietOptions { get; } = new List<string>();
        public List<string> ExerciseOptions { get; } = new List<string>();
        public List<string> SleepOptions { get; } = new List<string>();
        public List<string> SmokingOptions { get; } = new List<string>();
        public List<string> AlcoholOptions { get; } = new List<string>();

        // Commands
        public IRelayCommand NextPanelCommand { get; }
        public IRelayCommand PreviousPanelCommand { get; }
        public IRelayCommand FinishCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public IRelayCommand AddMedicationCommand { get; }
        public IRelayCommand RemoveMedicationCommand { get; }

        // Properties - Patient Info
        public string PatientName
        {
            get => _patientName;
            set => SetProperty(ref _patientName, value);
        }

        public string PatientEmail
        {
            get => _patientEmail;
            set => SetProperty(ref _patientEmail, value);
        }

        public DateTime PatientBday
        {
            get => _patientBday;
            set => SetProperty(ref _patientBday, value);
        }

        public string PatientSex
        {
            get => _patientSex;
            set => SetProperty(ref _patientSex, value);
        }

        public string PatientAddress
        {
            get => _patientAddress;
            set => SetProperty(ref _patientAddress, value);
        }

        public string PatientBloodType
        {
            get => _patientBloodType;
            set => SetProperty(ref _patientBloodType, value);
        }

        public string PatientContactNumber
        {
            get => _patientContactNumber;
            set => SetProperty(ref _patientContactNumber, value);
        }

        // Properties - Lifestyle
        public string Diet
        {
            get => _diet;
            set => SetProperty(ref _diet, value);
        }

        public string Exercise
        {
            get => _exercise;
            set => SetProperty(ref _exercise, value);
        }

        public string Sleep
        {
            get => _sleep;
            set => SetProperty(ref _sleep, value);
        }

        public string Smoking
        {
            get => _smoking;
            set => SetProperty(ref _smoking, value);
        }

        public string Alcohol
        {
            get => _alcohol;
            set => SetProperty(ref _alcohol, value);
        }

        // Properties - Medications
        public List<Medication> Medications
        {
            get => _medications;
            set => SetProperty(ref _medications, value);
        }

        // Properties - Allergies
        public bool HasMedicationAllergies
        {
            get => _hasMedicationAllergies;
            set => SetProperty(ref _hasMedicationAllergies, value);
        }

        public string MedicationAllergies
        {
            get => _medicationAllergies;
            set => SetProperty(ref _medicationAllergies, value);
        }

        public bool HasLatexAllergy
        {
            get => _hasLatexAllergy;
            set => SetProperty(ref _hasLatexAllergy, value);
        }

        public string LatexAllergy
        {
            get => _latexAllergy;
            set => SetProperty(ref _latexAllergy, value);
        }

        public bool HasFoodAllergies
        {
            get => _hasFoodAllergies;
            set => SetProperty(ref _hasFoodAllergies, value);
        }

        public string FoodAllergies
        {
            get => _foodAllergies;
            set => SetProperty(ref _foodAllergies, value);
        }

        // Properties - Conditions
        public string Conditions
        {
            get => _conditions;
            set => SetProperty(ref _conditions, value);
        }

        // Panel visibility properties
        public bool IsPatientInfoPanelVisible => _currentPanelIndex == 0;
        public bool IsLifestylePanelVisible => _currentPanelIndex == 1;
        public bool IsMedicationPanelVisible => _currentPanelIndex == 2;
        public bool IsAllergiesPanelVisible => _currentPanelIndex == 3;
        public bool IsConditionPanelVisible => _currentPanelIndex == 4;

        // New medication fields
        private string _newMedicationName = "";
        private string _newMedicationDosage = "";
        private string _newMedicationFrequency = "";

        public string NewMedicationName
        {
            get => _newMedicationName;
            set => SetProperty(ref _newMedicationName, value);
        }

        public string NewMedicationDosage
        {
            get => _newMedicationDosage;
            set => SetProperty(ref _newMedicationDosage, value);
        }

        public string NewMedicationFrequency
        {
            get => _newMedicationFrequency;
            set => SetProperty(ref _newMedicationFrequency, value);
        }

        private Medication _selectedMedication;
        public Medication SelectedMedication
        {
            get => _selectedMedication;
            set => SetProperty(ref _selectedMedication, value);
        }

        public AddPatientViewModel()
        {
            NextPanelCommand = new RelayCommand(NextPanel);
            PreviousPanelCommand = new RelayCommand(PreviousPanel);
            FinishCommand = new RelayCommand(Finish);
            CancelCommand = new RelayCommand(Cancel);
            AddMedicationCommand = new RelayCommand(AddMedication);
            RemoveMedicationCommand = new RelayCommand(RemoveMedication);

            Medications = new List<Medication>();

            // Initialize dropdown options
            SexOptions.AddRange(new[] { "Male", "Female", "Other" });
            BloodTypeOptions.AddRange(new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });
            DietOptions.AddRange(new[] { "Vegetarian", "Vegan", "Omnivore", "Pescatarian", "Keto", "Other" });
            ExerciseOptions.AddRange(new[] { "Daily", "3 times / week", "1-2 times / week", "Rarely", "Never" });
            SleepOptions.AddRange(new[] { "Less than 6 hours", "6 hours", "7 hours", "8 hours", "More than 8 hours" });
            SmokingOptions.AddRange(new[] { "Yes", "No", "Former" });
            AlcoholOptions.AddRange(new[] { "Never", "Occasional", "Regular", "Heavy" });
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        private void NextPanel()
        {
            if (_currentPanelIndex < 4)
            {
                _currentPanelIndex++;
                UpdatePanelVisibility();
            }
        }

        private void PreviousPanel()
        {
            if (_currentPanelIndex > 0)
            {
                _currentPanelIndex--;
                UpdatePanelVisibility();
            }
        }

        private void UpdatePanelVisibility()
        {
            OnPropertyChanged(nameof(IsPatientInfoPanelVisible));
            OnPropertyChanged(nameof(IsLifestylePanelVisible));
            OnPropertyChanged(nameof(IsMedicationPanelVisible));
            OnPropertyChanged(nameof(IsAllergiesPanelVisible));
            OnPropertyChanged(nameof(IsConditionPanelVisible));
        }

        private void AddMedication()
        {
            if (!string.IsNullOrWhiteSpace(NewMedicationName))
            {
                var medication = new Medication
                {
                    Name = NewMedicationName,
                    Dosage = NewMedicationDosage,
                    Frequency = NewMedicationFrequency
                };

                var updatedList = new List<Medication>(Medications) { medication };
                Medications = updatedList;

                NewMedicationName = "";
                NewMedicationDosage = "";
                NewMedicationFrequency = "";

                OnPropertyChanged(nameof(Medications));
            }
        }

        private void RemoveMedication()
        {
            if (SelectedMedication != null)
            {
                var updatedList = new List<Medication>(Medications);
                updatedList.Remove(SelectedMedication);
                Medications = updatedList;

                OnPropertyChanged(nameof(Medications));
            }
        }

        private void Finish()
        {
            var patient = new Patient
            {
                Name = PatientName,
                Email = PatientEmail,
                Bday = PatientBday,
                Sex = PatientSex,
                Address = PatientAddress,
                BloodType = PatientBloodType,
                ContactNumber = PatientContactNumber
            };

            var healthInfo = new HealthInformation
            {
                Diet = Diet,
                Exercise = Exercise,
                Sleep = Sleep,
                Smoking = Smoking,
                Alcohol = Alcohol,
                Medications = Medications,
                HasMedicationAllergies = HasMedicationAllergies,
                MedicationAllergies = MedicationAllergies,
                HasLatexAllergy = HasLatexAllergy,
                LatexAllergy = LatexAllergy,
                HasFoodAllergies = HasFoodAllergies,
                FoodAllergies = FoodAllergies,
                Conditions = Conditions
            };

            var patientWithHealth = new PatientWithHealth
            {
                Patient = patient,
                HealthInfo = healthInfo
            };

            _window?.Close(patientWithHealth);
        }

        private void Cancel()
        {
            _window?.Close(null);
        }
    }
}