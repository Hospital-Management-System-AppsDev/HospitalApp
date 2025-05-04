using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HospitalApp.Views.HelperWindows
{
    public partial class AddDoctorView : UserControl
    {
        public AddDoctorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}