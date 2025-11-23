using System;
using System.ComponentModel;

namespace TeraImageViewer.Models {
    public class MicroscopeData : INotifyPropertyChanged {
        private string _imageId;
        private string _timestamp;
        private string _imageDataBase64;
        private double? _intensityAverage;
        private double? _focusScore;
        private string _classificationLabel;
        private int[] _histogram;
        private DateTime _receivedAt;

        public string ImageId {
            get => _imageId;
            set {
                if (_imageId != value) {
                    _imageId = value;
                    OnPropertyChanged(nameof(ImageId));
                }
            }
        }

        public string Timestamp {
            get => _timestamp;
            set {
                if (_timestamp != value) {
                    _timestamp = value;
                    OnPropertyChanged(nameof(Timestamp));
                }
            }
        }

        public string ImageDataBase64 {
            get => _imageDataBase64;
            set {
                if (_imageDataBase64 != value) {
                    _imageDataBase64 = value;
                    OnPropertyChanged(nameof(ImageDataBase64));
                }
            }
        }

        public double? IntensityAverage {
            get => _intensityAverage;
            set {
                if (_intensityAverage != value) {
                    _intensityAverage = value;
                    OnPropertyChanged(nameof(IntensityAverage));
                }
            }
        }

        public double? FocusScore {
            get => _focusScore;
            set {
                if (_focusScore != value) {
                    _focusScore = value;
                    OnPropertyChanged(nameof(FocusScore));
                }
            }
        }

        public string ClassificationLabel {
            get => _classificationLabel;
            set {
                if (_classificationLabel != value) {
                    _classificationLabel = value;
                    OnPropertyChanged(nameof(ClassificationLabel));
                }
            }
        }

        public int[] Histogram {
            get => _histogram;
            set {
                if (_histogram != value) {
                    _histogram = value;
                    OnPropertyChanged(nameof(Histogram));
                }
            }
        }

        public DateTime ReceivedAt {
            get => _receivedAt;
            set {
                if (_receivedAt != value) {
                    _receivedAt = value;
                    OnPropertyChanged(nameof(ReceivedAt));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MicroscopeData() {
            ReceivedAt = DateTime.Now;
        }
    }
}
