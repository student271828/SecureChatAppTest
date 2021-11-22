using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Components.Network
{
    public enum ConnectionState
    {
        NotStarted,
        Started,
        Connected,
        Disconnected,
        Failed,
        Unknown,
    }

    public delegate void ConnectionStateChangedDelegate(object sender, ConnectionState state);

    public interface IConnector
    {
        event ConnectionStateChangedDelegate OnConnectionStateChanged;
        ConnectionState ConnectionState { get; }
        Socket Socket { get; }
        void Start();
        void Stop();
    }
}
