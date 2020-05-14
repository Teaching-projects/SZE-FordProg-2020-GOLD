using System.Collections;
using System.Diagnostics;

namespace GOLD.Builder
{
    internal class LRConflictItemList : ArrayList
    {
        public LRConflictItemList()
        {
        }

        internal int Add(LRConflictItem Item)
        {
            return this.Add((object)Item);
        }

        internal LRConflictItem this[int Index]
        {
            get
            {
                return (LRConflictItem)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }
    }
}