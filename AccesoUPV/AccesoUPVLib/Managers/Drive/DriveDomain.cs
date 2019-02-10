using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.Drive
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
        public string GetFullUserName(string userName)
        {
            return GetFullUserName(userName, PreferredStyle);
        }
        public string GetFullUserName(string userName, DomainStyle style)
        {
            if (style == DomainStyle.AtSignStyle)
            {
                return $"{userName}@{Name}";
            }
            else //DomainStyle.BackSlashStyle
            {
                return $"{Name}\\{userName}";
            }
        }
    }
}
