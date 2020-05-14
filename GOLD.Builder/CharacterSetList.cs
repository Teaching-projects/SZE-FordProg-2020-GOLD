using System.Collections;
namespace GOLD.Builder
{

    internal class CharacterSetList : ArrayList
    {
        public CharacterSetList()
        {
        }

        internal CharacterSetList(int Size)
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

        public CharacterSet this[int Index]
        {
            get
            {
                return (CharacterSet)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(ref CharacterSet Item)
        {
            return this.Add((object)Item);
        }
    }
}