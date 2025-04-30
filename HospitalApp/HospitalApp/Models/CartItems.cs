using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;


namespace HospitalApp.Models
{
    public partial class CartItems : ObservableObject
    {
        [ObservableProperty]
        private Medicines _medicine;

        [ObservableProperty]
        private int _quantity;

        [ObservableProperty]
        private decimal _totalPrice;

        partial void OnQuantityChanged(int value)
        {
            UpdateTotalPrice();
        }

        partial void OnMedicineChanged(Medicines value)
        {
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            TotalPrice = Medicine?.Price * Quantity ?? 0;
        }
    }
}