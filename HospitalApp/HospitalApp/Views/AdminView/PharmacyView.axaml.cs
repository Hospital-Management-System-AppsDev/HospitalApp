using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using System.Text.RegularExpressions;

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