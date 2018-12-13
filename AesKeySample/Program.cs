using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace AesKeySample
{
    class Program
    {
        static void Main(string[] args)
        {
            // tips: if you want to run AES (Encrypt/Decrypt) by itself(Independent operation) , you should set Aes's Key/IV by yourself like:this.....
            var key = "1234567890123456789";
            var iv = "98765432109876543210";

            var tryAgain = false;
            do
            {
                Console.WriteLine("AESKey Test Start!");
                Console.WriteLine("Input Some String Please......");
                var inputOriginData = Console.ReadLine();
                Console.WriteLine($"OriginalString: {inputOriginData}");

                // Convert the OriginalString to byte[]  and  Encrypt to Base64UrlString.
                var encrypted = EncryptAES(inputOriginData, key, iv);

                // Display the encrypted data.
                Console.WriteLine($"EncryptedString: {encrypted}");

                // Decrypt the Base64UrlString and Convert byte[] to a OriginalString.
                string dencrypted = DecryptAES(encrypted, key, iv);

                // Display the original data.
                Console.WriteLine($"DencryptedString: {dencrypted}");

                // Just check want to try the test again ?
                var againCheck = false;
                do
                {
                    Console.WriteLine("Try Again!?(Input Y/N)");
                    var result = Console.ReadLine().ToUpper();

                    if (result == "Y")
                    {
                        againCheck = false;
                        tryAgain = true;
                    }
                    else if (result == "N")
                    {
                        againCheck = false;
                        tryAgain = false;
                    }
                    else
                    {
                        againCheck = true;
                        tryAgain = true;
                    }
                } while (againCheck);
            } while (tryAgain);

            Console.WriteLine("AESKey Test Close!");
            Console.ReadLine();

        }


        private static string EncryptAES(string text, string key, string iv)
        {
            //Convert SourceString to byte []
            var sourceBytes = Encoding.UTF8.GetBytes(text);

            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            //Convert key/iv string to byte [] and assign to Aes's Key/IV property 
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] keyData = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            byte[] IVData = md5.ComputeHash(Encoding.UTF8.GetBytes(iv));
            aes.Key = keyData;
            aes.IV = IVData;

            var transform = aes.CreateEncryptor();
            //I Encrypt sourceString to Base64Url(Base64String for Url),you can Encrypt to Base64String (System.Convert.ToBase64String)
            return Base64UrlTextEncoder.Encode(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));
        }

        private static string DecryptAES(string text, string key, string iv)
        {
            //I Decrypt Base64Url(Base64String for Url) to byte [] ,you can Decrypt Base64String to byte [](System.Convert.FromBase64String)
            var encryptBytes = Base64UrlTextEncoder.Decode(text);

            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            //Convert key/iv string to byte [] and assign to Aes's Key/IV property 
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] keyData = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            byte[] IVData = md5.ComputeHash(Encoding.UTF8.GetBytes(iv));
            aes.Key = keyData;
            aes.IV = IVData;

            var transform = aes.CreateDecryptor();
            //Convert byte [] to SourceString
            return Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));
        }
    }
}
