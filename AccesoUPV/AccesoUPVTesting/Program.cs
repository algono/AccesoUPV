﻿using AccesoUPV.Lib.Services;
using AccesoUPV.Lib.Managers.Drive;
using AccesoUPV.Lib.Managers.VPN;
using System;

namespace AccesoUPVTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //Service test calls

            Console.WriteLine("Creating Service...");
            AccesoUPVService Service = new AccesoUPVService();
            Console.WriteLine("Service created.");

            ////VPN test calls

            //Service.VPN_UPV.Name = "UPV";
            //VPNTest(Service.VPN_UPV);

            //Service.VPN_DSIC.Name = "DSIC";
            //VPNTest(Service.VPN_DSIC);

            if (!Service.VPN_UPV.IsReachable()) ConnectTest(Service.VPN_UPV);

            //Drive test calls

            Service.User = "algono";

            Service.WDrive.Drive = "W:";
            Service.WDrive.Domain = UPVDomain.Alumno;
            DriveTest(Service.WDrive);

            Service.DSICDrive.Password = "INSERT PASSWORD HERE";
            DriveTest(Service.DSICDrive);

            //Remote desktop test

            //Console.WriteLine("Connecting to Linux Desktop...");
            //AccesoUPVService.ConnectToLinuxDesktop();

            if (Service.VPN_UPV.Connected) DisconnectTest(Service.VPN_UPV);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        //VPN tests
        static void VPNTest(VPNManagerBase Manager)
        {
            Console.WriteLine("--------- VPN TEST ({0}) ---------", Manager.Name);

            Console.WriteLine("Checking if the VPN exists...");
            if (Manager.Exists()) Console.WriteLine("The VPN already exists. Next step.");
            else CreateTest(Manager);
            try
            {
                ConnectTest(Manager);
                DisconnectTest(Manager);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The VPN connection process was canceled by the user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("--------- VPN TEST ENDED ---------");
        }

        static void CreateTest(VPNManagerBase Manager)
        {
            Console.WriteLine("Press any key to create...");
            Console.ReadKey();
            Console.WriteLine("Creating...");
            Console.WriteLine("Created: {0}", Manager.Create());
        }
        static void ConnectTest(VPNManagerBase Manager)
        {
            Console.WriteLine("Reachable before connecting: {0}", Manager.IsReachable());
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Connecting...");
            Manager.Connect();
            Console.WriteLine("Connected: {0}", Manager.Connected);
            Console.WriteLine("Reachable after connecting: {0}", Manager.IsReachable(VPNManagerBase.TEST_PING_TIMEOUT));
        }
        static void DisconnectTest(VPNManagerBase Manager)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Console.WriteLine("Disconnecting...");
            Manager.Disconnect();
            Console.WriteLine("Disconnected: {0}", !Manager.Connected);
            Console.WriteLine("Reachable after disconnecting: {0}", Manager.IsReachable());
        }

        //Drive tests
        static void DriveTest(DriveManagerBase Manager)
        {
            Console.WriteLine("--------- DRIVE TEST ({0}) ---------", Manager.Address);

            try
            {
                ConnectTest(Manager);
                DisconnectTest(Manager);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("--------- DRIVE TEST ENDED ---------");

        }
        static void ConnectTest(DriveManagerBase Manager)
        {
            Console.WriteLine("Press any key to connect...");
            Console.ReadKey();
            Console.WriteLine("Connecting...");
            Manager.Connect();
            Console.WriteLine("Connected: {0}", Manager.Connected);
        }
        static void DisconnectTest(DriveManagerBase Manager)
        {
            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();
            Console.WriteLine("Disconnecting...");
            Manager.Disconnect();
            Console.WriteLine("Disconnected: {0}", !Manager.Connected);
        }
    }
}