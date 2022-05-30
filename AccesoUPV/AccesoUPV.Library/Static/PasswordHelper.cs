using CredentialManagement;

namespace AccesoUPV.Library.Static
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Saves the passed credentials into the Windows Credentials Manager.
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
        /// Deletes the credentials associated to the passed <paramref name="target"/> (URL) from the Windows Credentials Manager.
        /// </summary>
        private static void DeletePassword(string target)
        {
            using var cred = new Credential() { Target = target };
            cred.Delete();
        }

        /// <summary>
        /// Checks if the credentials associated to the passed <paramref name="target"/> (URL) exist within the Windows Credentials Manager.
        /// </summary>
        public static bool Exists(string target)
        {
            using var cred = new Credential() { Target = target };
            return cred.Exists();
        }
    }
}
