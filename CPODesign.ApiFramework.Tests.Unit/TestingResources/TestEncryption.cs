using CPODesign.ApiFramework.Encryption;

namespace CPODesign.ApiFramework.Tests.Unit.TestingResources
{
    internal class TestEncryption : IUserAuthenticationEncryption
    {
        public string EncryptUserNameAndPassword(string userName, string password)
        {
            return $"{userName}-{password}";
        }
    }
}
