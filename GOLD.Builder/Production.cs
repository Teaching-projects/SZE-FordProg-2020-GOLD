using Microsoft.VisualBasic;
namespace GOLD.Builder
{

    public class Production
    {
        protected Symbol MyHead;
        protected SymbolList MyHandle;
        protected short MyTableIndex;

        internal Production()
        {
            this.MyHandle = new SymbolList();
            this.MyTableIndex = (short)-1;
        }

        internal Production(Symbol Head)
        {
            this.MyHead = Head;
            this.MyHandle = new SymbolList();
            this.MyTableIndex = (short)-1;
        }

        internal Production(Symbol Head, short TableIndex)
        {
            this.MyHead = Head;
            this.MyHandle = new SymbolList();
            this.MyTableIndex = TableIndex;
        }

        public Symbol Head
        {
            get
            {
                return this.MyHead;
            }
        }

        public SymbolList Handle()
        {
            return this.MyHandle;
        }

        public string Text()
        {
            return this.Name() + " ::= " + this.Definition();
        }

        public short TableIndex
        {
            get
            {
                return this.MyTableIndex;
            }
        }

        internal void SetTableIndex(short Value)
        {
            this.MyTableIndex = Value;
        }

        public override string ToString()
        {
            return this.Text();
        }

        internal bool ContainsOneNonTerminal()
        {
            return this.MyHandle.Count() == 1 && this.MyHandle[0].Type == SymbolType.Nonterminal;
        }

        internal string Definition()
        {
            string str = "";
            short num1 = checked((short)(this.MyHandle.Count() - 1));
            short num2 = 0;
            while ((int)num2 <= (int)num1)
            {
                str = str + this.MyHandle[(int)num2].Text(false) + " ";
                checked { ++num2; }
            }
            return Strings.RTrim(str);
        }

        internal bool Equals(ref Production SecondRule)
        {
            bool flag;
            if (this.MyHandle.Count() == SecondRule.Handle().Count() & this.MyHead.IsEqualTo(SecondRule.Head))
            {
                flag = true;
                short num = 0;
                while (flag & (int)num < this.MyHandle.Count())
                {
                    flag = this.MyHandle[(int)num].IsEqualTo(SecondRule.Handle()[(int)num]);
                    checked { ++num; }
                }
            }
            else
                flag = false;
            return flag;
        }

        internal string Name()
        {
            return "<" + this.MyHead.Name + ">";
        }

        internal void SetHead(Symbol NonTerminal)
        {
            this.MyHead = NonTerminal;
        }

        internal void SetHandle(ref SymbolList Symbols)
        {
            this.MyHandle = Symbols;
        }
    }
}