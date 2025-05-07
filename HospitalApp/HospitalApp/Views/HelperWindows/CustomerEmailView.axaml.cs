using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HospitalApp.Views.HelperWindows
{
    public partial class CustomerEmailView : Window
    {
        public CustomerEmailView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}