using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media; // ✅ Fix: Add missing Brushes reference
using HospitalApp.ViewModels;
using HospitalApp.Views;

namespace HospitalApp
{
    public class ViewLocator : IDataTemplate
    {
        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;

        public ViewLocator()
        {
            _apiService = new ApiService();
            _signalRService = new SignalRService();
        }

        public Control Build(object param)
        {
            var name = param.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name) ?? Type.GetType(name + ", " + Assembly.GetExecutingAssembly().FullName);

            if (type != null)
            {
                var instance = Activator.CreateInstance(type);
                if (instance is Control control)
                {
                    control.DataContext = param; // ✅ Bind ViewModel to View
                    return control;
                }

                return new TextBlock { Text = $"Failed to create view: {name}" };
            }

            Console.WriteLine($"ViewLocator: View not found for {name}");
            return new TextBlock { Text = $"View Not Found: {name}", Foreground = Brushes.Red };
        }


        public bool Match(object data) => data is ViewModelBase;
    }
}
