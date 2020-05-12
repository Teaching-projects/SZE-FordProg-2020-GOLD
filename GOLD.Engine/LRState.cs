using System.Collections.Generic;

namespace GOLD.Engine
{
    internal class LRState : List<LRAction>
    {
        // Returns the index of SymbolIndex in the table, -1 if not found
        public short IndexOf(Symbol item)
        {
            for (short i = 0; i < Count; i++)
            {
                if (item.Equals(base[i].Symbol))
                    return i;
            }

            return -1;
        }

        public LRAction this[short index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        public LRAction this[Symbol sym]
        {
            get
            {
                int index = IndexOf(sym);
                if (index == -1)
                    return null;

                return base[index];
            }
            set
            {
                int index = IndexOf(sym);
                if (index == -1)
                    return;
                base[index] = value;
            }
        }
    }
}