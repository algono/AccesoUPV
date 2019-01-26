using AccesoUPV.Lib.Managers.VPN;
using System;

namespace AccesoUPVTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            VPNManager Manager = DSICManager.Create("DSIC");

            Console.WriteLine("Checking if the VPN exists...");
            if (Manager.Exists()) Console.WriteLine("The VPN already exists. Next step.");
            else CreateTest(Manager);
            ConnectTest(Manager);
            DisconnectTest(Manager);
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void CreateTest(VPNManager Manager)
        {
            Console.WriteLine("Press any key to create...");
            Console.ReadKey();
            Console.WriteLine("Creating...");
            Manager.Create();
        }
        static void ConnectTest(VPNManager Manager)
        {
            Console.WriteLine("Reachable before connecting: {0}", Manager.IsReachable());
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Connecting...");
            Manager.Connect();
            Console.WriteLine("Reachable after connecting: {0}", Manager.IsReachable(4000));
        }
        static void DisconnectTest(VPNManager Manager)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Console.WriteLine("Disonnecting...");
            Manager.Disconnect();
            Console.WriteLine("Reachable after disconnecting: {0}", Manager.IsReachable());
        }
    }
}
