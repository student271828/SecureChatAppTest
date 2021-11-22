using SecureChatAppTest.Components.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Encryption
{
    /// <summary>
    /// Not Implemented
    /// </summary>
    public class AsymmetricEncryptionProvider : IEncryptionProvider
    {
        RSACryptoServiceProvider rsa;

        private string SelfPubicKey { get; set; }
        private string SelfPrivateKey { get; set; }
        private string OtherParticipantPublicKey { get; set; }

        public bool IsInitialized { get; private set; }

        public AsymmetricEncryptionProvider()
        {
            rsa = new RSACryptoServiceProvider();
            SelfPubicKey = rsa.ToXmlString(false);
            SelfPrivateKey = rsa.ToXmlString(true);
        }

        public List<byte[]> GetInitialBytes()
        {
            var initBytes = new List<byte[]>();
            return initBytes;
        }

        // TODO - need to add something to handle the key exchange between client and server

        // TODO - need to add something to handle the signing of the message to prove the sender

        public void Initialize(byte[] initBytes)
        {
            if (IsInitialized)
                return;

            if (OtherParticipantPublicKey == null)
            {
                OtherParticipantPublicKey = Convert.ToBase64String(initBytes);
            }
            else
            {
                IsInitialized = true;
            }
        }

        public string DecryptBytes(byte[] cipher)
        {
            byte[] decryptedData;
            string output = "";

            using (RSACryptoServiceProvider rsaCP = new RSACryptoServiceProvider())
            {
                rsaCP.FromXmlString(SelfPrivateKey);
                decryptedData = rsaCP.Decrypt(cipher, false);
                output = Convert.ToBase64String(decryptedData);
            }

            return output;
        }

        public byte[] EncryptBytes(string message)
        {
            byte[] data = Convert.FromBase64String(message);
            byte[] output;

            using (RSACryptoServiceProvider rsaCP = new RSACryptoServiceProvider())
            {
                rsaCP.FromXmlString(OtherParticipantPublicKey);
                output = rsaCP.Encrypt(data, false);
            }

            return output;
        }

        public byte[] HashAndSignBytes(byte[] data, RSAParameters key)
        {
            try
            {
                using (MemoryStream memStream = new MemoryStream(data))
                {
                    memStream.Position = 0;

                    using (RSACryptoServiceProvider rsaCP = new RSACryptoServiceProvider())
                    {
                        rsaCP.ImportParameters(key);
                        return rsaCP.SignData(memStream, SHA256.Create());
                    }
                }
            }
            catch (Exception)
            {
                throw new CryptographicException();
            }
        }

        public bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, RSAParameters key)
        {
            try
            {
                using (RSACryptoServiceProvider rsaCP = new RSACryptoServiceProvider())
                {
                    rsaCP.ImportParameters(key);
                    return rsaCP.VerifyData(DataToVerify, SHA256.Create(), SignedData);
                }
            }
            catch (Exception)
            {
                throw new CryptographicException();
            }
        }

        public void ExportPublicKey(ICommunicator communicator) //NOTE: I Don't know if this is correct for XML string to Base64 string
        {
            communicator.Send(Convert.FromBase64String(SelfPubicKey));
        }

        public void ImportPublicKey(byte[] publicKey) //NOTE: I Don't know if this is correct for XML string to Base64 string
        {
            OtherParticipantPublicKey = Convert.ToBase64String(publicKey);
        }
    }
}
