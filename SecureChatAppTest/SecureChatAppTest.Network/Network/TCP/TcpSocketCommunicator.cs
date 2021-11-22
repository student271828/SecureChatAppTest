using SecureChatAppTest.Components.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Network.Network.TCP
{
    public class TcpSocketCommunicator : ICommunicator
    {
        public event CommunicationReceivedHandler OnDataReceived;

        private readonly IConnector _connector;
        private readonly bool _isServer;
        private TcpCommunicatorState _state;

        public TcpSocketCommunicator(IConnector connector)
        {
            if (connector == null)
                throw new ArgumentException("Connector must not be null.");

            _connector = connector;
            if (_connector.ConnectionState == ConnectionState.Connected)
            {
                Start();
            }
            else
            {
                _connector.OnConnectionStateChanged += _connector_OnConnectionStateChanged;
            }
        }

        private void _connector_OnConnectionStateChanged(object sender, ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.Connected:
                    Start();
                    break;
                //TODO - Figure out what to do in other states
            }
        }

        public async void Send(byte[] data)
        {
            await _connector.Socket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
        }

        private void Start()
        {
            _state = new TcpCommunicatorState(_connector.Socket);
            StartBeginReceive(_state);
        }

        public void Stop()
        {
            _state?.Cancel();
            _connector.Stop();
        }

        private void StartBeginReceive(TcpCommunicatorState state)
        {
            try 
            {
                if (state.IsCanceled)
                    return;

                var buffer = new byte[65535];
                state.SetBuffer(buffer);
                _connector.Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, BeginReceiveCallback, state);
            }
            catch (Exception ex)
            {
                _connector.Stop();
            }
        }

        private void BeginReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = ar.AsyncState as TcpCommunicatorState;
                if (state == null)
                    throw new InvalidOperationException("state must not be null.");
                if (state.IsCanceled)
                    return;

                if (ar.IsCompleted)
                {
                    var receivedCount = _connector.Socket.EndReceive(ar);
                    var buffer = state.GetBuffer();
                    ProcessBuffer(buffer, receivedCount);
                    StartBeginReceive(state);
                }
            }
            catch (Exception ex)
            {
                _connector.Stop();
            }
        }

        private void ProcessBuffer(byte[] buffer, int receivedCount)
        {
            var outBuffer = new byte[receivedCount];
            Array.Copy(buffer, 0, outBuffer, 0, receivedCount);
            OnDataReceived?.Invoke(this, outBuffer);
        }
    }
}
