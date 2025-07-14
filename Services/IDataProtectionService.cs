
namespace myapp.Services
{
    public interface IDataProtectionService
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}
