using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HospitalApp.ViewModels;

namespace HospitalApp.Views
{
    public partial class MainMenuControl : UserControl
    {
        public MainMenuControl()
        {
            InitializeComponent();
            DataContext = new MainMenuViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}