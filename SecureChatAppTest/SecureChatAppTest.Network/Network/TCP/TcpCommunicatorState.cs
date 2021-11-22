using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecureChatAppTest.Network.Network.TCP
{
    public class TcpCommunicatorState
    {
        private readonly CancellationTokenSource _cancelToken;
        private readonly Socket _socket;
        private byte[] _buffer;
        public bool IsCanceled { get; private set; }

        public TcpCommunicatorState(Socket socket)
        {
            _cancelToken = new CancellationTokenSource();
            _socket = socket;
        }

        public void Cancel()
        {
            _cancelToken?.Cancel();
            IsCanceled = true;
        }

        internal void SetBuffer(byte[] buffer)
        {
            _buffer = buffer;
        }

        internal byte[] GetBuffer()
        {
            return _buffer;
        }
    }
}
