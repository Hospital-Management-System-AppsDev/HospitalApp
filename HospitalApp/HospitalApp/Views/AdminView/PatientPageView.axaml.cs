using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HospitalApp.Views{
    public partial class PatientPageView : UserControl
    {
        public PatientPageView()
        {
            InitializeComponent();
        }
         private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}