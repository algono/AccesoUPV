using AccesoUPV.Lib.Managers.Drive;
using AccesoUPV.Lib.Managers.VPN;
using System;

namespace AccesoUPVTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //VPNTest();
            DriveTest();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        //Drive tests
        static void DriveTest()
        {
            DriveManager Manager = new WDriveManager("W:", "algono");
            ConnectTest(Manager);
            DisconnectTest(Manager);

        }
        static void ConnectTest(DriveManager Manager)
        {
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Connecting...");
            Console.WriteLine($"Connected = {Manager.Connect()}");
        }
        static void DisconnectTest(DriveManager Manager)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Console.WriteLine("Disconnecting...");
            Console.WriteLine($"Disconnected = {Manager.Disconnect()}");
        }
        //VPN tests
        static void VPNTest()
        {
            VPNManager Manager = DSICManager.Create("DSIC");

            Console.WriteLine("Checking if the VPN exists...");
            if (Manager.Exists()) Console.WriteLine("The VPN already exists. Next step.");
            else CreateTest(Manager);
            ConnectTest(Manager);
            DisconnectTest(Manager);
        }

        static void CreateTest(VPNManager Manager)
        {
            Console.WriteLine("Press any key to create...");
            Console.ReadKey();
            Console.WriteLine("Creating...");
            Console.WriteLine($"Created = {Manager.Create()}");
        }
        static void ConnectTest(VPNManager Manager)
        {
            Console.WriteLine("Reachable before connecting: {0}", Manager.IsReachable());
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Connecting...");
            Console.WriteLine($"Connected = {Manager.Connect()}");
            Console.WriteLine("Reachable after connecting: {0}", Manager.IsReachable(4000));
        }
        static void DisconnectTest(VPNManager Manager)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Console.WriteLine("Disonnecting...");
            Console.WriteLine($"Disconnected = {Manager.Disconnect()}");
            Console.WriteLine("Reachable after disconnecting: {0}", Manager.IsReachable());
        }
    }
}
