﻿using NTMiner.MinerServer;
using System;
using System.Windows;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class ServerMessageViewModel : ViewModelBase, IServerMessage {
        private static readonly StreamGeometry InfoIcon = (StreamGeometry)Application.Current.Resources["Icon_Info"];
        private static readonly SolidColorBrush InfoColor = (SolidColorBrush)Application.Current.Resources["InfoColor"];
        private static readonly StreamGeometry NewVersionIcon = (StreamGeometry)Application.Current.Resources["Icon_NewVersion"];
        private static readonly SolidColorBrush NewVersionColor = (SolidColorBrush)Application.Current.Resources["NewVersionColor"];

        public static StreamGeometry GetIcon(ServerMessageType messageType) {
            switch (messageType) {
                case ServerMessageType.Info:
                    return InfoIcon;
                case ServerMessageType.NewVersion:
                    return NewVersionIcon;
                default:
                    return null;
            }
        }

        public static SolidColorBrush GetIconFill(ServerMessageType messageType) {
            switch (messageType) {
                case ServerMessageType.Info:
                    return InfoColor;
                case ServerMessageType.NewVersion:
                    return NewVersionColor;
                default:
                    return WpfUtil.BlackBrush;
            }
        }

        private Guid _id;
        private string _provider;
        private string _messageType;
        private string _content;
        private DateTime _timestamp;
        private readonly ServerMessageType _messageTypeEnum;

        public ServerMessageViewModel(IServerMessage data) {
            _id = data.Id;
            _provider = data.Provider;
            _messageType = data.MessageType;
            _content = data.Content;
            _timestamp = data.Timestamp;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Provider {
            get => _provider;
            set {
                _provider = value;
                OnPropertyChanged(nameof(Provider));
            }
        }

        public string MessageType {
            get => _messageType;
            set {
                _messageType = value;
                OnPropertyChanged(nameof(MessageType));
            }
        }

        public string Content {
            get => _content;
            set {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public string MessageTypeText {
            get {
                return _messageTypeEnum.GetDescription();
            }
        }

        public StreamGeometry MessageTypeIcon {
            get {
                return GetIcon(_messageTypeEnum);
            }
        }

        public SolidColorBrush IconFill {
            get {
                return GetIconFill(_messageTypeEnum);
            }
        }

        public DateTime Timestamp {
            get => _timestamp;
            set {
                _timestamp = value;
                OnPropertyChanged(nameof(Timestamp));
            }
        }

        public string TimestampText {
            get {
                int offDay = (DateTime.Now.Date - Timestamp.Date).Days;
                switch (offDay) {
                    case 0:
                        return $"今天 {Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 1:
                        return $"昨天 {Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 2:
                        return $"前天 {Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    default:
                        return Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }
    }
}
