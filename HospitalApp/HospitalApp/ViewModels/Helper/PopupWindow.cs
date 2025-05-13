using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace HospitalApp.ViewModels
{
    public class PopupWindow : Window
    {
        private readonly TaskCompletionSource<bool> _resultTcs = new TaskCompletionSource<bool>();

        /// <summary>
        /// Creates a new popup window with customizable title, message, and button labels
        /// </summary>
        /// <param name="title">Window title</param>
        /// <param name="message">The message to display to the user</param>
        /// <param name="confirmButtonText">Text for confirm button (default: "OK")</param>
        /// <param name="cancelButtonText">Text for cancel button (default: "Cancel")</param>
        public PopupWindow(string title, string message, string confirmButtonText = "OK", string cancelButtonText = "Cancel")
        {
            Title = title;
            Width = 400;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            CanResize = false;

            // Create the layout
            var mainPanel = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 20
            };

            // Message text
            var messageTextBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            mainPanel.Children.Add(messageTextBlock);

            // Buttons panel
            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Spacing = 10
            };

            // Confirm button
            var confirmButton = new Button
            {
                Content = confirmButtonText,
                Width = 100,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            confirmButton.Click += (sender, args) =>
            {
                _resultTcs.SetResult(true);
                Close();
            };

            buttonsPanel.Children.Add(confirmButton);

            // Add Cancel button only if cancelButtonText is not empty or whitespace
            if (!string.IsNullOrWhiteSpace(cancelButtonText))
            {
                var cancelButton = new Button
                {
                    Content = cancelButtonText,
                    Width = 100,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                cancelButton.Click += (sender, args) =>
                {
                    _resultTcs.SetResult(false);
                    Close();
                };

                buttonsPanel.Children.Add(cancelButton);
            }

            mainPanel.Children.Add(buttonsPanel);

            Content = mainPanel;
        }

        /// <summary>
        /// Shows the popup window and returns the result
        /// </summary>
        /// <param name="owner">The owner window</param>
        /// <returns>True if confirmed, False if canceled</returns>
        public async Task<bool> ShowDialog(Window owner)
        {
            base.ShowDialog(owner);
            return await _resultTcs.Task;
        }

        /// <summary>
        /// Static helper method to quickly show a confirmation dialog
        /// </summary>
        /// <param name="owner">Owner window</param>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="confirmButtonText">Text for the confirm button</param>
        /// <param name="cancelButtonText">Text for the cancel button</param>
        /// <returns>True if confirmed, False if canceled</returns>
        public static async Task<bool> ShowConfirmation(
            Window owner,
            string title,
            string message,
            string confirmButtonText = "OK",
            string cancelButtonText = "Cancel")
        {
            var dialog = new PopupWindow(title, message, confirmButtonText, cancelButtonText);
            return await dialog.ShowDialog(owner);
        }
    }
}
