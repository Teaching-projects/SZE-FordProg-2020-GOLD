namespace GOLD.Engine
{
    public enum EntryType : byte
    {
        Error = 0,
        Boolean = 66,   // B - 1 Byte, Value is 0 or 1
        Empty = 69,     // E
        UInt16 = 73,    // I - Unsigned, 2 byte
        String = 83,    // S - Unicode format
        Byte = 98,      // b
    }
}
