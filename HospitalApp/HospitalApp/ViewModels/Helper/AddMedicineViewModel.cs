using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using HospitalApp.Models;
using HospitalApp.Services;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HospitalApp.ViewModels;

public class AddMedicineViewModel : ViewModelBase
{
    private string _medicineName;
    private decimal _medicinePrice;
    private int _medicineQuantity;
    private string _medicineManufacturer;
    private string _medicineType;
    private decimal _medicineDosage;
    private string _medicineUnit;
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private Window _window;

    public string MedicineName
    {
        get => _medicineName;
        set
        {
            _medicineName = value;
            OnPropertyChanged();
        }
    }

    public decimal MedicinePrice
    {
        get => _medicinePrice;
        set
        {
            _medicinePrice = value;
            OnPropertyChanged();
        }
    }

    public int MedicineQuantity
    {
        get => _medicineQuantity;
        set
        {
            _medicineQuantity = value;
            OnPropertyChanged();
        }
    }

    public string MedicineManufacturer
    {
        get => _medicineManufacturer;
        set
        {
            _medicineManufacturer = value;
            OnPropertyChanged();
        }
    }

    public string MedicineType
    {
        get => _medicineType;
        set
        {
            _medicineType = value;
            OnPropertyChanged();
        }
    }

    public decimal MedicineDosage
    {
        get => _medicineDosage;
        set
        {
            _medicineDosage = value;
            OnPropertyChanged();
        }
    }

    public string MedicineUnit
    {
        get => _medicineUnit;
        set
        {
            _medicineUnit = value;
            OnPropertyChanged();
        }
    }

    public AddMedicineViewModel()
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
        var newMedicine = new Medicines
        {
            Name = MedicineName,
            Price = MedicinePrice,
            Stocks = MedicineQuantity,
            Manufacturer = MedicineManufacturer,
            Type = MedicineType,
            Dosage = MedicineDosage,
            Unit = MedicineUnit
        };

        _window?.Close(newMedicine);
    }

    private void Cancel()
    {
        _window?.Close(null);
    }
}