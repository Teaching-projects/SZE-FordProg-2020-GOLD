using System.Collections;
namespace GOLD.Builder
{

    public class ProductionList
    {
        private ArrayList m_Array;

        internal ProductionList()
        {
            this.m_Array = new ArrayList();
        }

        internal ProductionList(int Size)
        {
            this.m_Array = new ArrayList();
            this.ReDimension(Size);
        }

        internal void ReDimension(int Size)
        {
            this.m_Array.Clear();
            int num1 = checked(Size - 1);
            int num2 = 0;
            while (num2 <= num1)
            {
                this.m_Array.Add((object)null);
                checked { ++num2; }
            }
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        internal void Clear()
        {
            this.m_Array.Clear();
        }

        public Production this[int Index]
        {
            get
            {
                return (Production)this.m_Array[Index];
            }
            set
            {
                this.m_Array[Index] = (object)value;
            }
        }

        public int Add(Production Item)
        {
            return this.m_Array.Add((object)Item);
        }
    }
}