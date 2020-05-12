namespace GOLD.Engine
{
    internal enum LRConflict
    {
        ShiftShift = 1,
        ShiftReduce = 2,
        ReduceReduce = 3,
        AcceptReduce = 4,
        None = 5,
    }
}