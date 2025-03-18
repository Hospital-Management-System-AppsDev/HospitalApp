using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HospitalApp.ViewModels;

namespace HospitalApp.Views
{
    public partial class DashboardPageView : UserControl
    {
        public DashboardPageView()
        {
            InitializeComponent();
            DataContext = new DashboardPageViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}