using System;
using System.Net.Sockets;
using System.Threading;
using SecureChatAppTest.Components.Network;

namespace SecureChatAppTest.Network.Network.TCP.Client
{
    public class TcpClientConnector : IConnector
    {
        public event ConnectionStateChangedDelegate OnConnectionStateChanged;

        private string _remoteAddress;
        private int _remotePort;
        private ConnectionState _connectionState;

        private ClientState _clientState;

        public TcpClientConnector(string remoteAddress, int remotePort)
        {
            _remoteAddress = remoteAddress;
            _remotePort = remotePort;
        }

        public Socket Socket => _clientState?.Client?.Client;

        public ConnectionState ConnectionState 
        {
            get => _connectionState;
            private set
            {
                if (value == _connectionState)
                    return;
                _connectionState = value;
                OnConnectionStateChanged?.Invoke(this, value);
            }
        }

        public void Start()
        {
            if (_clientState != null && _clientState.Client.Active)
                return;

            var client = new TcpClientEx();

            _clientState = new ClientState(client);
            client.BeginConnect(_remoteAddress, _remotePort, BeginConnectCallback, _clientState);
        }

        public void Stop()
        {
            try
            {
                if (_clientState == null || _clientState.CancelToken == null)
                {
                    ConnectionState = ConnectionState.Disconnected;
                    return;
                }
                _clientState.CancelToken.Cancel();
                if (!_clientState.Client.Active)
                {
                    ConnectionState = ConnectionState.Disconnected;
                    return;
                }
                _clientState.Client.Close();
                _clientState.Client.Dispose();
                _clientState = null;
                ConnectionState = ConnectionState.Disconnected;
            }
            catch (Exception ex)
            {
                ConnectionState = ConnectionState.Unknown;
            }
        }

        private void BeginConnectCallback(IAsyncResult ar)
        {
            try
            {
                var state = ar.AsyncState as ClientState;
                if (state == null)
                    throw new InvalidOperationException("State must not be null.");

                if (state.CancelToken.IsCancellationRequested)
                {
                    Stop();
                    return;
                }

                if (ar.IsCompleted)
                {
                    state.Client.EndConnect(ar);
                    ConnectionState = ConnectionState.Connected;
                }
            }
            catch(Exception ex)
            {
                Stop();
            }
        }

        private class ClientState
        {
            public CancellationTokenSource CancelToken { get; private set; }
            public TcpClientEx Client { get; private set; }

            public ClientState(TcpClientEx client)
            {
                CancelToken = new CancellationTokenSource();
                Client = client;
            }
        }
    }
}
