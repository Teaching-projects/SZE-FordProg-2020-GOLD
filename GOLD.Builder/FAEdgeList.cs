using System.Collections;

namespace GOLD.Builder
{
    internal class FAEdgeList
    {
        private ArrayList m_List;

        public FAEdgeList()
        {
            this.m_List = new ArrayList();
        }

        public FAEdge this[int Index]
        {
            get
            {
                return (FAEdge)this.m_List[Index];
            }
            set
            {
                this.m_List[Index] = (object)value;
            }
        }

        public int Add(FAEdge Edge)
        {
            return this.m_List.Add((object)Edge);
        }

        public int Count()
        {
            return this.m_List.Count;
        }
    }
}