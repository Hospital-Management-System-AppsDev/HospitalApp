using HospitalApp.ViewModels;

namespace HospitalApp.Models
{
    public class Medicine : ViewModelBase
    {
        private string _name;
        private string _description;
        private string _price; // Changed to string
        private string _stock; // Changed to string
        private string _manufacturer;
        private string _category;
        private string _dosageForm;
        private string _strength;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public string Stock
        {
            get => _stock;
            set
            {
                _stock = value;
                OnPropertyChanged();
            }
        }

        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                _manufacturer = value;
                OnPropertyChanged();
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
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

        public string Strength
        {
            get => _strength;
            set
            {
                _strength = value;
                OnPropertyChanged();
            }
        }
    }

    public class CartItem : ViewModelBase
    {
        private int _quantity;

        public Medicine Medicine { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        // Updated to parse Price as decimal
        public decimal TotalPrice => (decimal.TryParse(Medicine.Price, out var price) ? price : 0) * Quantity;

        public string Name => Medicine.Name;
    }
}