using CredentialManagement;

namespace AccesoUPV.Library.Static
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Saves the passed credentials into the Windows Credentials Manager.
        /// If <paramref name="password"/> is empty, it deletes the credentials instead.
        /// <para></para>
        /// <returns>Returns <see langword="true"/> if the credentials have been stored, and <see langword="false"/> if they have been deleted.</returns>
        /// </summary>
        public static bool SavePassword(string username, string password, string target)
        {
            if (string.IsNullOrEmpty(password))
            {
                DeletePassword(target);
                return false;
            }

            using var cred = new Credential(username, password, target, CredentialType.Generic) { PersistanceType = PersistanceType.Enterprise };
            cred.Save();

            return true;
        }

        /// <summary>
        /// Saves the passed credentials into the Windows Credentials Manager.
        /// If <paramref name="securePassword"/> is empty, it deletes the credentials instead.
        /// <para></para>
        /// <returns>Returns <see langword="true"/> if the credentials have been stored, and <see langword="false"/> if they have been deleted.</returns>
        /// </summary>
        public static bool SaveSecurePassword(string username, System.Security.SecureString securePassword, string target)
        {
            if (securePassword == null || securePassword.Length == 0)
            {
                DeletePassword(target);
                return false;
            }

            using var cred = new Credential(username)
            {
                SecurePassword = securePassword,
                Target = target,
                Type = CredentialType.Generic,
                PersistanceType = PersistanceType.Enterprise
            };
            cred.Save();

            return true;
        }

        /// <summary>
        /// Deletes the credentials associated to the passed <paramref name="target"/> (URL) from the Windows Credentials Manager.
        /// </summary>
        public static void DeletePassword(string target)
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
