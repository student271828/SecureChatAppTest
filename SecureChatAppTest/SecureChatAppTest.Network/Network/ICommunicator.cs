using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Components.Network
{
    public delegate void CommunicationReceivedHandler(ICommunicator communicator, byte[] data);

    public interface ICommunicator
    {
        event CommunicationReceivedHandler OnDataReceived;

        void Send(byte[] data);

        void Stop();
    }
}
