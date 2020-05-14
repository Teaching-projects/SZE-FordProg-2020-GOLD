using System;
using System.Diagnostics;

namespace GOLD.Builder
{
    internal class UnicodeMapTable : DictionarySet
    {
        [DebuggerNonUserCode]
        public UnicodeMapTable()
        {
        }

        public void Add(UnicodeMapItem Item)
        {
            this.Add((DictionarySet.IMember)Item);
        }

        public void Add(int Code, int Map)
        {
            this.Add((DictionarySet.IMember)new UnicodeMapItem(Code, Map));
        }

        public UnicodeMapItem get_Item(int Index)
        {
            return (UnicodeMapItem)base[Index];
        }

        public int IndexOf(int Code)
        {
            return this.IndexOf((IComparable)Code);
        }

        public int Contains(int Code)
        {
            return -(this.Contains((IComparable)Code) ? 1 : 0);
        }
    }
}