using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HospitalApp.ViewModels;

namespace HospitalApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            // No need to set DataContext here as it will be set in the UserControl
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}