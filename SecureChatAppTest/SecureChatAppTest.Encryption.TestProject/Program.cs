using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SecureChatAppTest.Encryption;
using SecureChatAppTest.Components.Messages;

namespace SecureChatAppTest.Encryption.TestProject
{
    /// <summary>
    /// Test class to see if the encryption stuff is working
    /// </summary>
    class Program
    {
        private static string input = "";
        private static string encryptedArray;

        static void Main(string[] args)
        {
            RunTheThing_DiffieHellman();

            Console.WriteLine("Would you like to run again? (y or n)");
            string input = Console.ReadLine();

            if (input.ToLower() == "y")
            {
                RunTheThing_DiffieHellman();
            }
            else
            {
                Console.WriteLine("Thanks.");
            }


            Console.ReadLine();
        }

        static void RunTheThing_Symmetric()
        {
            Console.WriteLine("Enter a string to encrypt");
            input = Console.ReadLine();

            Console.WriteLine($"Your input string is '{input}'");
            Console.WriteLine($"The string length is: {input.Length}");

            SymmetricEncryptionProvider sep = new SymmetricEncryptionProvider(true);
            SymmetricEncryptionProvider testSep = new SymmetricEncryptionProvider(false);

            Console.WriteLine($"Is key set: {sep.IsSymmetricKeySet().ToString()}");
            Console.WriteLine($"Is test key set: {testSep.IsSymmetricKeySet().ToString()}");

            encryptedArray = sep.EncryptAES(input);

            Console.WriteLine($"Encrypted message: {encryptedArray}");
            Console.WriteLine($"The encrypted message length is: {encryptedArray.Length}");

            Console.WriteLine(Environment.NewLine + $"Decrypted Message: {sep.DecryptAES(encryptedArray)}");
            Console.WriteLine($"The string length is: {sep.DecryptAES(encryptedArray).Length}");
        }

        //Note you will need to change the Encryption key in the class to your own to get this to work
        static void RunTheThing_Asymmetric()
        {
            Console.WriteLine("Enter a string to encrypt");
            input = Console.ReadLine();

            Console.WriteLine($"Your input string is '{input}'");
            Console.WriteLine($"The string length is: {input.Length}");

            AsymmetricTest at = new AsymmetricTest();

            var pk1 = at.ExportPublicKey();
            at.ImportPublicKey(pk1);

            byte[] encryptedData = at.EncryptData(input);

            Console.WriteLine($"Encrypted data: {Convert.ToBase64String(encryptedData)}");
            Console.WriteLine($"Encrypted data length: {Convert.ToBase64String(encryptedData).Length}");

            string decryptedData = at.DecryptData(encryptedData);

            Console.WriteLine($"Decrypted message: {decryptedData}");
            Console.WriteLine($"The string length is: {decryptedData.Length}");
        }

        static void RunTheThing_DiffieHellman()
        {
            Console.WriteLine("Enter a string to encrypt");
            input = Console.ReadLine();

            Console.WriteLine($"Your input string is '{input}'");
            Console.WriteLine($"The string length is: {input.Length}");

            var Alice = new DiffieHellmanExchangeProvider();
            var Bob = new DiffieHellmanExchangeProvider(Alice.GetOwnPublicKey());
            Alice.ImportPublicKey(Bob.GetOwnPublicKey());


            string sessionID = "ahfgjsdlkfjasdlkf";
            var AKey = new DiffieHellmanSymmetricProvider(Alice.GetDerivedKey(), sessionID);
            

            var BKey = new DiffieHellmanSymmetricProvider(Bob.GetDerivedKey(), sessionID);

            Alice.Dispose();
            Bob.Dispose();

            var cipherMessage = AKey.SendMessage(input);

            //Console.WriteLine(Environment.NewLine + $"IV: {Convert.ToBase64String(cipherMessage.IV)}");
            //Console.WriteLine($"Message: {Convert.ToBase64String(cipherMessage.MessageData)}");

            //var serializedCipher = MessageSerializer.SerializeMessage(cipherMessage);

            //var deserializedCipher = MessageSerializer.DeserializeMessage(serializedCipher);

            //Console.WriteLine("*********************************");
            //Console.WriteLine($"Raw IV: {Convert.ToBase64String(deserializedCipher.IV)}");
            //Console.WriteLine($"Raw IV length: {deserializedCipher.IV.Length}");
            //Console.WriteLine($"Raw ID: {Convert.ToBase64String(deserializedCipher.MessageID)}");
            //Console.WriteLine($"Raw ID length: {deserializedCipher.MessageID.Length}");
            //Console.WriteLine($"Raw Data: {Convert.ToBase64String(deserializedCipher.MessageData)}");
            //Console.WriteLine($"Raw Data length: {deserializedCipher.MessageData.Length}");
            //Console.WriteLine("*********************************");

            var decryptedMessage = BKey.ReceiveMessage(cipherMessage, out bool isValid);

            Console.WriteLine(Environment.NewLine + $"Decrypted message: {decryptedMessage}");
            Console.WriteLine($"The string length is: {decryptedMessage.Length}");

            //Console.WriteLine($"Encrypted data: {Convert.ToBase64String(encryptedData)}");
            //Console.WriteLine($"Encrypted data length: {Convert.ToBase64String(encryptedData).Length}");

            //byte[] encryptedData = AKey.EncryptBytes(iv, input);

            //Console.WriteLine($"Encrypted data: {Convert.ToBase64String(encryptedData)}");
            //Console.WriteLine($"Encrypted data length: {Convert.ToBase64String(encryptedData).Length}");

            //string decryptedData = BKey.DecryptBytes(iv, encryptedData);

            //Console.WriteLine($"Decrypted message: {decryptedData}");
            //Console.WriteLine($"The string length is: {decryptedData.Length}");
        }
    }
}
