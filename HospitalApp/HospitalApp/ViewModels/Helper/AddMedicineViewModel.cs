using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using HospitalApp.Models; // If you’re using a Medicine model

namespace HospitalApp.ViewModels;

public class AddMedicineViewModel : ViewModelBase
{
    private string _medicineName;
    private string _medicinePrice;
    private string _medicineQuantity;
    private string _medicineManufacturer;
    private string _medicineType;
    private string _dosageForm;
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

    public string MedicinePrice
    {
        get => _medicinePrice;
        set
        {
            _medicinePrice = value;
            OnPropertyChanged();
        }
    }

    public string MedicineQuantity
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
    public string DosageForm
    {
        get => _dosageForm;
        set
        {
            _dosageForm = value;
            OnPropertyChanged();
        }
    }


    public AddMedicineViewModel()
    {
        SaveCommand = new RelayCommand(_ => Save());
        CancelCommand = new RelayCommand(_ => Cancel());
    }

    public void SetWindow(Window window)
    {
        _window = window;
    }

    private void Save()
    {
        var newMedicine = new Medicine
        {
            Name = MedicineName,
            Price = MedicinePrice,
            Stock = MedicineQuantity,
            Manufacturer = MedicineManufacturer,
            Category = MedicineType,
            DosageForm = DosageForm
        };

        _window?.Close(newMedicine);
    }

    private void Cancel()
    {
        _window?.Close(null);
    }
}