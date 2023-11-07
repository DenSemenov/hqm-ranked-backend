using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using System.Text;

namespace hqm_ranked_backend.Helpers
{
    public static class Encryption
    {
        public static string GetMD5Hash(string password)
        {
            using (var hashAlg = MD5.Create())
            {
                byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("X2"));
                }
                return builder.ToString();
            }
        }
    }
}
