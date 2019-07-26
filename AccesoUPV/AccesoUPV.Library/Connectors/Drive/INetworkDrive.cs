namespace AccesoUPV.Library.Connectors.Drive
{
    public interface INetworkDrive : Connectable, Openable
    {
        string Address { get; }
        string ConnectedDrive { get; }
        DriveDomain Domain { get; }
        string Drive { get; set; }
        string Password { get; set; }
        bool UseCredentials { get; set; }
        string Username { get; set; }
        bool YesToAll { get; set; }
    }
}