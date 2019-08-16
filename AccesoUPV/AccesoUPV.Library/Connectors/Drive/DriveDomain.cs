namespace AccesoUPV.Library.Connectors.Drive
{
    public enum DomainStyle { BackSlashStyle, AtSignStyle }
    public class DriveDomain
    {
        public string Name { get; }
        public string Folder { get; }
        public DomainStyle PreferredStyle { get; }

        public DriveDomain(string name, DomainStyle style = DomainStyle.BackSlashStyle, string folder = null)
        {
            Name = name;
            PreferredStyle = style;
            Folder = folder;
        }
        public string GetFullUsername(string userName) => GetFullUserName(userName, PreferredStyle);
        public string GetFullUserName(string userName, DomainStyle style)
        {
            switch (style)
            {
                case DomainStyle.AtSignStyle:
                    return $"{userName}@{Name}";
                case DomainStyle.BackSlashStyle:
                    return $@"{Name}\{userName}";
                default:
                    return null;
            }
        }
    }
}
