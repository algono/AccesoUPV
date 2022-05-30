using CredentialManagement;

namespace AccesoUPV.Library.Static
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Saves the passed credentials into the Windows Credential Manager.
        /// If the <paramref name="password"/> is empty, it deletes the credentials instead.
        /// </summary>
        public static void SavePassword(string username, string password, string target)
        {
            if (string.IsNullOrEmpty(password))
            {
                DeletePassword(target);
                return;
            }

            using var cred = new Credential(username, password, target, CredentialType.Generic) { PersistanceType = PersistanceType.Enterprise };
            cred.Save();
        }

        /// <summary>
        /// Gets the password associated to the passed <paramref name="target"/> (URL) from the Windows Credential Manager.
        /// </summary>
        public static string GetPassword(string target)
        {
            using var cred = new Credential() { Target = target };
            cred.Load();
            return cred.Password;
        }

        /// <summary>
        /// Deletes the credentials associated to the passed <paramref name="target"/> (URL) from the Windows Credential Manager.
        /// </summary>
        public static void DeletePassword(string target)
        {
            using var cred = new Credential() { Target = target };
            cred.Delete();
        }
    }
}
