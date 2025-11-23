using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TeraImageViewer.Services;

namespace TeraImageViewer.ViewModels {
    public class LoginViewModel : ViewModelBase {
        private readonly AuthenticationService _authService;
        private string _username;
        private string _password;
        private string _statusMessage;
        private bool _isLoggingIn;

        public string Username {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string StatusMessage {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsLoggingIn {
            get => _isLoggingIn;
            set => SetProperty(ref _isLoggingIn, value);
        }

        public ICommand LoginCommand { get; }

        public event EventHandler LoginSuccessful;

        public LoginViewModel(AuthenticationService authService) {
            _authService = authService;
            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => !IsLoggingIn);
        }

        private async Task LoginAsync() {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) {
                StatusMessage = "Please enter username and password";
                return;
            }

            IsLoggingIn = true;
            StatusMessage = "Logging in...";

            try {
                bool success = await _authService.LoginAsync(Username, Password);

                if (success) {
                    StatusMessage = "Login successful!";
                    LoginSuccessful?.Invoke(this, EventArgs.Empty);
                } else {
                    StatusMessage = "Login failed. Please check your credentials or try later.";
                }
            } catch (Exception ex) {
                StatusMessage = $"Error: {ex.Message}";
            } finally {
                IsLoggingIn = false;
            }
        }
    }
}
