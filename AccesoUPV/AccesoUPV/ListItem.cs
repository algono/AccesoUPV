using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.GUI
{
    public class ListItem
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public ListItem(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => Name;
    }
}
