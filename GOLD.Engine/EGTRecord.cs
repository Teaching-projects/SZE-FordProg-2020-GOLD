namespace GOLD.Engine
{
    internal enum EGTRecord : byte
    {
        DFAState = 68,      // D
        InitialStates = 73, // I
        LRState = 76,       // L
        Production = 82,    // R   R for Rule (related productions)
        Symbol = 83,        // S
        CharRanges = 99,    // c
        Group = 103,        // g
        Property = 112,     // p
        TableCounts = 116,  // t   Table Counts
    }
}