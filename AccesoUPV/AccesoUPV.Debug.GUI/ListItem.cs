namespace AccesoUPV.Debug.GUI
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
