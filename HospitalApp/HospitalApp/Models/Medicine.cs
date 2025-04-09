namespace HospitalApp.Models
{
    public class Medicine
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public string DosageForm { get; set; } // tablet, capsule, liquid, etc.
        public string Strength { get; set; } // 500mg, 10ml, etc.
    }

    public class CartItem
    {
        public Medicine Medicine { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Medicine.Price * Quantity;

        public string Name => Medicine.Name;
    }
}