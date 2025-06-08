using System.Security.Cryptography;
using System.Text;

namespace CleanArchitecture.Application.Common.Helpers;

public static class HashHelper
{
    public static string GenerateSha256Hash(string data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder builder = new StringBuilder();

            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}