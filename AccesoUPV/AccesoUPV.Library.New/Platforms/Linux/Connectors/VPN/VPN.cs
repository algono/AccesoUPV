using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public partial class VPN
    {
        #region Properties
        public bool IsActuallyConnected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private static readonly ProcessStartInfo
            ConnectionInfo = CreateProcessInfo("nmcli"),
            DisconnectionInfo = CreateProcessInfo("nmcli");
        #endregion

        #region Connection methods
        protected override string GetConnectProcessArguments() => throw new NotImplementedException();
        protected override string GetDisconnectProcessArguments() => throw new NotImplementedException();
        #endregion

        #region Utility methods
        private static string GetName(string str) => str;

        public static List<string> GetList()
        {
            throw new NotImplementedException();
        }

        public static Task<List<string>> GetListAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
