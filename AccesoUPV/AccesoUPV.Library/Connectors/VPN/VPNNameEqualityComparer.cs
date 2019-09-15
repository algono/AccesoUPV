using System;
using System.Collections.Generic;

namespace AccesoUPV.Library.Connectors.VPN
{
    public class VPNNameEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode(string obj) => obj.GetHashCode();
    }
}
