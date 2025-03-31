using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HospitalApp.ViewModels;

namespace HospitalApp.Views
{
    public partial class DashboardPageView : UserControl
    {
        public DashboardPageView() { InitializeComponent(); }

        // Constructor with services for manual dependency injection
        public DashboardPageView(ApiService apiService, SignalRService signalRService)
        {
            InitializeComponent();
            DataContext = new DashboardPageViewModel(apiService, signalRService);
        }
    }
}
