using System.Collections;

namespace GOLD.Builder
{
    internal class RegExpSeq
    {
        private ArrayList m_Array;
        private short m_Priority;

        public RegExpSeq()
        {
            this.m_Array = new ArrayList();
        }

        public bool IsVariableLength()
        {
            bool flag = false;
            short num = 0;
            while ((int)num < this.m_Array.Count & !flag)
            {
                if (((RegExpItem)this.m_Array[(int)num]).IsVariableLength())
                    flag = true;
                checked { ++num; }
            }
            return flag;
        }

        internal short Priority
        {
            get
            {
                return this.m_Priority;
            }
            set
            {
                this.m_Priority = value;
            }
        }

        public RegExpItem this[int Index]
        {
            get
            {
                if (Index >= 0 & Index < this.m_Array.Count)
                    return (RegExpItem)this.m_Array[Index];
                return (RegExpItem)null;
            }
            set
            {
                if (!(Index >= 0 & Index < this.m_Array.Count))
                    return;
                this.m_Array[Index] = (object)value;
            }
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        public void Add(ref RegExpItem Item)
        {
            this.m_Array.Add((object)Item);
        }

        public override string ToString()
        {
            string str = "";
            if (this.m_Array.Count >= 1)
            {
                str = this.m_Array[0].ToString();
                int num = checked(this.m_Array.Count - 1);
                int index = 1;
                while (index <= num)
                {
                    str = str + " " + this.m_Array[index].ToString();
                    checked { ++index; }
                }
            }
            return str;
        }
    }
}