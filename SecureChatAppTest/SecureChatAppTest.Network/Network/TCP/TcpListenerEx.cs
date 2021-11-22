using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Network.Network.TCP
{
    public class TcpListenerEx : TcpListener
    {
        public TcpListenerEx(IPEndPoint localEP) : base(localEP) { }
        public TcpListenerEx(int port) : base(port) { }
        public TcpListenerEx(IPAddress localaddr, int port) : base(localaddr, port) { }

        public new bool Active
        {
            get { return base.Active; }
        }
    }
}
