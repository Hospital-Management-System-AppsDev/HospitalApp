﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HospitalApp.Views.HelperWindows
{
    public partial class AddMedicineView : UserControl
    {
        public AddMedicineView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}