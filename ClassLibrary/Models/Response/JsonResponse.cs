using System.Text;
using System.Security.Cryptography;

namespace ClassLibrary.Models.Response
{
    public class JsonResponse
    {
        private static string PadOrTruncateKey(string key, int desiredSize)
        {
            if (key.Length < desiredSize)
                key = key.PadRight(desiredSize, '\0');
            else if (key.Length > desiredSize)
                key = key.Substring(0, desiredSize);

            return key;
        }

        public string EncryptJson(object jsonData, string encryptionKey)
        {
            try
            {
                string jsonString = System.Text.Json.JsonSerializer.Serialize(jsonData);
                return EncryptJson(jsonString, encryptionKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EncryptObjectToJson Error: {ex.Message}");
                return string.Empty;
            }
        }

        public T DecryptJson<T>(string encryptedData, string encryptionKey)
        {
            try
            {
                string decryptedJsonString = DecryptJson(encryptedData, encryptionKey);
                return System.Text.Json.JsonSerializer.Deserialize<T>(decryptedJsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DecryptJsonToObject Error: {ex.Message}");
                return default(T)!;
            }
        }

        private string EncryptJson(string data, string encryptionKey)
        {
            try
            {
                encryptionKey = PadOrTruncateKey(encryptionKey, 32);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
                    aesAlg.GenerateIV();

                    using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    using (var msEncrypt = new MemoryStream())
                    {
                        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EncryptJsonString Error: {ex.Message}");
                return string.Empty;
            }
        }

        private string DecryptJson(string encryptedData, string encryptionKey)
        {
            try
            {
                encryptionKey = PadOrTruncateKey(encryptionKey, 32);
                byte[] fullCipher = Convert.FromBase64String(encryptedData);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
                    byte[] iv = fullCipher.Take(16).ToArray();
                    byte[] cipher = fullCipher.Skip(16).ToArray();
                    aesAlg.IV = iv;

                    using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    using (var msDecrypt = new MemoryStream(cipher))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DecryptJsonString Error: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
