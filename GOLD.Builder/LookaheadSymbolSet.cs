using System;
namespace GOLD.Builder
{

    internal class LookaheadSymbolSet : DictionarySet
    {
        public LookaheadSymbolSet()
        {
        }

        public LookaheadSymbolSet(LookaheadSymbolSet A, LookaheadSymbolSet B)
          : base((DictionarySet)A, (DictionarySet)B)
        {
        }

        public bool Add(LookaheadSymbol Item)
        {
            return Add((DictionarySet.IMember)Item);
        }

        public LookaheadSymbol this[int Index]
        {
            get
            {
                return (LookaheadSymbol)base[Index];
            }
        }

        public bool UnionWith(LookaheadSymbolSet SetB)
        {
            return this.UnionWith((DictionarySet)SetB);
        }

        public LookaheadSymbol ByKey(short TableIndex)
        {
            return (LookaheadSymbol)this.get_ByKey((IComparable)TableIndex);
        }

        public LookaheadSymbolSet Union(LookaheadSymbolSet SetB)
        {
            return new LookaheadSymbolSet(this, SetB);
        }

        public string Text()
        {
            string str = "";
            if (this.Count() >= 1)
            {
                str = this[0].Parent.Text(false);
                short num1 = checked((short)(this.Count() - 1));
                short num2 = 1;
                while ((int)num2 <= (int)num1)
                {
                    str = str + " " + this[(int)num2].Parent.Text(false);
                    checked { ++num2; }
                }
            }
            return str;
        }
    }
}