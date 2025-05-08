using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models;
using Avalonia.Controls.ApplicationLifetimes;


namespace HospitalApp.ViewModels
{
    public partial class AddPatientViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _errorMessage = "";
        
        private int _currentPanelIndex = 0;
        private Window _window;

        [ObservableProperty]
        private string _patientName = "";
        [ObservableProperty]
        private string _patientEmail = "";
        [ObservableProperty]
        private DateTime _patientBday = DateTime.Today;
        [ObservableProperty]
        private string _patientSex = "";
        [ObservableProperty]
        private string _patientAddress = "";
        [ObservableProperty]
        private string _patientBloodType = "";
        [ObservableProperty]
        private string _patientContactNumber = "";

        [ObservableProperty]
        private string _patientDiet = "";
        [ObservableProperty]
        private string _patientExercise = "";
        [ObservableProperty]
        private string _patientSleep = "";
        [ObservableProperty]
        private string _patientSmoking = "";
        [ObservableProperty]
        private string _patientAlcohol = "";
        [ObservableProperty]
        private string _patientCurrentMedication = "";
        [ObservableProperty]
        private string _patientMedicalAllergies = "";
        [ObservableProperty]
        private bool _patientLatexAllergy = false;
        [ObservableProperty]
        private string _patientFoodAllergies = "";
        [ObservableProperty]
        private string _patientOtherAllergies = "";

        [ObservableProperty]
        private bool hasfoodallergies = false;
        [ObservableProperty]
        private bool hasmedicationallergies = false;

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

        // Panel visibility properties
        public bool IsPatientInfoPanelVisible => _currentPanelIndex == 0;
        public bool IsLifestylePanelVisible => _currentPanelIndex == 1;
        public bool IsMedicationPanelVisible => _currentPanelIndex == 2;
        public bool IsAllergiesPanelVisible => _currentPanelIndex == 3;

        public AddPatientViewModel()
        {
            NextPanelCommand = new RelayCommand(NextPanel);
            PreviousPanelCommand = new RelayCommand(PreviousPanel);
            FinishCommand = new RelayCommand(async () => await Finish());
            CancelCommand = new RelayCommand(Cancel);
            // Initialize dropdown options
            SexOptions.AddRange(new[] { "Male", "Female" });
            BloodTypeOptions.AddRange(new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });
            DietOptions.AddRange(new[] { "Vegetarian", "Vegan", "Omnivore", "Pescatarian", "Keto", "Other" });
            ExerciseOptions.AddRange(new[] { "Daily", "Occasionally", "Rarely", "Never" });
            SleepOptions.AddRange(new[] { "7 hours", "Less than 6 hours", "6-7 hours", "8+ hours" });
            SmokingOptions.AddRange(new[] { "Yes", "No", "Former smoker" });
            AlcoholOptions.AddRange(new[] { "Occasional", "Daily", "Weekly", "Rarely", "Never" });
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        private void NextPanel()
        {
            // Clear previous error message
            ErrorMessage = "";
            
            // Validate based on current panel
            if (_currentPanelIndex == 0)
            {
                // Validate Patient Info Panel
                if (string.IsNullOrWhiteSpace(PatientName))
                {
                    ErrorMessage = "Please enter patient name.";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(PatientEmail))
                {
                    ErrorMessage = "Please enter patient email.";
                    return;
                }
                
                // Validate email format using regex
                if (!IsValidEmail(PatientEmail))
                {
                    ErrorMessage = "Please enter a valid email address.";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(PatientSex))
                {
                    ErrorMessage = "Please select patient sex.";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(PatientAddress))
                {
                    ErrorMessage = "Please enter patient address.";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(PatientBloodType))
                {
                    ErrorMessage = "Please select patient blood type.";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(PatientContactNumber))
                {
                    ErrorMessage = "Please enter patient contact number.";
                    return;
                }
                
                // Additional validation for date of birth
                if (PatientBday > DateTime.Today)
                {
                    ErrorMessage = "Birth date cannot be in the future.";
                    return;
                }
            }
            else if (_currentPanelIndex == 1)
            {
                // Validate Lifestyle Panel
                if (string.IsNullOrWhiteSpace(PatientDiet))
                {
                    ErrorMessage = "Please select patient diet.";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(PatientExercise))
                {
                    ErrorMessage = "Please select patient exercise habit.";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(PatientSleep))
                {
                    ErrorMessage = "Please select patient sleep pattern.";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(PatientSmoking))
                {
                    ErrorMessage = "Please select patient smoking status.";
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(PatientAlcohol))
                {
                    ErrorMessage = "Please select patient alcohol consumption.";
                    return;
                }
            }
            
            // If validation passes, proceed to next panel
            if (_currentPanelIndex < 3)
            {
                _currentPanelIndex++;
                UpdatePanelVisibility();
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
                
            try
            {
                // This regex pattern checks for the format of name@domain.tld
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void PreviousPanel()
        {
            // Clear error message when going back
            ErrorMessage = "";
            
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
        }

        private async Task Finish()
        {
            // Validate allergies panel if needed
            if (hasmedicationallergies && string.IsNullOrWhiteSpace(PatientMedicalAllergies))
            {
                ErrorMessage = "Please list medication allergies.";
                return;
            }
            
            if (hasfoodallergies && string.IsNullOrWhiteSpace(PatientFoodAllergies))
            {
                ErrorMessage = "Please list food allergies.";
                return;
            }

            await SaveFile();
            
            var patient = new Patient
            {
                Name = PatientName,
                Email = PatientEmail,
                Bday = PatientBday,
                Sex = PatientSex,
                Address = PatientAddress,
                BloodType = PatientBloodType,
                ContactNumber = PatientContactNumber,
                ProfilePicture = _originalFileName != null ? $"avares://HospitalApp/Assets/Patients/Profiles/pfp_{_originalFileName}" : null,
                PatientMedicalInfo = new PatientMedicalInfo
                {
                    diet = PatientDiet,
                    exercise = PatientExercise,
                    sleep = PatientSleep,
                    smoking = PatientSmoking,
                    alcohol = PatientAlcohol,
                    currentMedication = PatientCurrentMedication,
                    medicalAllergies = PatientMedicalAllergies,
                    latexAllergy = PatientLatexAllergy,
                    foodAllergy = PatientFoodAllergies,
                    otherAllergies = PatientOtherAllergies,
                }
            };

            _window?.Close(patient);
        }

        private void Cancel()
        {
            _window?.Close(null);
        }

         [ObservableProperty] private Bitmap? _imagePreview;
        [ObservableProperty] private string? _fileText;
        [ObservableProperty] private string? _fileTextProfilePath;

        private byte[]? _originalFileBytes;
        private string? _originalFileName;

        [RelayCommand]
        private async Task OpenFileProfilePicture(CancellationToken token)
        {
            try
            {
                var file = await DoOpenFilePickerAsync();
                if (file is null) return;

                var fileName = file.Name.ToLowerInvariant();
                var isImage = fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") ||
                            fileName.EndsWith(".png") || fileName.EndsWith(".bmp") || fileName.EndsWith(".gif") || fileName.EndsWith(".webp");

                if (!isImage)
                    throw new Exception("Only image files are supported.");

                var size = (await file.GetBasicPropertiesAsync()).Size;
                if (size > 20 * 1024 * 1024)
                    throw new Exception("File exceeded 20MB limit.");

                await using var readStream = await file.OpenReadAsync();
                await using var memoryStream = new MemoryStream();
                await readStream.CopyToAsync(memoryStream);
                _originalFileBytes = memoryStream.ToArray();
                _originalFileName = file.Name;

                memoryStream.Position = 0;
                ImagePreview = new Bitmap(memoryStream);
                FileText = $"[Image loaded: {file.Name}]";
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                Console.WriteLine(e.Message);
            }
        }

        [RelayCommand]
        private async Task SaveFile()
        {
            try
            {
                // Validate file presence and size
                if (_originalFileBytes == null || string.IsNullOrWhiteSpace(_originalFileName))
                    throw new InvalidOperationException("No profile image loaded to save.");

                if (_originalFileBytes.Length > 20 * 1024 * 1024)
                    throw new InvalidOperationException("File size exceeds the 20MB limit.");

                // Construct the save path relative to the project structure
                var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
                if (string.IsNullOrWhiteSpace(projectRoot))
                    throw new DirectoryNotFoundException("Project root directory could not be determined.");

                var saveDirectory = Path.Combine(projectRoot, "Assets", "Patients", "Profiles");
                Directory.CreateDirectory(saveDirectory); // Ensure the target directory exists

                // Build and write the file
                var fileName = $"pfp_{_originalFileName}";
                var filePath = Path.Combine(saveDirectory, fileName);

                await File.WriteAllBytesAsync(filePath, _originalFileBytes);

                // Update UI-related or binding properties
                FileTextProfilePath = filePath;
                FileText += $"\nImage saved to: {filePath}";

                Console.WriteLine($"Profile image saved successfully at: {filePath}");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Console.WriteLine($"Error saving file: {ex}");
            }
        }


        private async Task<IStorageFile?> DoOpenFilePickerAsync()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.StorageProvider is not { } provider)
                throw new NullReferenceException("Missing StorageProvider instance.");

            var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Image File",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Image Files")
                    {
                        Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif", "*.webp" }
                    }
                }
            });

            return files?.Count >= 1 ? files[0] : null;
        }
    }
}