namespace GOLD.Builder
{
    internal class LRConflictItem
    {
        public SymbolBuild Symbol;
        public LRConflict Conflict;
        public LRConfigSet Reduces;
        public LRConfigSet Shifts;

        public LRConflictItem(SymbolBuild Symbol)
        {
            this.Symbol = Symbol;
            this.Conflict = LRConflict.None;
            this.Shifts = new LRConfigSet();
            this.Reduces = new LRConfigSet();
        }

        public LRConflictItem(LRConflictItem Item, LRConflict Status)
        {
            this.Symbol = Item.Symbol;
            this.Conflict = Status;
            this.Shifts = Item.Shifts;
            this.Reduces = Item.Reduces;
        }
    }
}