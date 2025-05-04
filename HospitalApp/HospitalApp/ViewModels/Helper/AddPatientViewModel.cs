using System;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using HospitalApp.Models; 

namespace HospitalApp.ViewModels;

public class AddPatientViewModel : ViewModelBase
{
    private string _patientName;
    private string _patientEmail;
    private string _patientBday;
    private string _patientSex;
    private string _patientAddress;
    private string _patientBloodtype;
    private string _patientContactNumber;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private Window _window;

    public string PName
    {
        get => _patientName;
        set
        {
            _patientName = value;
            OnPropertyChanged();
        }
    }

    public string PEmail
    {
        get => _patientEmail;
        set
        {
            _patientEmail = value;
            OnPropertyChanged();
        }
    }

    public string PSex
    {
        get => _patientSex;
        set
        {
            _patientSex = value;
            OnPropertyChanged();
        }
    }

    public string PAddress
    {
        get => _patientAddress;
        set
        {
            _patientAddress = value;
            OnPropertyChanged();
        }
    }

    public string PBloodType
    { //todo add list every possible bloodtype
        get => _patientBloodtype;
        set
        {
            _patientBloodtype = value;
            OnPropertyChanged();
        }
    }
    public string PContactNumber
    {
        get => _patientContactNumber;
        set
        {
            _patientContactNumber = value;
            OnPropertyChanged();
        }
    }

    public string PBday
        {
            get => _patientBday.ToString();
            set
            {
                if (DateTime.TryParse(value, out DateTime parsedDate))
                {
                    _patientBday = parsedDate.ToString();
                    OnPropertyChanged();
                }
            }
        }


    public AddPatientViewModel()
    {
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public void SetWindow(Window window)
    {
        _window = window;
    }

    private void Save()
    {
        var newPatient = new Patient
        {
            Name = PName,
            Bday = DateTime.Parse(PBday),
            Sex = PSex,
            Address = PAddress,
            BloodType = PBloodType,
            Email = PEmail,
            ContactNumber = PContactNumber
        };

        _window?.Close(newPatient);
    }

    private void Cancel()
    {
        _window?.Close(null);
    }
}