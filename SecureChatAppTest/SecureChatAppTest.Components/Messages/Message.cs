using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SecureChatAppTest.Components.Messages
{
    [XmlRoot("Message")]
    public class Message
    {
        [XmlArray]
        public byte[] IV { get; set; }

        [XmlArray]
        public byte[] MessageID { get; set; }

        [XmlArray]
        public byte[] MessageData { get; set; }
    }
}
