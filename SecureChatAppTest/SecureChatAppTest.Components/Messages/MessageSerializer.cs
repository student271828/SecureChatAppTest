using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SecureChatAppTest.Components.Messages
{
    public static class MessageSerializer
    {
        public static byte[] SerializeMessage(Message message)
        {
            byte[] output;

            XmlSerializer serializer = new XmlSerializer(typeof(Message));

            using (MemoryStream mem = new MemoryStream())
            {
                mem.Position = 0;
                serializer.Serialize(mem, message);
                output = mem.ToArray();
            }

            return output;
        }

        public static Message DeserializeMessage(byte[] data)
        {
            Message output;

            XmlSerializer deserializer = new XmlSerializer(typeof(Message));

            using (MemoryStream mem = new MemoryStream(data))
            {
                mem.Position = 0;

                output = (Message)deserializer.Deserialize(mem);
            }

            return output;
        }
    }
}
