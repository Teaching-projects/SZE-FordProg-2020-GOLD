using System.Collections;
namespace GOLD.Builder
{

    internal class GroupList : ArrayList
    {
        public GroupList()
        {
        }

        internal GroupList(int Size)
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

        public Group this[int Index]
        {
            get
            {
                return (Group)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(Group Item)
        {
            return this.Add((object)Item);
        }
    }
}