using AccesoUPV.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPVTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            VPNManager Manager = new UPVManager("Test");

            CreateTest(Manager);
            ConnectTest(Manager);
            DisconnectTest(Manager);
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void CreateTest(VPNManager Manager)
        {
            Console.WriteLine("Press any key to create...");
            Console.ReadKey();
            Manager.Create();
        }
        static void ConnectTest(VPNManager Manager)
        {
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Reachable before connecting: {0}", Manager.IsReachable());
            Manager.Connect();
            Console.WriteLine("Reachable after connecting: {0}", Manager.IsReachable(4000));
        }
        static void DisconnectTest(VPNManager Manager)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Manager.Disconnect();
            Console.WriteLine("Reachable after disconnecting: {0}", Manager.IsReachable());
        }
    }
}
