using System.Security.Cryptography;
using System.Text;
namespace AddMemberSystem.Classes.Util
{
    public class HashUtil
    {
        public static string ComputeSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                string hashedValue = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashedValue;
            }
        }
    }
}