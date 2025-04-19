using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HospitalApp.Views;

public partial class SettingsPageView : UserControl
{
    public SettingsPageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}