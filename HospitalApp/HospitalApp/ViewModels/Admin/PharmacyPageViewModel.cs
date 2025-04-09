using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using HospitalApp.Models;

namespace HospitalApp.ViewModels
{
    public class PharmacyPageViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private ObservableCollection<Medicine> _filteredItems;
        private Medicine _selectedMedicine;
        private ObservableCollection<CartItem> _cartItems;

        private int _quantityToAdd = 1;
        public int QuantityToAdd
        {
            get => _quantityToAdd;
            set
            {
                _quantityToAdd = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Medicine> Medicines { get; } = new()
        {
            new Medicine { Name = "Paracetamol", Description = "Common pain reliever and fever reducer", Price = 5.99M, Stock = 100, Manufacturer = "Pharma Inc.", Category = "Pain Relief", DosageForm = "Tablet", Strength = "500mg" },
            new Medicine { Name = "Loperamide", Description = "Anti-diarrheal medication", Price = 7.50M, Stock = 75, Manufacturer = "MediCorp", Category = "Gastrointestinal", DosageForm = "Capsule", Strength = "2mg" },
            new Medicine { Name = "Ibuprofen", Description = "Non-steroidal anti-inflammatory drug", Price = 6.75M, Stock = 120, Manufacturer = "Pharma Inc.", Category = "Pain Relief", DosageForm = "Tablet", Strength = "400mg" },
            new Medicine { Name = "Amoxicillin", Description = "Antibiotic medication", Price = 12.99M, Stock = 50, Manufacturer = "HealthPharm", Category = "Antibiotics", DosageForm = "Capsule", Strength = "500mg" },
            new Medicine { Name = "Cetirizine", Description = "Antihistamine for allergy relief", Price = 8.25M, Stock = 85, Manufacturer = "AllergyRx", Category = "Allergy", DosageForm = "Tablet", Strength = "10mg" },
            // Keep all other medicines from your original list - truncated for brevity
            new Medicine { Name = "Aspirin", Description = "Blood thinner and pain reliever", Price = 4.50M, Stock = 150, Manufacturer = "Pharma Inc.", Category = "Pain Relief", DosageForm = "Tablet", Strength = "81mg" },
            new Medicine { Name = "Metformin", Description = "Anti-diabetic medication", Price = 9.99M, Stock = 70, Manufacturer = "DiabeteCare", Category = "Diabetes", DosageForm = "Tablet", Strength = "500mg" },
            new Medicine { Name = "Simvastatin", Description = "Cholesterol-lowering medication", Price = 15.75M, Stock = 60, Manufacturer = "HeartHealth", Category = "Cardiovascular", DosageForm = "Tablet", Strength = "20mg" },
            // Add the rest of your medicines here with additional properties
        };

        // Commands
        public ICommand AddMedicineCommand { get; }
        public ICommand EditMedicineCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand CheckoutCommand { get; }

        public PharmacyPageViewModel()
        {
            _filteredItems = new ObservableCollection<Medicine>(Medicines);
            _cartItems = new ObservableCollection<CartItem>();
            
            // Initialize commands
            AddMedicineCommand = new RelayCommand(_ => AddMedicine());
            EditMedicineCommand = new RelayCommand(_ => EditMedicine(), _ => SelectedMedicine != null);
            AddToCartCommand = new RelayCommand(_ => AddToCart(), _ => SelectedMedicine != null);
            RemoveFromCartCommand = new RelayCommand(param => RemoveFromCart(param as CartItem));
            CheckoutCommand = new RelayCommand(_ => Checkout(), _ => CartItems.Count > 0);
        }

        public ObservableCollection<Medicine> FilteredItems
        {
            get => _filteredItems;
            private set
            {
                _filteredItems = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    FilterMedicines();
                }
            }
        }

        public Medicine SelectedMedicine
        {
            get => _selectedMedicine;
            set
            {
                _selectedMedicine = value;
                OnPropertyChanged();
                QuantityToAdd = 1;
                ((RelayCommand)EditMedicineCommand).RaiseCanExecuteChanged();
                ((RelayCommand)AddToCartCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set
            {
                _cartItems = value;
                OnPropertyChanged();
                ((RelayCommand)CheckoutCommand).RaiseCanExecuteChanged();
            }
        }

        private void FilterMedicines()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                FilteredItems = new ObservableCollection<Medicine>(Medicines);
            }
            else
            {
                var filtered = Medicines
                    .Where(m => m.Name.ToLower().Contains(_searchText.ToLower()))
                    .ToList();

                FilteredItems = new ObservableCollection<Medicine>(filtered);
            }
        }

        private void AddMedicine()
        {
           
        }

        private void EditMedicine()
        {
    
        }

        private void AddToCart()
        {
                if (SelectedMedicine != null && QuantityToAdd > 0 && QuantityToAdd <= SelectedMedicine.Stock)
            {
                var existingItem = CartItems.FirstOrDefault(i => i.Medicine.Name == SelectedMedicine.Name);
                
                if (existingItem != null)
                {
                    existingItem.Quantity += QuantityToAdd;
                    var temp = new ObservableCollection<CartItem>(CartItems);
                    CartItems = temp;
                }
                else
                {
                    CartItems.Add(new CartItem 
                    { 
                        Medicine = SelectedMedicine, 
                        Quantity = QuantityToAdd 
                    });
                }
                
                ((RelayCommand)CheckoutCommand).RaiseCanExecuteChanged();
            }
        }

        private void RemoveFromCart(CartItem item)
        {
            if (item != null)
            {
                CartItems.Remove(item);
                ((RelayCommand)CheckoutCommand).RaiseCanExecuteChanged();
            }
        }

        private void Checkout()
        {
            if (CartItems.Count > 0)
            {
                CartItems.Clear();
                ((RelayCommand)CheckoutCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}