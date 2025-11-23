using System.Windows;
using TeraImageViewer.ViewModels;

namespace TeraImageViewer.Views {
    public partial class MainWindow : Window {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel) {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.LogoutRequested += OnLogoutRequested;

            Closing += (s, e) => _viewModel.Cleanup();
        }

        private void OnLogoutRequested(object sender, System.EventArgs e) {
            _viewModel.Cleanup();

            Close();

            var app = (App)Application.Current;
            app.ShowLoginWindow();
        }
    }
}
