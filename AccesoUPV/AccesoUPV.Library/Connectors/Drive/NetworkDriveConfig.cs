using System;
using System.Collections.Generic;

namespace AccesoUPV.Library.Connectors.Drive
{
    public class NetworkDriveConfig<T> where T : Enum
    {
        public Func<string, DriveDomain, string> GetAddress { get; }
        public IDictionary<T, DriveDomain> Domains { get; }

        public NetworkDriveConfig(Func<string, DriveDomain, string> getAddress, IDictionary<T, DriveDomain> domains)
        {
            GetAddress = getAddress;
            Domains = domains;
        }

        public DriveDomain GetDriveDomain(T domain) => Domains[domain];
    }
}