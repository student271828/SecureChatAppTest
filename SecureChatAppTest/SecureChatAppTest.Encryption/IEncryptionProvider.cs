using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTest.Encryption
{
    /// <summary>
    /// Not Implemented
    /// </summary>
    public interface IEncryptionProvider
    {
        bool IsInitialized {get;}

        List<byte[]> GetInitialBytes();
        void Initialize(byte[] initBytes);
        byte[] EncryptBytes(string message);
        string DecryptBytes(byte[] cipher);
    }
}
