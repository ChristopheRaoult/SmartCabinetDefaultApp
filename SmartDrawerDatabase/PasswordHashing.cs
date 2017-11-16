using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase
{
    public static class PasswordHashing
    {
        public static string Md5Of(string text)
        {
            return Md5Of(text, Encoding.Default);
        }

        public static string Md5Of(string text, Encoding enc)
        {
            return HashOf<MD5CryptoServiceProvider>(text, enc);
        }

        public static string Sha1Of(string text)
        {
            return Sha1Of(text, Encoding.Default);
        }

        public static string Sha1Of(string text, Encoding enc)
        {
            return HashOf<SHA1CryptoServiceProvider>(text, enc);
        }

        public static string Sha256Of(string text)
        {
            return Sha256Of(text, Encoding.Default);
        }

        public static string Sha256Of(string text, Encoding enc)
        {
            return HashOf<SHA256CryptoServiceProvider>(text, enc);
        }

        public static string Sha512Of(string text)
        {
            return Sha512Of(text, Encoding.Default);
        }

        public static string Sha512Of(string text, Encoding enc)
        {
            return HashOf<SHA512CryptoServiceProvider>(text, enc);
        }

        public static string HashOf<TP>(string text, Encoding enc)
            where TP : HashAlgorithm, new()
        {
            var buffer = enc.GetBytes(text);
            var provider = new TP();
            return BitConverter.ToString(provider.ComputeHash(buffer)).Replace("-", "");
        }
    }
}
