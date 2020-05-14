namespace GOLD.Builder
{

    internal class GroupBuild : Group
    {
        internal string NestingNames;
        internal string ContainerName;
        internal bool IsBlock;

        internal GroupBuild()
        {
            this.IsBlock = false;
        }

        internal GroupBuild(string Name, bool IsBlock)
        {
            this.Name = Name;
            this.NestingNames = "None";
            this.Advance = AdvanceMode.Character;
            this.Ending = EndingMode.Closed;
            this.IsBlock = IsBlock;
        }

        internal GroupBuild(
          string Name,
          SymbolBuild Container,
          SymbolBuild Start,
          SymbolBuild End,
          EndingMode Mode)
        {
            this.Name = Name;
            this.Container = (Symbol)Container;
            this.Start = (Symbol)Start;
            this.End = (Symbol)End;
            this.Ending = Mode;
            this.IsBlock = false;
            this.NestingNames = "None";
            this.Advance = AdvanceMode.Character;
        }

        internal GroupBuild(string Name, bool IsBlock, EndingMode Ending)
        {
            this.Name = Name;
            this.IsBlock = IsBlock;
            this.NestingNames = "None";
            this.Advance = AdvanceMode.Character;
            this.Ending = Ending;
        }
    }
}