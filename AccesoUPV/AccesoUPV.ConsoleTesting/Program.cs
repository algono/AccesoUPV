using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Static;
using System;
using System.Text;

namespace AccesoUPV.ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            // Service test calls
            IAccesoUPVService service = ServiceTest();

            // VPN test calls

            service.VPN_UPV.SetNameAuto();
            VPNTest(service.VPN_UPV);

            service.VPN_DSIC.SetNameAuto();
            VPNTest(service.VPN_DSIC);

            // Drive test calls

            Console.WriteLine("Press any key to start Drive testing...");
            Console.ReadKey();
            Console.Clear();

            try
            {
                if (!service.VPN_UPV.IsReachable()) ConnectTest(service.VPN_UPV);

                Console.Write("Type your user: ");
                service.User = Console.ReadLine();
                Console.Write("Type the drive (i.e: 'C:'): ");
                service.WDrive.Drive = Console.ReadLine();
                Console.WriteLine("Type your domain:");
                foreach (var value in Enum.GetValues(typeof(UPVDomain)))
                {
                    Console.WriteLine($"{(int)value} - {(UPVDomain)value}");
                }
                Console.Write("Type here: ");
                service.WDrive.Domain = (UPVDomain)int.Parse(Console.ReadLine());

                DriveTest(service.WDrive);

                try
                {
                    Console.Write("Type your DSIC password: ");
                    service.DSICDrive.Password = ReadPassword();
                    DriveTest(service.DSICDrive);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("DSIC Drive testing was canceled by the user.");
                };

                // Remote desktop test

                Console.WriteLine("Connecting to Linux Desktop...");
                RemoteDesktop.ConnectToLinuxDesktop();

                Console.WriteLine("Press any key when the Linux Desktop is closed");
                Console.ReadKey();

                Console.WriteLine("Connecting to Windows Desktop...");
                RemoteDesktop.ConnectToWindowsDesktop();

                Console.WriteLine("Press any key when the Windows Desktop is closed");
                Console.ReadKey();

                if (service.VPN_UPV.Connected) DisconnectTest(service.VPN_UPV);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The VPN connection process was canceled by the user.\nDrive testing ended.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static string ReadPassword()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Escape) throw new OperationCanceledException();
                if (key.Key == ConsoleKey.Backspace) { if (input.Length > 0) input.Remove(input.Length - 1, 1); }
                else input.Append(key.KeyChar);
            }
            return input.ToString();
        }

        static IAccesoUPVService ServiceTest()
        {
            Console.WriteLine("Creating Service...");
            IAccesoUPVService service = new AccesoUPVService();
            Console.WriteLine("Service created.");

            if (service.AreUninitializedSettings)
            {
                Console.WriteLine("--------- SERVICE TEST ---------");

                Console.WriteLine("There are uninitialized settings. These are:");

                StringBuilder builder = new StringBuilder();
                foreach (System.Configuration.SettingsPropertyValue setting in service.UninitializedSettings) // Loop through all strings
                {
                    builder.Append(" | ").Append(setting.Name.ToString()); // Append string to StringBuilder
                }
                Console.WriteLine(builder.ToString()); // Get string from StringBuilder

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

                Console.WriteLine("--------- SERVICE TEST ENDED ---------");
            }
            return service;
        }

        //VPN tests
        static void VPNTest(VPN vpn)
        {
            Console.WriteLine("--------- VPN TEST ({0}) ---------", vpn.Name);

            Console.WriteLine("Checking if the VPN exists...");
            if (vpn.Exists()) Console.WriteLine("The VPN already exists. Next step.");
            else CreateTest(vpn);
            try
            {
                ConnectTest(vpn);
                DisconnectTest(vpn);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The VPN connection process was canceled by the user.");
            }

            Console.WriteLine("--------- VPN TEST ENDED ---------");
        }

        static void CreateTest(VPN vpn)
        {
            Console.WriteLine("Press any key to create...");
            Console.ReadKey();
            Console.WriteLine("Creating...");
            Console.WriteLine("Created: {0}", vpn.Create());
        }
        static void ConnectTest(VPN vpn)
        {
            Console.WriteLine("Reachable before connecting: {0}", vpn.IsReachable());
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Connecting...");
            vpn.Connect();
            Console.WriteLine("Connected: {0}", vpn.Connected);
            Console.WriteLine("Reachable after connecting: {0}", vpn.IsReachable());
        }
        static void DisconnectTest(VPN vpn)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Console.WriteLine("Disconnecting...");
            vpn.Disconnect();
            Console.WriteLine("Disconnected: {0}", !vpn.Connected);
            Console.WriteLine("Reachable after disconnecting: {0}", vpn.IsReachable());
        }

        //Drive tests
        static void DriveTest(INetworkDrive drive)
        {
            Console.WriteLine("--------- DRIVE TEST ({0}) ---------", drive.Address);

            ConnectTest(drive);
            DisconnectTest(drive);

            Console.WriteLine("--------- DRIVE TEST ENDED ---------");

        }
        static void ConnectTest(INetworkDrive drive)
        {
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Connecting...");
            drive.Connect();
            Console.WriteLine("Connected: {0}", drive.Connected);
        }
        static void DisconnectTest(INetworkDrive drive)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Console.WriteLine("Disconnecting...");
            drive.Disconnect();
            Console.WriteLine("Disconnected: {0}", !drive.Connected);
        }
    }
}
