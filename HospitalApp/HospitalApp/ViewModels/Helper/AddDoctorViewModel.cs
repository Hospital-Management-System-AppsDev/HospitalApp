using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Services;
using HospitalApp.Models;
using System.Windows.Input; // For ICommand
using Avalonia.Controls;   // For Window
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace HospitalApp.ViewModels
{
    public partial class AddDoctorViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;

        [ObservableProperty]
        private string _emailErrorMessage;
        [ObservableProperty]
        private string _doctorName;
        [ObservableProperty]
        private string _doctorEmail;
        [ObservableProperty]
        private string _doctorUsername;
        [ObservableProperty]
        private string _doctorPassword;
        [ObservableProperty]
        private string _doctorSex;
        [ObservableProperty]
        private string _doctorContactNumber;
        [ObservableProperty]
        private string _doctorSpecialization;
        [ObservableProperty]
        private DateTime _doctorBirthday;
        [ObservableProperty]
        private ObservableCollection<string> _genderList = new(){"Male", "Female"};
        [ObservableProperty]
        private string _selectedGender;

        [ObservableProperty]
        private string _errorMessage;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private Window _addDoctorWindow;

        public AddDoctorViewModel(ApiService apiService, SignalRService signalRService)
        {
            _apiService = apiService;
            _signalRService = signalRService;
            _doctorBirthday = DateTime.Now;
            SaveCommand = new RelayCommand(() => Save());
            CancelCommand = new RelayCommand(() => Cancel());
        }

        public void SetWindow(Window window)
        {
            _addDoctorWindow = window;
        }

        private async Task Save()
        {
            // Save both files first
            await SaveFile("profile");
            await SaveFile("signature");

            if (string.IsNullOrEmpty(DoctorName) ||
                string.IsNullOrEmpty(DoctorEmail) ||
                string.IsNullOrEmpty(DoctorUsername) ||
                string.IsNullOrEmpty(DoctorPassword) ||
                string.IsNullOrEmpty(SelectedGender) ||
                string.IsNullOrEmpty(DoctorContactNumber) ||
                string.IsNullOrEmpty(DoctorSpecialization) ||
                string.IsNullOrEmpty(FileTextProfilePath) ||
                string.IsNullOrEmpty(FileTextSignaturePath) ||
                string.IsNullOrEmpty(FileText) ||
                string.IsNullOrEmpty(FileTextSignature) ||
                _doctorBirthday == default)
            {
                ErrorMessage = "All fields are required";
                return;
            }

            var newDoctor = new Doctor
            {
                Name = $"Dr. {DoctorName}",
                Email = DoctorEmail,
                Username = DoctorUsername,
                Password = DoctorPassword,
                Sex = SelectedGender,
                ContactNumber = DoctorContactNumber,
                specialization = DoctorSpecialization,
                is_available = 1,
                Birthday = _doctorBirthday,
                profile_picture = _originalFileName != null ? $"avares://HospitalApp/Assets/Doctor/Profile/drp_{_originalFileName}" : null,
                signature = FileTextSignaturePath
            };

            Console.WriteLine($"Profile picture: {newDoctor.profile_picture}");
            Console.WriteLine($"Signature: {newDoctor.signature}");

            _addDoctorWindow?.Close(newDoctor);
        }

        private void Cancel()
        {
            _addDoctorWindow?.Close(null);
        }

        [ObservableProperty] private Bitmap? _imagePreview;
        [ObservableProperty] private string? _fileText;
        [ObservableProperty] private string? _fileTextProfilePath;
        [ObservableProperty] private Bitmap? _imagePreviewSignature;
        [ObservableProperty] private string? _fileTextSignature;
        [ObservableProperty] private string? _fileTextSignaturePath;

        private byte[]? _originalFileBytes;
        private string? _originalFileName;
        private byte[]? _originalFileBytesSignature;
        private string? _originalFileNameSignature;

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
                Console.WriteLine(e.Message);
            }
        }

        [RelayCommand]
        private async Task OpenFileSignature(CancellationToken token)
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
                _originalFileBytesSignature = memoryStream.ToArray();
                _originalFileNameSignature = file.Name;

                memoryStream.Position = 0;
                ImagePreviewSignature = new Bitmap(memoryStream);
                FileTextSignature = $"[Image loaded: {file.Name}]";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [RelayCommand]
        private async Task SaveFile(string type)
        {
            try
            {
                if (type == "profile")
                {
                    if (_originalFileBytes == null || string.IsNullOrWhiteSpace(_originalFileName))
                        throw new Exception("No profile image loaded to save.");

                    if (_originalFileBytes.Length > 20 * 1024 * 1024)
                        throw new Exception("File exceeded 20MB limit.");

                    // Save to a specific folder
                    var basePath = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
                    var appDataPath = Path.Combine(basePath ?? "", "Assets", "Doctor", "Profile");
                    Directory.CreateDirectory(appDataPath); // Ensure folder exists

                    var filePath = Path.Combine(appDataPath, $"drp_{_originalFileName}");
                    await File.WriteAllBytesAsync(filePath, _originalFileBytes);
                    FileTextProfilePath = filePath;
                    Console.WriteLine($"Profile saved to: {FileTextProfilePath}");
                    FileText += $"\nImage saved to: {filePath}";
                }
                else if (type == "signature")
                {
                    if (_originalFileBytesSignature == null || string.IsNullOrWhiteSpace(_originalFileNameSignature))
                        throw new Exception("No signature image loaded to save.");

                    if (_originalFileBytesSignature.Length > 20 * 1024 * 1024)
                        throw new Exception("File exceeded 20MB limit.");

                    // Save to a specific folder
                    var basePath = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
                    var appDataPath = Path.Combine(basePath ?? "", "Assets", "Doctor", "Signatures");
                    Directory.CreateDirectory(appDataPath); // Ensure folder exists

                    var filePath = Path.Combine(appDataPath, $"dsg_{_originalFileNameSignature}");
                    await File.WriteAllBytesAsync(filePath, _originalFileBytesSignature);
                    FileTextSignaturePath = filePath;
                    Console.WriteLine($"Signature saved to: {FileTextSignaturePath}");
                    FileTextSignature += $"\nImage saved to: {filePath}";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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

        partial void OnDoctorEmailChanged(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                EmailErrorMessage = "Email is required";
            }
            else if(!Regex.IsMatch(value, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                EmailErrorMessage = "Invalid email format";
            }
            else
            {
                EmailErrorMessage = "";
            }
        }
    }
}