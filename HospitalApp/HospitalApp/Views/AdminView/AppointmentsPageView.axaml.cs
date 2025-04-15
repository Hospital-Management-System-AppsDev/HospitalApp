using Avalonia.Controls;
using HospitalApp.ViewModels;

namespace HospitalApp.Views
{
    public partial class AppointmentsPageView : UserControl
    {
        public AppointmentsPageView()
        {
            InitializeComponent();
            DataContext = new AppointmentsPageViewModel(); 
        }
    }
}
