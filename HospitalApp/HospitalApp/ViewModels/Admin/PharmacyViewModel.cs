using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HospitalApp.Models;
using Avalonia.Controls;
using HospitalApp.Views.HelperWindows;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Input;
using System.Text.RegularExpressions;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia;

namespace HospitalApp.ViewModels
{
    public partial class PharmacyViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;
        private Window _parentWindow;

        [ObservableProperty]
        private ObservableCollection<Medicines> medicines = new();

        [ObservableProperty]
        private ObservableCollection<Medicines> filteredMedicines = new();
        [ObservableProperty]
        private Medicines selectedMedicine;
        [ObservableProperty]
        private CartItems selectedCartItem;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private int quantityToAdd = 0;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private ObservableCollection<CartItems> cartItems = new();

        partial void OnCartItemsChanged(ObservableCollection<CartItems> value)
        {
            if (value != null)
            {
                value.CollectionChanged += (s, e) =>
                {
                    UpdateTotalPrice();
                };
            }
        }

        private void UpdateTotalPrice()
        {
            TotalCartPrice = CartItems.Sum(item => item.TotalPrice);
        }

        [ObservableProperty]
        private decimal totalCartPrice = 0;

        [ObservableProperty]
        private string errorMessage;

        public PharmacyViewModel(ApiService apiService, SignalRService signalRService)
        {
            _apiService = apiService;
            _signalRService = signalRService;
            InitializeAsync();
        }

        public async void InitializeAsync()
        {
            await LoadMedicines();
            FilterMedicines();
        }

        public async Task LoadMedicines()
        {
            var medicines = await _apiService.GetPharmacy();
            Medicines.Clear();
            Medicines = new ObservableCollection<Medicines>(medicines);
        }

        public void FilterMedicines()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredMedicines = new ObservableCollection<Medicines>(Medicines);
            }
            else
            {
                var filteredList = Medicines
                    .Where(m => m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                FilteredMedicines = new ObservableCollection<Medicines>(filteredList);
            }
        }

        [RelayCommand]
        public async Task AddMedicine()
        {
            try
            {
                Window parentWindow = null;
                if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    parentWindow = desktop.MainWindow;
                }

                if (parentWindow == null)
                {
                    ErrorMessage = "Error: Parent window not available";
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

                var result = await dialog.ShowDialog<Medicines>(parentWindow);

                if (result != null)
                {
                    var addedMedicine = await _apiService.AddMedicine(result);
                    if (addedMedicine != null)
                    {
                        await LoadMedicines();
                        FilterMedicines();
                        ErrorMessage = string.Empty;
                    }
                    else
                    {
                        ErrorMessage = "Failed to add medicine. Please try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding medicine: {ex.Message}";
            }
        }

        [RelayCommand]
        public void EditMedicine()
        {
            IsEditing = !IsEditing;
        }

        [RelayCommand]
        public void AddToCart()
        {
            if (SelectedMedicine != null && QuantityToAdd > 0 && SelectedMedicine.Stocks >= QuantityToAdd)
            {
                var existingItem = CartItems.FirstOrDefault(i => i.Medicine.Id == SelectedMedicine.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity += QuantityToAdd;
                }
                else
                {
                    CartItems.Add(new CartItems
                    {
                        Medicine = SelectedMedicine,
                        Quantity = QuantityToAdd
                    });
                }

                SelectedMedicine.Stocks -= QuantityToAdd;
                TotalCartPrice += SelectedMedicine.Price * QuantityToAdd;
                ErrorMessage = string.Empty;
            }
            else if (SelectedMedicine != null && QuantityToAdd > SelectedMedicine.Stocks)
            {
                ErrorMessage = $"Error: Quantity ({QuantityToAdd}) exceeds available stock ({SelectedMedicine.Stocks})";
            }
        }

        [RelayCommand]
        public void SaveEdit()
        {
            UpdateMedicine();
            IsEditing = false;
        }

        private async Task ShowMessageDialog(string title, string message)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _parentWindow = desktop.MainWindow;
            }

            var button = new Button
            {
                Content = "OK",
                Width = 80,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var messageWindow = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Content = new StackPanel
                {
                    Margin = new Thickness(20),
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = message,
                            TextWrapping = TextWrapping.Wrap,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            FontSize = 16,
                        },
                        button
                    }
                }
            };

            button.Command = new RelayCommand<object>(_ =>
            {
                messageWindow.Close();
            });

            await messageWindow.ShowDialog(_parentWindow);
        }

        [RelayCommand]
        public async Task DeleteMedicine()
        {
            if (SelectedMedicine == null)
            {
                ErrorMessage = "Please select a medicine to delete";
                return;
            }

            try
            {
                var result = await ShowConfirmationDialog("Delete Medicine", "Are you sure you want to delete this medicine?");
                if (!result)
                {
                    return;
                }

                var success = await _apiService.DeleteMedicine(SelectedMedicine.Id);
                if (success)
                {
                    await LoadMedicines();
                    FilterMedicines();
                    ErrorMessage = string.Empty;
                    IsEditing = false;
                    await ShowMessageDialog("Success", "Medicine successfully deleted!");
                }
                else
                {
                    await ShowMessageDialog("Error", "Failed to delete medicine. Please try again.");
                }
            }
            catch (Exception ex)
            {
                await ShowMessageDialog("Error", $"Error deleting medicine: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task UpdateMedicine()
        {
            if (SelectedMedicine == null)
            {
                ErrorMessage = "Please select a medicine to update";
                return;
            }

            try
            {
                Console.WriteLine("Updating medicine");
                Console.WriteLine(SelectedMedicine.Name);
                Console.WriteLine(SelectedMedicine.Price);
                Console.WriteLine(SelectedMedicine.Stocks);
                Console.WriteLine(SelectedMedicine.Manufacturer);
                Console.WriteLine(SelectedMedicine.Type);
                Console.WriteLine(SelectedMedicine.Dosage);
                Console.WriteLine(SelectedMedicine.Unit);
                var success = await _apiService.UpdateMedicine(SelectedMedicine);
                if (success)
                {
                    await LoadMedicines();
                    FilterMedicines();
                    ErrorMessage = string.Empty;
                    IsEditing = false;
                    await ShowMessageDialog("Success", "Medicine successfully updated!");
                }
                else
                {
                    await ShowMessageDialog("Error", "Failed to update medicine. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating medicine: {ex.Message}";
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterMedicines();
        }

        [RelayCommand]
        public async Task Checkout()
        {
            if (CartItems.Count == 0)
            {
                await ShowMessageDialog("Error", "Cart is empty");
                return;
            }

            try
            {
                foreach (var item in CartItems)
                {
                    var success = await _apiService.UpdateMedicine(item.Medicine);
                    if (!success)
                    {
                        await ShowMessageDialog("Error", $"Failed to update stock for {item.Medicine.Name}");
                        return;
                    }
                }

                // Convert ObservableCollection to List for PDF generation
                var cartItemsList = CartItems.ToList();
                PdfServices.GeneratePharmacyReceipt(cartItemsList, TotalCartPrice, "Pharmacy Receipt");

                CartItems.Clear();
                TotalCartPrice = 0;
                await LoadMedicines();
                FilterMedicines();
                await ShowMessageDialog("Success", "Checkout completed successfully!");
            }
            catch (Exception ex)
            {
                await ShowMessageDialog("Error", $"Error during checkout: {ex.Message}");
            }
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // Allow control keys (backspace, delete, etc.)
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right)
                return;

            // Allow decimal point only if there isn't already one in the text
            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                if (sender is TextBox textBox && textBox.Text.Contains('.'))
                {
                    e.Handled = true;
                }
                return;
            }

            // Only allow digits
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                e.Handled = true;
            }
        }

        [RelayCommand]
        public void RemoveFromCart()
        {
            if (SelectedCartItem != null)
            {
                // Find the medicine in the Medicines collection
                var medicine = Medicines.FirstOrDefault(m => m.Id == SelectedCartItem.Medicine.Id);
                if (medicine != null)
                {
                    medicine.Stocks += SelectedCartItem.Quantity;
                }

                TotalCartPrice -= SelectedCartItem.TotalPrice;
                CartItems.Remove(SelectedCartItem);
            }
        }

        private Task<bool> ShowConfirmationDialog(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>();

            Window confirmationWindow = null;

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _parentWindow = desktop.MainWindow;
            }

            confirmationWindow = new Window
            {
                Title = title,
                Width = 350,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Content = new StackPanel
                {
                    Margin = new Thickness(20),
                    Spacing = 15,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = message,
                            TextWrapping = TextWrapping.Wrap,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            FontSize = 16,
                        },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 20,
                            Children =
                            {
                                new Button
                                {
                                    Content = "Yes",
                                    Width = 80,
                                    Command = new RelayCommand<object>(_ =>
                                    {
                                        tcs.SetResult(true);
                                        confirmationWindow.Close();
                                    })
                                },
                                new Button
                                {
                                    Content = "No",
                                    Width = 80,
                                    Command = new RelayCommand<object>(_ =>
                                    {
                                        tcs.SetResult(false);
                                        confirmationWindow.Close();
                                    })
                                }
                            }
                        }
                    }
                }
            };

            confirmationWindow.ShowDialog(_parentWindow);

            return tcs.Task;
        }
    }
}
