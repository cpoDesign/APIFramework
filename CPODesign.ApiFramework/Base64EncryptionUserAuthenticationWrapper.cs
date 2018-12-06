using System;

namespace CPODesign.ApiFramework
{
    public class Base64EncryptionUserAuthenticationWrapper : IUserAuthenticationEncryption
    {
        public string EncryptUserNameAndPassword(string userName, string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{userName}:{password}"));
        }
    }
}