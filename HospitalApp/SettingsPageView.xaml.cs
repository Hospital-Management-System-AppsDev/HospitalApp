// In your code-behind file (SettingsPageView.xaml.cs)
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HospitalApp.Views
{
    public partial class SettingsPageView : UserControl
    {
        public SettingsPageView()
        {
            InitializeComponent();
            
            // Hook up the General button click event
            GeneralButton.Click += OnGeneralButtonClick;
        }
        
        private void OnGeneralButtonClick(object sender, RoutedEventArgs e)
        {
            // Get the main content control (you'll need to add an x:Name to your content area)
            ContentControl mainContent = this.FindControl<ContentControl>("MainContent");
            
            // Create a new instance of the patient profile view
            PatientProfileView patientView = new PatientProfileView();
            
            // Set the content to the patient profile view
            mainContent.Content = patientView;
        }
    }
}// In your code-behind file (SettingsPageView.xaml.cs)
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HospitalApp.Views
{
    public partial class SettingsPageView : UserControl
    {
        public SettingsPageView()
        {
            InitializeComponent();
            
            // Hook up the General button click event
            GeneralButton.Click += OnGeneralButtonClick;
        }
        
        private void OnGeneralButtonClick(object sender, RoutedEventArgs e)
        {
            // Get the main content control (you'll need to add an x:Name to your content area)
            ContentControl mainContent = this.FindControl<ContentControl>("MainContent");
            
            // Create a new instance of the patient profile view
            PatientProfileView patientView = new PatientProfileView();
            
            // Set the content to the patient profile view
            mainContent.Content = patientView;
        }
    }
}
