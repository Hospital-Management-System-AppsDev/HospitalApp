using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HospitalApp.ViewModels;

namespace HospitalApp.Views;

public partial class PharmacyPageView : UserControl
{
    public PharmacyPageView()
    {
        InitializeComponent();
        DataContext = new PharmacyPageViewModel();

    }
}