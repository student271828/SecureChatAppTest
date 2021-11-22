using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Network.Network.TCP
{
    public class TcpClientEx : TcpClient
    {
        public TcpClientEx()
        {
        }

        public TcpClientEx(IPEndPoint localEP) : base(localEP)
        {
        }

        public TcpClientEx(AddressFamily family) : base(family)
        {
        }

        public TcpClientEx(string hostname, int port) : base(hostname, port)
        {
        }

        public new bool Active { get { return base.Active; } }
    }
}
