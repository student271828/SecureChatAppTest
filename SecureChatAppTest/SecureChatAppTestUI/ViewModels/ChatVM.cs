using SecureChatAppTest.Components.Network;
using SecureChatAppTest.Encryption;
using SecureChatAppTest.Network.Network.TCP;
using SecureChatAppTestUI.Models;
using SecureChatAppTestUI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SecureChatAppTestUI.ViewModels
{
    public class ChatVM : INotifyPropertyChanged, IDisposable
    {
        public event EventHandler IsClosingChanged;

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dispatcher _dispatcher;
        DiffieHellmanExchangeProvider dhExchange;
        DiffieHellmanSymmetricProvider dhSymmetric;

        //private IEncryptionProvider encryptionProvider;
        private ICommunicator Communicator { get; set; }
        public IConnector Connector { get; set; }

        private string SessionID { get; set; }
        private string Username { get; set; }

        public bool IsServer { get; private set; }

        public bool IsClosing
        {
            get => isClosing;
            private set
            {
                if (isClosing == value)
                    return;

                isClosing = value;
                IsClosingChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private ObservableCollection<ChatMessage> _chatMessages = new ObservableCollection<ChatMessage>();
        public ObservableCollection<ChatMessage> ChatMessages { get => _chatMessages; set => _chatMessages = value; }

        private string _chatEntryTextBox;
        private bool isClosing;
        private string otherUsername;
        private bool isConnectionSecure;

        public string ChatEntryTextBox
        {
            get { return _chatEntryTextBox; }
            set
            {
                if (_chatEntryTextBox == value)
                {
                    return;
                }
                _chatEntryTextBox = value;
                RaisePropertyChanged(nameof(ChatEntryTextBox));
            }
        }

        public ICommand SendButtonCommand
        {
            get { return new ActionCommand(() => SendButton()); }
        }

        public string OtherUsername
        {
            get => otherUsername;
            set
            {
                if (otherUsername == value)
                    return;
                otherUsername = value;
                RaisePropertyChanged(nameof(OtherUsername));
            }
        }

        public bool IsConnectionSecure
        {
            get => isConnectionSecure;
            private set
            {
                if (isConnectionSecure == value)
                    return;
                isConnectionSecure = value;
                RaisePropertyChanged(nameof(IsConnectionSecure));
            }
        }

        #endregion

        public ChatVM(IConnector connector,
                      ICommunicator communicator,
                      bool isServer,
                      string sessionId,
                      string username)
        {
            _dispatcher = Application.Current.Dispatcher;
            Communicator = communicator;
            IsServer = isServer;
            SessionID = sessionId;
            Username = username;

            Communicator.OnDataReceived += Comm_OnDataReceived;

            AddMessage("Waiting for connections...", false);
            ChatEntryTextBox = "";

            Connector = connector;
            HandleConnectionState();
        }

        private void HandleConnectionState()
        {
            switch (Connector.ConnectionState)
            {
                case ConnectionState.Connected:
                    SetCommunicator();
                    break;
                case ConnectionState.Started:
                case ConnectionState.NotStarted:
                    Connector.OnConnectionStateChanged -= Connector_OnConnectionStateChanged;
                    Connector.OnConnectionStateChanged += Connector_OnConnectionStateChanged;
                    break;
                case ConnectionState.Disconnected:
                case ConnectionState.Failed:
                case ConnectionState.Unknown:
                    IsClosing = true;
                    break;
            }
        }

        #region Methods

        private void Connector_OnConnectionStateChanged(object sender, ConnectionState state)
        {
            HandleConnectionState();
        }

        private void SetCommunicator()
        {
            if (IsServer)
            {
                dhExchange = new DiffieHellmanExchangeProvider();
                Communicator.Send(dhExchange.GetOwnPublicKey());
            }

            //encryptionProvider = new SymmetricEncryptionProvider(IsServer);

            //foreach (var initBytes in encryptionProvider.GetInitialBytes())
            //{
            //    Communicator.Send(initBytes);
            //}
        }

        private void SendButton()
        {
            if (Connector.ConnectionState == ConnectionState.Connected)
            {
                if (string.IsNullOrEmpty(ChatEntryTextBox))
                {
                    return;
                }

                var messageBytes = dhSymmetric.SendMessage(ChatEntryTextBox);
                Communicator.Send(messageBytes);

                AddMessage(ChatEntryTextBox, false);
                ChatEntryTextBox = "";
            }
            else
            {
                return;
            }
        }

        private void Comm_OnDataReceived(ICommunicator communicator, byte[] data)
        {
            //if (!encryptionProvider.IsInitialized)
            //{
            //    encryptionProvider.Initialize(data);
            //}

            if (IsServer && !dhExchange.IsInitialized)
            {
                dhExchange.ImportPublicKey(data);
                dhSymmetric = new DiffieHellmanSymmetricProvider(dhExchange.GetDerivedKey(), SessionID);
                dhExchange.Dispose();
                var messageBytes = dhSymmetric.SendMessage($"Connection established with {Username}");
                Communicator.Send(messageBytes);
            }
            else if (!IsServer && dhExchange == null)
            {
                dhExchange = new DiffieHellmanExchangeProvider(data);
                Communicator.Send(dhExchange.GetOwnPublicKey());
                dhSymmetric = new DiffieHellmanSymmetricProvider(dhExchange.GetDerivedKey(), SessionID);
                dhExchange.Dispose();
                var messageBytes = dhSymmetric.SendMessage($"Connection established with {Username}");
                Communicator.Send(messageBytes);
            }
            else
            {
                var message = dhSymmetric.ReceiveMessage(data, out bool isValid);
                IsConnectionSecure = isValid;
                if (isValid && OtherUsername == null)
                {
                    OtherUsername = message.Remove(0, "Connection established with ".Length);
                }
                //if (!isValid)
                //{
                //    // TODO - End connection and add text to IpEndpointView when session is invalid instead of just displaying message to user
                //}
                AddMessage(message, true);
            }
        }

        private string GetFormattedTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        public void AddMessage(string message, bool incomming)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(new Action(() => AddMessage(message, incomming)));
                return;
            }

            ChatMessage chatMessage;
            if (incomming)
            {
                chatMessage = new IncommingMessage()
                {
                    Text = message,
                    DateTime = GetFormattedTime()
                };
            }
            else
            {
                chatMessage = new OutgoingMessage()
                {
                    Text = message,
                    DateTime = GetFormattedTime()
                };
            }

            ChatMessages.Add(chatMessage);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(new Action(() => RaisePropertyChanged(propertyName)));
                return;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            if (Communicator != null)
            {
                Communicator.OnDataReceived -= Comm_OnDataReceived;
                Communicator.Stop();
            }

            dhExchange?.Dispose();
        }

        #endregion
    }
}
