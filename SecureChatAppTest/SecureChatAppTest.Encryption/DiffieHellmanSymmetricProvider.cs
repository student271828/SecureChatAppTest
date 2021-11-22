using SecureChatAppTest.Components.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Encryption
{
    public class DiffieHellmanSymmetricProvider
    {
        private byte[] AesKey { get; set; }
        private string SessionID { get; set; }

        public DiffieHellmanSymmetricProvider(byte[] key, string sessionId)
        {
            AesKey = key;
            SessionID = sessionId;
        }

        public void SetSessionID(string id)
        {
            SessionID = id;
        }

        public byte[] SendMessage(string plainText)
        {
            var message = new Message();
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = AesKey;
                aes.GenerateIV();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

                message.IV = aes.IV;
                message.MessageID = EncryptMessageID(HashMessageID(aes.IV, plainText), enc);
                message.MessageData = EncryptMessageData(plainText, enc);
            }
            return MessageSerializer.SerializeMessage(message);
        }

        public string ReceiveMessage(byte[] data, out bool isValid)
        {
            var message = MessageSerializer.DeserializeMessage(data);

            string decrypted = null;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = AesKey;
                aes.IV = message.IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

                var messageID = DecryptMessageID(message, dec);

                decrypted = DecryptMessageData(message, dec);

                if (!VerifyMessageID(messageID, aes.IV, decrypted))
                {
                    decrypted = "You have received an invalid message. Please terminate the session immediately.";
                    isValid = false;
                }
                else
                {
                    isValid = true;
                }
            }

            return decrypted;
        }

        private byte[] EncryptMessageID(byte[] id, ICryptoTransform enc)
        {
            byte[] encryptedId;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(Convert.ToBase64String(id));
                    }

                    encryptedId = ms.ToArray();
                }
            }
            return encryptedId;
        }

        private byte[] EncryptMessageData(string plainText, ICryptoTransform enc)
        {
            byte[] encryptedData;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    encryptedData = ms.ToArray();
                }
            }
            return encryptedData;

        }

        private byte[] DecryptMessageID(Message message, ICryptoTransform dec)
        {
            byte[] decipheredId;

            using (MemoryStream ms = new MemoryStream(message.MessageID))
            {
                using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        decipheredId = Convert.FromBase64String(sr.ReadToEnd());
                    }
                }
            }
            return decipheredId;
        }

        private string DecryptMessageData(Message message, ICryptoTransform dec)
        {
            string plainTextData;

            using (MemoryStream ms = new MemoryStream(message.MessageData))
            {
                using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        plainTextData = sr.ReadToEnd();
                    }
                }
            }
            return plainTextData;
        }

        private byte[] HashMessageID(byte[] iv, string plainText)
        {
            byte[] output;

            byte[] sessionIdBytes = Encoding.UTF8.GetBytes(SessionID);
            byte[] temp = iv.Concat(sessionIdBytes).ToArray();
            byte[] combined = temp.Concat(Encoding.UTF8.GetBytes(plainText)).ToArray();
            using (SHA256 sha256 = SHA256.Create())
            {
                output = sha256.ComputeHash(combined);
            }

            return output;
        }

        public bool VerifyMessageID(byte[] hashedID, byte[] iv, string plainText)
        {
            string ourHashValue = Convert.ToBase64String(HashMessageID(iv, plainText));
            string senderHashValue = Convert.ToBase64String(hashedID);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(ourHashValue, senderHashValue) == 0;
        }

        private void DebugMethod(string name, byte[] data)
        {
            Console.WriteLine($"{name}: {Convert.ToBase64String(data)}");
            Console.WriteLine($"{name} Length: {data.Length}");
        }
    }
}
