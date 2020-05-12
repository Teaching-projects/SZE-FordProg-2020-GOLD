using System.Collections.Generic;

namespace GOLD.Engine
{
    internal class FAStateList : List<FAState>
    {
        public short InitialState;
        public Symbol ErrorSymbol;

        public FAStateList() : base()
        {
            InitialState = 0;
            ErrorSymbol = null;
        }

        internal FAStateList(int size) : base()
        {
            ReDimension(size);
            InitialState = 0;
            ErrorSymbol = null;
        }

        internal void ReDimension(int size)
        {
            Clear();
            for (int n = 0; n < size; n++)
            {
                Add(null);
            }
        }
    }
}