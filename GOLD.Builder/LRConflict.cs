namespace GOLD.Builder
{
    internal enum LRConflict
    {
        None,
        ShiftReduce,
        ReduceReduce,
        AcceptReduce,
        AcceptShift,
    }
}