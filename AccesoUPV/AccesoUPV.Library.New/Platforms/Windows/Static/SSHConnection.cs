using System;

namespace AccesoUPV.Library.Static
{
    public static partial class SSHConnection
    {
        private static readonly string SSH_PATH = $@"C:\Windows\{(Environment.Is64BitProcess ? "System32" : "Sysnative")}\OpenSSH\ssh.exe";
    }
}