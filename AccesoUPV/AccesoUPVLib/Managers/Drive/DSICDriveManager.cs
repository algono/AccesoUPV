﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.Drive
{
    public class DSICDriveManager : DriveManager
    {
        public override string Address
        {
            get
            {
                return $"\\\\fileserver.dsic.upv.es\\{User}";
            }
        }

        public DSICDriveManager(string drive = null, string user = null, string password = null) : base(drive, user, password, "DSIC", true)
        {
        }
    }
}
