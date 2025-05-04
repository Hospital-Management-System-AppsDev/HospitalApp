using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HospitalApp.Models
{
    public partial class Medicines : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [ObservableProperty]
        private decimal _price;
        
        // This needs to be 'partial void' not 'public void'
        // partial void OnPriceChanging(decimal value)
        // {
        //     // This gets called before the property changes
        //     _price = value is decimal d ? d : Convert.ToDecimal(value);
        // }
        
        [ObservableProperty]
        private int _stocks;
        
        // partial void OnStocksChanging(int value)
        // {
        //     // This gets called before the property changes
        //     _stocks = value is int i ? i : Convert.ToInt32(value);
        // }
        
        public string Manufacturer { get; set; }
        public string Type { get; set; }
        
        [ObservableProperty]
        private decimal _dosage;
        
        partial void OnDosageChanging(decimal value)
        {
            // This gets called before the property changes
            _dosage = value is decimal d ? d : Convert.ToDecimal(value);
        }
        
        public string Unit { get; set; }
    }
}