using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HospitalApp.Views{
    public partial class AdminDoctorPageView : UserControl
    {
        public AdminDoctorPageView()
        {
            InitializeComponent();
        }
         private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}