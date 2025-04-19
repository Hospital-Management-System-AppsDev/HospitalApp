using System;
using Avalonia;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HospitalApp.Models;
using Avalonia.Controls;
using HospitalApp.Views.HelperWindows;
using Avalonia.Controls.ApplicationLifetimes;

namespace HospitalApp.ViewModels
{
    public class PharmacyPageViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private ObservableCollection<Medicine> _filteredItems;
        private Medicine _selectedMedicine;
        private ObservableCollection<CartItem> _cartItems;
        private bool _isEditing; // New property to track editing state

        private string _quantityToAdd = "1";
        public string QuantityToAdd
        {
            get => _quantityToAdd;
            set
            {
                _quantityToAdd = value;
                OnPropertyChanged(nameof(QuantityToAdd));
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Medicine> Medicines { get; } = new()
        {
            new Medicine { Name = "Paracetamol", Description = "Common pain reliever and fever reducer", Price = "5.99", Stock = "100", Manufacturer = "Pharma Inc.", Category = "Pain Relief", DosageForm = "Tablet", Strength = "500mg" },
            new Medicine { Name = "Loperamide", Description = "Anti-diarrheal medication", Price = "7.50", Stock = "75", Manufacturer = "MediCorp", Category = "Gastrointestinal", DosageForm = "Capsule", Strength = "2mg" },
            new Medicine { Name = "Ibuprofen", Description = "Non-steroidal anti-inflammatory drug", Price = "6.75", Stock = "120", Manufacturer = "Pharma Inc.", Category = "Pain Relief", DosageForm = "Tablet", Strength = "400mg" },
            new Medicine { Name = "Amoxicillin", Description = "Antibiotic medication", Price = "12.99", Stock = "50", Manufacturer = "HealthPharm", Category = "Antibiotics", DosageForm = "Capsule", Strength = "500mg" },
            new Medicine { Name = "Cetirizine", Description = "Antihistamine for allergy relief", Price = "8.25", Stock = "85", Manufacturer = "AllergyRx", Category = "Allergy", DosageForm = "Tablet", Strength = "10mg" },
            new Medicine { Name = "Aspirin", Description = "Blood thinner and pain reliever", Price = "4.50", Stock = "150", Manufacturer = "Pharma Inc.", Category = "Pain Relief", DosageForm = "Tablet", Strength = "81mg" },
            new Medicine { Name = "Metformin", Description = "Anti-diabetic medication", Price = "9.99", Stock = "70", Manufacturer = "DiabeteCare", Category = "Diabetes", DosageForm = "Tablet", Strength = "500mg" },
            new Medicine { Name = "Simvastatin", Description = "Cholesterol-lowering medication", Price = "15.75", Stock = "60", Manufacturer = "HeartHealth", Category = "Cardiovascular", DosageForm = "Tablet", Strength = "20mg" },
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
            _isEditing = false; // TextBox fields disabled by default

            _cartItems.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TotalCartPrice));
                ((RelayCommand)CheckoutCommand).RaiseCanExecuteChanged();
            };

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
                QuantityToAdd = "1";
                IsEditing = false; // Reset editing state when selecting a new medicine
                ((RelayCommand)EditMedicineCommand).RaiseCanExecuteChanged();
                ((RelayCommand)AddToCartCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set
            {
                _cartItems.CollectionChanged -= (s, e) =>
                {
                    OnPropertyChanged(nameof(TotalCartPrice));
                    ((RelayCommand)CheckoutCommand).RaiseCanExecuteChanged();
                };

                _cartItems = value;

                if (_cartItems != null)
                {
                    _cartItems.CollectionChanged += (s, e) =>
                    {
                        OnPropertyChanged(nameof(TotalCartPrice));
                        ((RelayCommand)CheckoutCommand).RaiseCanExecuteChanged();
                    };
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalCartPrice));
                ((RelayCommand)CheckoutCommand).RaiseCanExecuteChanged();
            }
        }

        public decimal TotalCartPrice => CartItems.Sum(item => item.TotalPrice);

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

        private async void AddMedicine()
        {
            Window parentWindow = null;
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                parentWindow = desktop.MainWindow;
            }

            if (parentWindow == null)
            {
                Console.WriteLine("Error: Parent window not available");
                return;
            }
    
            var addMedicineViewModel = new AddMedicineViewModel();
    
            var dialog = new Window
            {
                Title = "Add New Medicine",
                Content = new AddMedicineView { DataContext = addMedicineViewModel },
                Width = 400,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
    
            addMedicineViewModel.SetWindow(dialog);
    
            var result = await dialog.ShowDialog<Medicine>(parentWindow);
    
            if (result != null)
            {
                Medicines.Add(result);
                FilterMedicines(); 
            }
        }

        private void EditMedicine()
        {
            if (SelectedMedicine != null)
            {
                IsEditing = !IsEditing; // Toggle editing state
            }
        }

        private void AddToCart()
        {
            if (SelectedMedicine != null && 
                int.TryParse(QuantityToAdd, out int quantity) && 
                quantity > 0)
            {
                // Validate and parse Stock
                if (!int.TryParse(SelectedMedicine.Stock, out int stock) || stock < quantity)
                {
                    // Optionally notify user of invalid stock
                    return;
                }

                // Validate and parse Price
                if (!decimal.TryParse(SelectedMedicine.Price, out decimal price) || price < 0)
                {
                    // Optionally notify user of invalid price
                    return;
                }

                var existingItem = CartItems.FirstOrDefault(i => i.Medicine.Name == SelectedMedicine.Name);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    CartItems.Add(new CartItem
                    {
                        Medicine = SelectedMedicine,
                        Quantity = quantity
                    });
                }

                // Update stock as string
                SelectedMedicine.Stock = (stock - quantity).ToString();
                // Notify changes
                OnPropertyChanged(nameof(CartItems));
                OnPropertyChanged(nameof(TotalCartPrice));
                ((RelayCommand)CheckoutCommand).RaiseCanExecuteChanged();

                QuantityToAdd = "1";
            }
        }

        private void RemoveFromCart(CartItem item)
        {
            if (item != null)
            {
                CartItems.Remove(item);
            }
        }

        private void Checkout()
        {
            if (CartItems.Count > 0)
            {
                CartItems.Clear();
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