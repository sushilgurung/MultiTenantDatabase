using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Security
{
    public static class EncriptionHelper
    {
        // Key should be 32 bytes for AES-256
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("YourEncryptionKeyHere1234567890123456"); 
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("YourIVHere123456"); 
     
        public static string Encrypt(string dataToEncrypt)
        {
            if (string.IsNullOrEmpty(dataToEncrypt))
                throw new ArgumentNullException(nameof(dataToEncrypt));

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(dataToEncrypt);
                        }
                    }

                    byte[] encrypted = msEncrypt.ToArray();
                    return Convert.ToBase64String(encrypted); // Return encrypted data as base64 string
                }
            }
        }

        public static string Decrypt(string dataToDecrypt)
        {
            if (string.IsNullOrEmpty(dataToDecrypt))
                throw new ArgumentNullException(nameof(dataToDecrypt));

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] cipherText = Convert.FromBase64String(dataToDecrypt);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd(); // Return decrypted data as plain text
                        }
                    }
                }
            }
        }
    }
}
