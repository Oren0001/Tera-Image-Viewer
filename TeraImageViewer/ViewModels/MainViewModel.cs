using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TeraImageViewer.Models;
using TeraImageViewer.Services;

namespace TeraImageViewer.ViewModels {
    public class MainViewModel : ViewModelBase {
        private readonly DataPollingService _pollingService;
        private readonly AuthenticationService _authService;
        private MicroscopeData _selectedData;
        private string _errorMessage;

        public ObservableCollection<MicroscopeData> History => _pollingService.History;

        public MicroscopeData SelectedData {
            get => _selectedData;
            set => SetProperty(ref _selectedData, value);
        }

        public string ErrorMessage {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LogoutCommand { get; }

        public event EventHandler LogoutRequested;

        public MainViewModel(DataPollingService pollingService, AuthenticationService authService) {
            _pollingService = pollingService;
            _authService = authService;

            LogoutCommand = new RelayCommand(_ => Logout());

            _pollingService.StartPolling();
            _pollingService.History.CollectionChanged += OnHistoryChanged;
            _pollingService.ErrorOccurred += OnErrorOccurred;
        }

        public void StartPolling() {
            _pollingService.StartPolling();
        }

        public void StopPolling() {
            _pollingService.StopPolling();
        }

        private void OnHistoryChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            Application.Current.Dispatcher.Invoke(() => {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null) {
                    if (SelectedData == null && e.NewItems.Count > 0) {
                        SelectedData = (MicroscopeData)e.NewItems[0];
                    }
                }
            });
        }

        private void OnErrorOccurred(object sender, string error) {
            Application.Current.Dispatcher.Invoke(() => {
                ErrorMessage = error;
            });
        }

        private void Logout() {
            StopPolling();
            _pollingService.Reset();
            _authService.Logout();
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Cleanup() {
            StopPolling();
            _pollingService.History.CollectionChanged -= OnHistoryChanged;
            _pollingService.ErrorOccurred -= OnErrorOccurred;
        }
    }
}
