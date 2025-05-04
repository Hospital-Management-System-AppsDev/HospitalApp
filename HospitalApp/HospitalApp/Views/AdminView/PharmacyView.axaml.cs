using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
<<<<<<< HEAD
using System.Text.RegularExpressions;
using System;
=======
using System.Diagnostics;
using System;
using System.Linq;
using Avalonia.VisualTree;
>>>>>>> 1236877854defc891fee64cb03163c541b675ffe

namespace HospitalApp.Views
{
    public partial class PharmacyView : UserControl
    {
        public PharmacyView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
        }
    }
}