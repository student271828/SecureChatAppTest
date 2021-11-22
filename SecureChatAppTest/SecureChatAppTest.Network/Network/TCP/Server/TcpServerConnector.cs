using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SecureChatAppTest.Components.Network;

namespace SecureChatAppTest.Network.Network.TCP.Server
{
    public class TcpServerConnector : IConnector
    {
        public event ConnectionStateChangedDelegate OnConnectionStateChanged;

        private string _localAddress;
        private int _localPort;
        private ConnectionState _connectionState;

        private ListenerState _listenerState;

        private Socket _client;

        public TcpServerConnector(string localAddress, int localPort)
        {
            _localAddress = localAddress;
            _localPort = localPort;
        }

        public Socket Socket => _client;

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
            if (_listenerState != null && _listenerState.Listener != null && _listenerState.Listener.Active)
            {
                return;
            }

            var listener = new TcpListenerEx(IPAddress.Parse(_localAddress), _localPort);
            _listenerState = new ListenerState(listener);
            listener.Start();
            //TODO - Re-evaluate the layer for accepting connections and accepting clients
            _listenerState.Listener.BeginAcceptSocket(BeginAcceptSocketCallback, _listenerState);
        }

        public void Stop()
        {
            try
            {
                if (_listenerState == null || _listenerState.CancelToken == null)
                {
                    ConnectionState = ConnectionState.Disconnected;
                    return;
                }
                _listenerState.CancelToken.Cancel();

                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();
                }

                if (!_listenerState.Listener.Active)
                {
                    ConnectionState = ConnectionState.Disconnected;
                    return;
                }
                _listenerState.Listener.Stop();
                _listenerState = null;
                ConnectionState = ConnectionState.Disconnected;
            }
            catch (Exception)
            {
                ConnectionState = ConnectionState.Unknown;
            }
        }

        //TODO - Re-evaluate the layer for accepting connections and accepting clients
        private void BeginAcceptSocketCallback(IAsyncResult ar)
        {
            try
            {
                var state = ar.AsyncState as ListenerState;
                if (state == null)
                    throw new InvalidOperationException("State must not be null.");

                if (state.CancelToken.IsCancellationRequested)
                {
                    Stop();
                    return;
                }

                if (ar.IsCompleted)
                {
                    var client = state.Listener.EndAcceptSocket(ar);
                    _client = client;
                    ConnectionState = ConnectionState.Connected;
                }
            }
            catch (Exception)
            {
                Stop();
            }
        }

        class ListenerState
        {
            public CancellationTokenSource CancelToken { get; private set; }
            public TcpListenerEx Listener { get; private set; }
            public ListenerState(TcpListenerEx listener)
            {
                CancelToken = new CancellationTokenSource();
                Listener = listener;
            }
        }
    }
}
