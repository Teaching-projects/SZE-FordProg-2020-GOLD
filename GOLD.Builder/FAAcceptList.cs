using System.Collections;

namespace GOLD.Builder
{
    internal class FAAcceptList
    {
        private ArrayList m_Array;

        public FAAcceptList()
        {
            this.m_Array = new ArrayList();
        }

        public void Clear()
        {
            this.m_Array.Clear();
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        public FAAccept this[int Index]
        {
            get
            {
                if (Index >= 0 & Index < this.m_Array.Count)
                    return (FAAccept)this.m_Array[Index];
                return (FAAccept)null;
            }
            set
            {
                if (!(Index >= 0 & Index < this.m_Array.Count))
                    return;
                this.m_Array[Index] = (object)value;
            }
        }

        public int Add(FAAccept Item)
        {
            return this.m_Array.Add((object)Item);
        }

        public int Add(short SymbolIndex, short Priority)
        {
            return this.m_Array.Add((object)new FAAccept(SymbolIndex, Priority));
        }
    }
}