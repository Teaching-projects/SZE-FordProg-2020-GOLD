using System.Collections;

namespace GOLD.Builder
{
    internal class LRStateList : ArrayList
    {
        public short InitialState;

        public LRStateList()
        {
        }

        internal LRStateList(int Size)
        {
            this.ReDimension(Size);
        }

        internal void ReDimension(int Size)
        {
            this.Clear();
            int num1 = checked(Size - 1);
            int num2 = 0;
            while (num2 <= num1)
            {
                this.Add((object)null);
                checked { ++num2; }
            }
        }

        public LRState this[int Index]
        {
            get
            {
                return (LRState)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(ref LRState Item)
        {
            return this.Add((object)Item);
        }
    }
}