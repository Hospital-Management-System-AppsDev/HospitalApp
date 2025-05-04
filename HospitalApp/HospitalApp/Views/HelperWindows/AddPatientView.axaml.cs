using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HospitalApp.Views.HelperWindows
{
    public partial class AddPatientView : UserControl
    {
        public AddPatientView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}