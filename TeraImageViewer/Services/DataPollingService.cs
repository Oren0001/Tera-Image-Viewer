using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TeraImageViewer.Models;

namespace TeraImageViewer.Services {
    public class DataPollingService {
        private const int _defaultPollingIntervalMs = 2000;
        private readonly TeraApiService _apiClient;
        private readonly AuthenticationService _authService;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _pollingTask;
        private string _currentImageId;
        private bool _isPolling;
        private Dictionary<string, int> _dataDict = new Dictionary<string, int>();
        private ObservableCollection<MicroscopeData> _observableList = new ObservableCollection<MicroscopeData>();

        public event EventHandler<string> ErrorOccurred;

        public bool IsPolling => _isPolling;
        public ObservableCollection<MicroscopeData> History => _observableList;

        public DataPollingService(TeraApiService apiClient, AuthenticationService authService) {
            _apiClient = apiClient;
            _authService = authService;

            _authService.AuthenticationStatusChanged += OnAuthenticationStatusChanged;
        }

        public void StartPolling(int intervalMs = _defaultPollingIntervalMs) {
            if (_isPolling) {
                return;
            }

            _isPolling = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _pollingTask = Task.Run(async () => await PollingLoopAsync(intervalMs, _cancellationTokenSource.Token));
        }

        public void StopPolling() {
            if (!_isPolling) {
                return;
            }

            _isPolling = false;
            _cancellationTokenSource?.Cancel();

            try {
                _pollingTask?.Wait(TimeSpan.FromSeconds(5));
            } catch (AggregateException) {
                // Task was cancelled, this is expected
            }

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _pollingTask = null;
        }

        private async Task PollingLoopAsync(int intervalMs, CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                await PollDataAsync();

                try {
                    await Task.Delay(intervalMs, cancellationToken);
                } catch (TaskCanceledException) {
                    // Expected when stopping, exit loop
                    break;
                }
            }
        }

        private void OnAuthenticationStatusChanged(object sender, AuthenticationStatus status) {
            string message = "Status: Connected";

            switch (status) {
                case AuthenticationStatus.Authenticating:
                    message = "Status: Authenticating, please wait...";
                    break;

                case AuthenticationStatus.NotAuthenticated:
                    message = "Status: Session expired, Please login again.";
                    break;
            }

            ErrorOccurred?.Invoke(this, message);
        }

        private async Task PollDataAsync() {
            try {
                if (!_authService.IsAuthenticated) {
                    return;
                }

                var imageTask = _apiClient.GetLatestImageAsync();
                var resultsTask = _apiClient.GetInferenceResultsAsync();

                await Task.WhenAll(imageTask, resultsTask);

                var imageData = imageTask.Result;
                var resultsData = resultsTask.Result;

                if (imageData != null && resultsData != null && imageData.ImageId == resultsData.ImageId) {
                    AddOrUpdateImageData(imageData, resultsData);
                } else {
                    if (imageData != null) {
                        AddOrUpdateImageData(imageData, null);
                    }

                    if (resultsData != null && _dataDict.ContainsKey(resultsData.ImageId)) {
                        UpdateWithResults(resultsData.ImageId, resultsData);
                    }
                }

            } catch (Exception ex) {
                
            }
        }

        private void AddOrUpdateImageData(ImageData imageData, InferenceResults resultsData) {
            string imageId = imageData?.ImageId;

            if (string.IsNullOrEmpty(imageId) || imageId == _currentImageId) {
                return;
            }

            if (_dataDict.ContainsKey(imageId)) {
                int index = _dataDict[imageId];
                var data = _observableList[index];

                data.Timestamp = imageData?.Timestamp ?? data.Timestamp;
                data.ImageDataBase64 = imageData?.ImageDataBase64 ?? data.ImageDataBase64;
                data.IntensityAverage = resultsData?.IntensityAverage ?? data.IntensityAverage;
                data.FocusScore = resultsData?.FocusScore ?? data.FocusScore;
                data.ClassificationLabel = resultsData?.ClassificationLabel ?? data.ClassificationLabel;
                data.Histogram = resultsData?.Histogram ?? data.Histogram;
            } else {
                var data = new MicroscopeData {
                    ImageId = imageId,
                    Timestamp = imageData?.Timestamp,
                    ImageDataBase64 = imageData?.ImageDataBase64,
                    IntensityAverage = resultsData?.IntensityAverage,
                    FocusScore = resultsData?.FocusScore,
                    ClassificationLabel = resultsData?.ClassificationLabel,
                    Histogram = resultsData?.Histogram
                };

                Application.Current.Dispatcher.Invoke(() => {
                    _observableList.Add(data);
                    _dataDict[imageId] = _observableList.Count - 1;
                });
            }

            _currentImageId = imageId;
        }

        private void UpdateWithResults(string imageId, InferenceResults resultsData) {
            if (!_dataDict.ContainsKey(imageId)) {
                return;
            }

            int index = _dataDict[imageId];
            var data = _observableList[index];

            data.IntensityAverage = resultsData.IntensityAverage;
            data.FocusScore = resultsData.FocusScore;
            data.ClassificationLabel = resultsData.ClassificationLabel;
            data.Histogram = resultsData.Histogram;
        }

        public void Reset() {
            _currentImageId = null;
            _dataDict.Clear();
            Application.Current.Dispatcher.Invoke(() => {
                _observableList.Clear();
            });
        }
    }
}
