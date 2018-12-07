namespace CPODesign.ApiFramework.Encryption
{
    public interface IUserAuthenticationEncryption
    {
        string EncryptUserNameAndPassword(string userName, string password);
    }
}
