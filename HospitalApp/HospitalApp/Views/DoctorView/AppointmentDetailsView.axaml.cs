using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using HospitalApp.ViewModels;

namespace HospitalApp.Views;

public partial class AppointmentDetailsView : UserControl
{
    public AppointmentDetailsView()
    {
        InitializeComponent();

        this.AttachedToVisualTree += (_, __) =>
        {
            if (DataContext is AppointmentDetailsViewModel vm && this.GetVisualRoot() is Window window)
            {
                vm.ParentWindow = window;
            }
        };
    }
}
