using System.Security.Cryptography;
using System.Text;

namespace LugaresParaIr.Utils;

public class HashPassword
{
    private int KeySize = 64;
    private int Iterations = 350000;
    private HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;
    private string Password;
    public HashPassword(string password)
    {
        Password = password;
    }
    public string CreateHashSalt(out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(KeySize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(Password),
            salt, Iterations,
            HashAlgorithm,
            KeySize);
        return Convert.ToHexString(hash);
    }
}