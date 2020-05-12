using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace GOLD.Engine
{
    public class SymbolList : List<Symbol>
    {
        internal SymbolList() : base()
        {
        }

        internal SymbolList(int size) : base()
        {
            ReDimension(size);
        }

        internal void ReDimension(int size)
        {
            Clear();
            for (int n = 0; n < size; n++)
            {
                Add(null);
            }
        }

        [Description("Returns the symbol with the specified index.")]
        public Symbol this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    return null;

                return base[index];
            }

            internal set
            {
                base[index] = value;
            }
        }

        internal Symbol GetFirstOrDefaultType(SymbolType type)
        {
            for (int i = 0; i < Count; i++)
            {
                if (base[i].Type == type)
                    return base[i];
            }

            return null;
        }

        public override string ToString()
        {
            return ToBNFString();
        }

        [Description("Returns a list of the symbol names in BNF format.")]
        public string ToBNFString(string separator, bool alwaysDelimitTerminals)
        {
            string result = "";
            for (int i = 0; i < Count; i++)
            {
                Symbol symbol = base[i];
                result += (i == 0 ? string.Empty : separator) + symbol.ToBNFString(alwaysDelimitTerminals);
            }
            return result;
        }

        [Description("Returns a list of the symbol names in BNF format.")]
        public string ToBNFString()
        {
            return ToBNFString(", ", false);
        }
    }
}