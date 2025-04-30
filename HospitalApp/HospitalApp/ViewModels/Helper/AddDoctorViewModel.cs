using System;
using System.Windows.Input;
using Avalonia.Controls;
using HospitalApp.Models;
using Avalonia;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;

namespace HospitalApp.ViewModels
{
    public class AddDoctorViewModel : ViewModelBase
    {
        private string _doctorName;
        private string _doctorEmail;
        private string _doctorUsername;
        private string _doctorSex;
        private string _doctorContactNumber;
        private string _doctorSpecialization;
        private DateTime _doctorBirthday;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private Window _addDoctorWindow;

        public string DoctorName
        {
            get => _doctorName;
            set
            {
                _doctorName = value;
                OnPropertyChanged();
            }
        }

        public string DoctorEmail
        {
            get => _doctorEmail;
            set
            {
                _doctorEmail = value;
                OnPropertyChanged();
            }
        }

        public string DoctorUsername
        {
            get => _doctorUsername;
            set
            {
                _doctorUsername = value;
                OnPropertyChanged();
            }
        }

        public string DoctorGender
        {
            get => _doctorSex;
            set
            {
                _doctorSex = value;
                OnPropertyChanged();
            }
        }

        public string DoctorContactNumber
        {
            get => _doctorContactNumber;
            set
            {
                _doctorContactNumber = value;
                OnPropertyChanged();
            }
        }
        
        public string DoctorSpecialization
        {
            get => _doctorSpecialization;
            set
            {
                _doctorSpecialization = value;
                OnPropertyChanged();
            }
        }

        public string DoctorBirthday
        {
            get => _doctorBirthday.ToString();
            set
            {
                if (DateTime.TryParse(value, out DateTime parsedDate))
                {
                    _doctorBirthday = parsedDate;
                    OnPropertyChanged();
                }
            }
        }

        public AddDoctorViewModel()
        {
            _doctorBirthday = DateTime.Now;
            SaveCommand = new RelayCommand(() => Save());
            CancelCommand = new RelayCommand(() => Cancel());
        }

        public void SetWindow(Window window)
        {
            _addDoctorWindow = window;
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(DoctorName) ||
                string.IsNullOrEmpty(DoctorEmail) ||
                string.IsNullOrEmpty(DoctorUsername) ||
                string.IsNullOrEmpty(DoctorGender) ||
                string.IsNullOrEmpty(DoctorContactNumber) ||
                string.IsNullOrEmpty(DoctorSpecialization) ||
                _doctorBirthday == default)
            {
                Console.WriteLine("All fields are required");
                return;
            }

            var newDoctor = new Doctor
            {
                Name = DoctorName,
                Email = DoctorEmail,
                Username = DoctorUsername,
                Sex = DoctorGender,
                ContactNumber = DoctorContactNumber,
                specialization = DoctorSpecialization,
                is_available = 1,
                Birthday = _doctorBirthday
            };

            _addDoctorWindow?.Close(newDoctor);
        }

        private void Cancel()
        {
            _addDoctorWindow?.Close(null);
        }
    }
}