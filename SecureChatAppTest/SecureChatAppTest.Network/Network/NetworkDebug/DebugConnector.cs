using SecureChatAppTest.Components.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Network.Network.NetworkDebug
{
    public class DebugConnector : IConnector
    {
        public bool IsConnected { get; private set; }

        public Socket Socket => throw new NotImplementedException();
        public IConnector OtherConnector { get; set; }

        public event EventHandler OnConnectionComplete;

        public async void Start()
        {
            await Task.Run(() =>
            {
                Task.Delay(100).Wait();
                IsConnected = true;

                while (!OtherConnector.IsConnected)
                {
                    Task.Delay(200).Wait();
                }

                OnConnectionComplete?.Invoke(this, null);
            });
        }

        public void Stop()
        {
            IsConnected = false;
        }
    }
}
