using System.Security.Cryptography;
using System.Text;

namespace TicketManager.Helpers
{
    public class SecurityHelper
    {
        // this method to encrypt the password
        public static string Encrypt(string password, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length);
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(password);
                            }
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        // this method to decrypt the password
        public static string Decrypt(string hashedPassword, string key)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] fullCipher = Convert.FromBase64String(hashedPassword);
                byte[] iv = new byte[aes.IV.Length];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);

                using (var decryptor = aes.CreateDecryptor(Encoding.UTF8.GetBytes(key), iv))
                {
                    using (var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
