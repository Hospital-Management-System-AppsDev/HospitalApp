using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using HospitalApp.ViewModels.Helper;

namespace HospitalApp.ViewModels
{
    public partial class CustomerEmailViewModel : ObservableObject
    {
        private string _customerEmail;
        private bool _hasErrors;
        private string _errorMessage;

        public string CustomerEmail
        {
            get => _customerEmail;
            set
            {
                SetProperty(ref _customerEmail, value);
                ValidateEmail();
                SubmitCommand.NotifyCanExecuteChanged();
            }
        }

        public bool HasErrors
        {
            get => _hasErrors;
            private set => SetProperty(ref _hasErrors, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public IRelayCommand SubmitCommand { get; }
        public IRelayCommand CancelCommand { get; }

        private Window _emailWindow;

        public CustomerEmailViewModel()
        {
            SubmitCommand = new RelayCommand(Submit, CanSubmit);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void SetWindow(Window window)
        {
            _emailWindow = window;
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(CustomerEmail))
            {
                HasErrors = true;
                ErrorMessage = "Email is required.";
                return;
            }
            
            if (!IsValidEmail(CustomerEmail))
            {
                HasErrors = true;
                ErrorMessage = "Invalid email address.";
                return;
            }

            HasErrors = false;
            ErrorMessage = string.Empty;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                return false;
            }
        }

        private bool CanSubmit()
        {
            return !HasErrors && !string.IsNullOrWhiteSpace(CustomerEmail);
        }

        private async void Submit()
        {
            if (string.IsNullOrWhiteSpace(CustomerEmail))
            {
                // Show error popup if field is empty
                await PopupWindow.ShowConfirmation(
                    _emailWindow,
                    "Validation Error",
                    "Email is required. Please enter a valid email address.",
                    "OK",
                    null); // Pass null to hide the cancel button
                return;
            }

            if (!IsValidEmail(CustomerEmail))
            {
                await PopupWindow.ShowConfirmation(
                    _emailWindow,
                    "Validation Error",
                    "Please enter a valid email address.",
                    "OK",
                    null);
                return;
            }

            // Show confirmation popup
            bool result = await PopupWindow.ShowConfirmation(
                _emailWindow,
                "Confirm Submission",
                $"Are you sure you want to submit the email: {CustomerEmail}?",
                "Submit",
                "Cancel");

            if (result)
            {
                // User confirmed, return the email
                _emailWindow?.Close(CustomerEmail);
            }
            // If result is false, do nothing and let the user continue editing
        }

        private void Cancel()
        {
            _emailWindow?.Close(null);
        }
    }
}