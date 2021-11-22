using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Encryption
{
    public class DiffieHellmanExchangeProvider : IDisposable
    {
        ECDiffieHellmanCng cng;

        private byte[] SelfPublicKey { get; set; }
        private byte[] DerivedKey { get; set; }
        public bool IsInitialized { get; private set; }


        public DiffieHellmanExchangeProvider()
        {
            cng = new ECDiffieHellmanCng();
            cng.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            cng.HashAlgorithm = CngAlgorithm.Sha256;
            SelfPublicKey = cng.PublicKey.ToByteArray();
        }

        public DiffieHellmanExchangeProvider(byte[] _otherPublicKey)
        {
            using (ECDiffieHellmanCng DhCng = new ECDiffieHellmanCng())
            {
                DhCng.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                DhCng.HashAlgorithm = CngAlgorithm.Sha256;
                SelfPublicKey = DhCng.PublicKey.ToByteArray();
                DerivedKey = DhCng.DeriveKeyMaterial(CngKey.Import(_otherPublicKey, CngKeyBlobFormat.EccPublicBlob));
                IsInitialized = true;
            }
        }

        public void ImportPublicKey(byte[] key)
        {
            CreateDerivedKey(key);
            IsInitialized = true;
        }

        public byte[] GetOwnPublicKey()
        {
            return SelfPublicKey;
        }

        private void CreateDerivedKey(byte[] key)
        {
            DerivedKey = cng.DeriveKeyMaterial(CngKey.Import(key, CngKeyBlobFormat.EccPublicBlob));
        }

        public byte[] GetDerivedKey()
        {
            return DerivedKey;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.cng != null)
                    this.cng.Dispose();
            }
        }
    }
}
