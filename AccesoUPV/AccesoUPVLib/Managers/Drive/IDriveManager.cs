namespace AccesoUPV.Library.Managers.Drive
{
    public interface IDriveManager : IConnectionManager
    {
        string Address { get; }
        string ConnectedDrive { get; }
        DriveDomain Domain { get; }
        string Drive { get; set; }
        string Password { get; set; }
        bool UseCredentials { get; set; }
        string UserName { get; set; }
        bool YesToAll { get; set; }
    }
}