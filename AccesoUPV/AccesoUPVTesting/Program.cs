using AccesoUPV.Lib.Managers.Drive;
using AccesoUPV.Lib.Managers.VPN;
using System;

namespace AccesoUPVTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //VPN test calls

            //VPNTest(UPVManager.Create("UPV"));
            //VPNTest(DSICManager.Create("DSIC"));

            //ConnectTest(UPVManager.Create("UPV"));

            //Drive test calls

            //DriveManager WManager = new WDriveManager("W:", "algono");
            //DriveTest(WManager);
            //DriveManager DSICManager = new DSICDriveManager("W:", "algono", "INSERT PASS HERE");
            //DriveTest(DSICManager);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        //Drive tests
        static void DriveTest(DriveManager Manager)
        {
            Console.WriteLine("--------- DRIVE TEST ({0}) ---------", Manager.Address);

            ConnectTest(Manager);
            DisconnectTest(Manager);

            Console.WriteLine("--------- DRIVE TEST ENDED ---------");

        }
        static void ConnectTest(DriveManager Manager)
        {
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Connecting...");
            Console.WriteLine("Connected: {0}", Manager.Connect());
        }
        static void DisconnectTest(DriveManager Manager)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Console.WriteLine("Disconnecting...");
            Console.WriteLine("Disconnected: {0}", Manager.Disconnect());
        }
        //VPN tests
        static void VPNTest(VPNManager Manager)
        {
            Console.WriteLine("--------- VPN TEST ({0}) ---------", Manager.Name);

            Console.WriteLine("Checking if the VPN exists...");
            if (Manager.Exists()) Console.WriteLine("The VPN already exists. Next step.");
            else CreateTest(Manager);
            ConnectTest(Manager);
            DisconnectTest(Manager);

            Console.WriteLine("--------- VPN TEST ENDED ---------");
        }

        static void CreateTest(VPNManager Manager)
        {
            Console.WriteLine("Press any key to create...");
            Console.ReadKey();
            Console.WriteLine("Creating...");
            Console.WriteLine("Created: {0}", Manager.Create());
        }
        static void ConnectTest(VPNManager Manager)
        {
            Console.WriteLine("Reachable before connecting: {0}", Manager.IsReachable());
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Connecting...");
            Console.WriteLine("Connected: {0}", Manager.Connect());
            Console.WriteLine("Reachable after connecting: {0}", Manager.IsReachable(4000));
        }
        static void DisconnectTest(VPNManager Manager)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Console.WriteLine("Disconnecting...");
            Console.WriteLine("Disconnected: {0}", Manager.Disconnect());
            Console.WriteLine("Reachable after disconnecting: {0}", Manager.IsReachable());
        }
    }
}
