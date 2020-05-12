using System.Collections.Generic;

namespace GOLD.Engine
{
    internal class LRStateList : List<LRState>
    {
        public short InitialState { get; set; }

        public LRStateList() : base()
        {
            InitialState = 0;
        }

        internal LRStateList(int size) : base()
        {
            ReDimension(size);
            InitialState = 0;
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