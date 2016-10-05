using System;
using System.Security.Cryptography;
using System.Text;

namespace Ix4Models.CryptoModule
{
    public class Cryptor : IDisposable
    {
        private static readonly string Schlüssel = "je8dGckI8dnMXjdusemdpv8n4D8fm2Jd";
        private static readonly string Brunnen = "pvmJd4L4jdprfHnÜ";
        public string Encrypt(string decrypted)
        {
            if (string.IsNullOrEmpty(decrypted))
                return string.Empty;
            byte[] textBytes = ASCIIEncoding.ASCII.GetBytes(decrypted);
            AesCryptoServiceProvider encDec = GetCryptProvider();

            ICryptoTransform icrypt = encDec.CreateEncryptor(encDec.Key, encDec.IV);

            byte[] enc = icrypt.TransformFinalBlock(textBytes, 0, textBytes.Length);
            icrypt.Dispose();
            encDec.Dispose();
            return Convert.ToBase64String(enc);

        }
        private AesCryptoServiceProvider _cyptProvider;
        private AesCryptoServiceProvider GetCryptProvider()
        {
            return new AesCryptoServiceProvider()
            {
                BlockSize = 128,
                KeySize = 256,
                Key = ASCIIEncoding.ASCII.GetBytes(Schlüssel),
                IV = ASCIIEncoding.ASCII.GetBytes(Brunnen),
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            };
        }

        public string Decrypt(string encrypted)
        {
            byte[] encryptBytes = Convert.FromBase64String(encrypted);
            AesCryptoServiceProvider encDec = GetCryptProvider();
            ICryptoTransform icrypt = encDec.CreateDecryptor(encDec.Key, encDec.IV);
            byte[] decryptBytes = icrypt.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length);
            icrypt.Dispose();
            encDec.Dispose();

            return ASCIIEncoding.ASCII.GetString(decryptBytes);

        }

        public void Dispose()
        {
            if (_cyptProvider != null)
            {
                _cyptProvider.Dispose();
                _cyptProvider = null;
            }

        }
    }
}
