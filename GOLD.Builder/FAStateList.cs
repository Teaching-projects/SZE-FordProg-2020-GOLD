using System.Collections;
namespace GOLD.Builder
{

    internal class FAStateList : ArrayList
    {
        public short InitialState;
        public Symbol ErrorSymbol;
        public Symbol EndSymbol;

        public FAStateList()
        {
        }

        internal FAStateList(int Size)
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

        public FAState this[int Index]
        {
            get
            {
                return (FAState)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(FAState Item)
        {
            return this.Add((object)Item);
        }
    }
}