namespace GOLD.Builder
{

    public enum SysLogSection
    {
        Internal = -1, // 0xFFFFFFFF
        System = 0,
        Grammar = 1,
        DFA = 2,
        LALR = 3,
        CommandLine = 4
    }
}