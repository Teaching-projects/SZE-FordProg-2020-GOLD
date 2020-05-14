namespace GOLD.Builder
{

    internal class LRStateBuild : LRState
    {
        public LRConfigSet ConfigSet;
        public LRConflictItemList ConflictList;
        public string Note;
        public LRStatus Status;
        public NumberSet PriorStates;
        public bool Modified;
        public bool Expanded;

        public LRConflict Add(LRAction Action)
        {
            LRAction lrAction = Action;
            SymbolBuild symbol = (SymbolBuild)lrAction.Symbol;
            ref SymbolBuild local1 = ref symbol;
            LRActionType lrActionType = Action.Type();
            ref LRActionType local2 = ref lrActionType;
            int num1 = (int)Action.Value();
            int num2 = (int)this.ActionAdd(ref local1, ref local2, (short)num1);
            lrAction.Symbol = (Symbol)symbol;
            return (LRConflict)num2;
        }

        private LRConflict ActionAdd(
          ref SymbolBuild TheSymbol,
          ref LRActionType Type,
          short Value = 0)
        {
            bool flag1 = false;
            bool flag2 = false;
            LRConflict lrConflict = LRConflict.None;
            short index = 0;
            while ((int)index < this.Count & !flag1 & !flag2)
            {
                LRAction lrAction = this[index];
                if (lrAction.Symbol.IsEqualTo((Symbol)TheSymbol))
                {
                    if (lrAction.Type() == Type & (int)lrAction.Value() == (int)Value)
                    {
                        flag2 = true;
                    }
                    else
                    {
                        lrConflict = lrAction.ConflictWith(Type);
                        flag1 = true;
                    }
                }
                checked { ++index; }
            }
            if (!flag2)
                base.Add(new LRAction((Symbol)TheSymbol, Type, Value));
            return lrConflict;
        }

        public LRConflict ConflictForAction(
          ref SymbolBuild TheSymbol,
          ref LRActionType Type,
          ref short Value)
        {
            bool flag = false;
            LRConflict lrConflict = LRConflict.None;
            short index = 0;
            while ((int)index < this.Count & !flag)
            {
                LRAction lrAction = this[index];
                if (lrAction.Symbol.IsEqualTo((Symbol)TheSymbol))
                {
                    lrConflict = !(lrAction.Type() == Type & (int)lrAction.Value() == (int)Value) ? lrAction.ConflictWith(Type) : LRConflict.None;
                    flag = true;
                }
                checked { ++index; }
            }
            return lrConflict;
        }

        public LRStateBuild()
        {
            this.ConfigSet = new LRConfigSet();
            this.ConflictList = new LRConflictItemList();
            this.PriorStates = new NumberSet(new int[0]);
            this.Modified = false;
            this.Expanded = false;
            this.Note = "";
            this.Status = LRStatus.Info;
        }
    }
}