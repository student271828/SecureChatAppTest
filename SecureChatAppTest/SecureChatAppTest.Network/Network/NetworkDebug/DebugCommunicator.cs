using SecureChatAppTest.Components.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Network.Network.NetworkDebug
{
    public class DebugCommunicator : ICommunicator
    {
        public event CommunicationReceivedHandler OnDataReceived;

        public DebugCommunicator OtherCommunicator { get; set; }
        public DebugCommunicator()
        {
        }

        public void Send(byte[] data)
        {
            OtherCommunicator.SendToOther(data);
        }

        private void SendToOther(byte[] data)
        {

            OnDataReceived?.Invoke(this, data);
        }
    }
}
