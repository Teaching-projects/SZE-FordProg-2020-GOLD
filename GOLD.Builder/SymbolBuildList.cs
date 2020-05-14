using Microsoft.VisualBasic.CompilerServices;
namespace GOLD.Builder
{

    internal class SymbolBuildList : SymbolList
    {
        public SymbolBuildList()
        {
        }

        public SymbolBuildList(int Size)
          : base(Size)
        {
        }

        public SymbolBuild this[int Index]
        {
            get
            {
                return (SymbolBuild)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public SymbolBuild AddUnique(SymbolBuild Item)
        {
            short num = this.ItemIndex(Item);
            if (num == (short)-1)
            {
                num = checked((short)this.Add((Symbol)Item));
            }
            else
            {
                SymbolBuild symbolBuild = (SymbolBuild)base[(int)num];
                if (symbolBuild.RegularExp == null)
                {
                    symbolBuild.RegularExp = Item.RegularExp;
                    symbolBuild.Type = Item.Type;
                }
            }
            return (SymbolBuild)base[(int)num];
        }

        public int Add(SymbolBuild Item)
        {
            return Add((Symbol)Item);
        }

        public new string ToString()
        {
            return this.Text(", ", false);
        }

        internal short ItemIndex(SymbolBuild Search)
        {
            return this.ItemIndex(Search.Name, Search.Type);
        }

        internal short ItemIndex(string Name, SymbolType Type)
        {
            int num = -1;
            int index = 0;
            while (index < this.Count() & num == -1)
            {
                SymbolBuild symbolBuild = (SymbolBuild)base[index];
                if (Operators.CompareString(symbolBuild.Name.ToUpper(), Name.ToUpper(), true) == 0 & symbolBuild.Type == Type)
                    num = index;
                checked { ++index; }
            }
            return checked((short)num);
        }

        internal short ItemIndex(string Name)
        {
            int num = -1;
            int index = 0;
            while (index < this.Count() & num == -1)
            {
                if (Operators.CompareString(base[index].Name.ToUpper(), Name.ToUpper(), true) == 0)
                    num = index;
                checked { ++index; }
            }
            return checked((short)num);
        }

        internal short ItemIndexCategory(string Name, SymbolCategory Category)
        {
            int num = -1;
            int index = 0;
            while (index < this.Count() & num == -1)
            {
                SymbolBuild symbolBuild = (SymbolBuild)base[index];
                if (Operators.CompareString(symbolBuild.Name.ToUpper(), Name.ToUpper(), true) == 0 & symbolBuild.Category() == Category)
                    num = index;
                checked { ++index; }
            }
            return checked((short)num);
        }

        internal short TerminalIndex(string Name)
        {
            int num = -1;
            int index = 0;
            while (index < this.Count() & num == -1)
            {
                Symbol symbol = base[index];
                if (Operators.CompareString(symbol.Name.ToUpper(), Name.ToUpper(), true) == 0 & symbol.Type != SymbolType.Nonterminal)
                    num = index;
                checked { ++index; }
            }
            return checked((short)num);
        }

        internal short NonterminalIndex(string Name)
        {
            int num = -1;
            int index = 0;
            while (index < this.Count() & num == -1)
            {
                Symbol symbol = base[index];
                if (Operators.CompareString(symbol.Name.ToUpper(), Name.ToUpper(), true) == 0 & symbol.Type == SymbolType.Nonterminal)
                    num = index;
                checked { ++index; }
            }
            return checked((short)num);
        }

        internal bool Contains(string Name)
        {
            return this.ItemIndex(Name) != (short)-1;
        }

        public string Names(string Separator = ", ", string NamePrefix = "'", string NamePostfix = "'")
        {
            string str = "";
            if (this.Count() >= 1)
            {
                str = base[0].Name;
                int num = checked(this.Count() - 1);
                int index = 1;
                while (index <= num)
                {
                    str = str + Separator + base[index].Name;
                    checked { ++index; }
                }
            }
            return str;
        }
    }
}