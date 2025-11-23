using System.Windows;
using TeraImageViewer.ViewModels;

namespace TeraImageViewer.Views {
    public partial class LoginWindow : Window {
        private readonly LoginViewModel _viewModel;

        public LoginWindow(LoginViewModel viewModel) {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.LoginSuccessful += OnLoginSuccessful;
            PasswordBox.Password = _viewModel.Password;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            _viewModel.Password = PasswordBox.Password;
        }

        private void OnLoginSuccessful(object sender, System.EventArgs e) {
            DialogResult = true;
            Close();
        }
    }
}
