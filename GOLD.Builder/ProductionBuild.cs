namespace GOLD.Builder
{
    internal class ProductionBuild : Production
    {
        private short MyPriority;

        public ProductionBuild()
        {
            this.MyHandle = (SymbolList)new SymbolBuildList();
        }

        internal ProductionBuild(SymbolBuild Head, short TableIndex)
        {
            this.MyHandle = (SymbolList)new SymbolBuildList();
            this.MyHead = (Symbol)Head;
            this.MyTableIndex = TableIndex;
        }

        internal short Priority
        {
            get
            {
                return this.MyPriority;
            }
            set
            {
                this.MyPriority = value;
            }
        }

        internal SymbolBuildList Handle()
        {
            return (SymbolBuildList)base.Handle();
        }

        internal SymbolBuild Head
        {
            get
            {
                return (SymbolBuild)this.MyHead;
            }
            set
            {
                this.MyHead = (Symbol)value;
            }
        }
    }
}