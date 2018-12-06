using System;
using System.Collections.Generic;
using System.Text;

namespace CPODesign.ApiFramework
{
    public interface IUserAuthenticationEncryption
    {
        string EncryptUserNameAndPassword(string userName, string password);
    }
}
