using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Encryption
{
    /// <summary>
    /// Not Implemented
    /// </summary>
    public class AsymmetricTest
    {
        private RSAParameters SelfPrivateKey { get; set; }
        private RSAParameters SelfPublicKey { get; set; }
        private RSAParameters OtherPublicKey { get; set; }

        public AsymmetricTest()
        {
            GenerateKeyPair();
        }

        private void GenerateKeyPair()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                SelfPublicKey = rsa.ExportParameters(false);
                SelfPrivateKey = rsa.ExportParameters(true);
            }
        }

        public byte[] EncryptData(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            byte[] encryptedMessage;

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(OtherPublicKey);
                encryptedMessage = rsa.Encrypt(data, true);
            }

            return encryptedMessage;
        }

        public string DecryptData(byte[] input)
        {
            byte[] decrypted;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(SelfPrivateKey);
                decrypted = rsa.Decrypt(input, true);
            }

            return Encoding.ASCII.GetString(decrypted);
        }

        public byte[] ExportPublicKey()
        {
            byte[] export;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportParameters(SelfPublicKey);
                export = rsa.ExportCspBlob(false);
            }
            return export;
        }

        public void ImportPublicKey(byte[] key)
        {
            RSAParameters rp;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportCspBlob(key);
                rp = rsa.ExportParameters(false);
            }

            OtherPublicKey = rp;
        }
    }
}
