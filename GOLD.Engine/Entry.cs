namespace GOLD.Engine
{
    public class Entry
    {
        public EntryType Type;
        public object Value;

        public Entry()
        {
            Type = EntryType.Empty;
            Value = string.Empty;
        }

        public Entry(EntryType type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}
