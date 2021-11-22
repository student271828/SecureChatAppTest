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
    public class SymmetricEncryptionProvider : IEncryptionProvider
    {
        private readonly bool _isServer;

        private string AesKey { get; set; }
        private string AesIv { get; set; }

        public bool IsInitialized { get; private set; }

        public SymmetricEncryptionProvider(bool server)
        {
            _isServer = server;
            if (server)
            {
                AesManaged _aes = new AesManaged();
                _aes.GenerateKey();
                _aes.GenerateIV();

                AesKey = Convert.ToBase64String(_aes.Key);
                AesIv = Convert.ToBase64String(_aes.IV);
            }
            IsInitialized = _isServer;
        }

        public List<byte[]> GetInitialBytes()
        {
            var initBytes = new List<byte[]>();
            if (_isServer)
            {
                initBytes.Add(GetSymmetricKey_Bytes());
                initBytes.Add(GetSymmetricIv_Bytes());
            }
            return initBytes;
        }

        public void Initialize(byte[] data)
        {
            if (IsInitialized)
                return;

            if (!IsSymmetricKeySet())
            {
                SetSymmetricKey_Bytes(data);
            }
            else if (!IsSymmetricIvSet())
            {
                SetSymmetricIv_Bytes(data);
            }
            else
            {
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Returns a byte array from the key class property implemenation
        /// </summary>
        /// <returns></returns>
        private byte[] GetSymmetricKey_Bytes()
        {
            string key = AesKey;
            return Convert.FromBase64String(key);
        }

        /// <summary>
        /// Returns a byte array from the IV class property implementation
        /// </summary>
        /// <returns></returns>
        private byte[] GetSymmetricIv_Bytes()
        {
            string iv = AesIv;
            return Convert.FromBase64String(iv);
        }

        /// <summary>
        /// Converts a byte array to the key string for the class implementation
        /// </summary>
        /// <param name="key"></param>
        public void SetSymmetricKey_Bytes(byte[] key)
        {
            AesKey = Convert.ToBase64String(key);
        }

        /// <summary>
        /// Converts a byte array to the IV string for the class implementation
        /// </summary>
        /// <param name="iv"></param>
        public void SetSymmetricIv_Bytes(byte[] iv)
        {
            AesIv = Convert.ToBase64String(iv);
        }

        /// <summary>
        /// Determines if current instance has a symmetric key set
        /// </summary>
        /// <returns>true if key has a value, false if key is not set</returns>
        public bool IsSymmetricKeySet()
        {
            if (AesKey != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if current instance has a symmetric IV set
        /// </summary>
        /// <returns>true if IV has a value, false if IV is not set</returns>
        public bool IsSymmetricIvSet()
        {
            if (AesIv != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Original Example
        public string EncryptAES(string plainText)
        {
            byte[] encrypted;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = Convert.FromBase64String(AesKey);
                aes.IV = Convert.FromBase64String(AesIv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }

                        encrypted = ms.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        // Interface implementation
        public byte[] EncryptBytes(string plainText)
        {
            byte[] encrypted;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = Convert.FromBase64String(AesKey);
                aes.IV = Convert.FromBase64String(AesIv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }

                        encrypted = ms.ToArray();
                    }
                }
            }

            return encrypted;
        }

        //Original Example
        public string DecryptAES(string encryptedText)
        {
            string decrypted = null;
            byte[] cipher = Convert.FromBase64String(encryptedText);

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = Convert.FromBase64String(AesKey);
                aes.IV = Convert.FromBase64String(AesIv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipher))
                {
                    using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            decrypted = sr.ReadToEnd();
                        }
                    }
                }
            }

            return decrypted;
        }

        // Interface implementation
        public string DecryptBytes(byte[] cipher)
        {
            string decrypted = null;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = Convert.FromBase64String(AesKey);
                aes.IV = Convert.FromBase64String(AesIv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipher))
                {
                    using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            decrypted = sr.ReadToEnd();
                        }
                    }
                }
            }

            return decrypted;
        }
    }
}
