namespace Bookify.Domain;

public interface IPasswordCipherService
{
    string EncryptPassword(string passwordRaw);
}