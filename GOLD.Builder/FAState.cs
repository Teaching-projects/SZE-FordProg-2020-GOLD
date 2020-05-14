namespace GOLD.Builder
{
    internal class FAState
    {
        internal FAEdgeList EdgeList;
        private Symbol m_Accept;
        private short m_TableIndex;

        public FAState(Symbol Accept)
        {
            this.m_Accept = Accept;
            this.EdgeList = new FAEdgeList();
        }

        public FAState()
        {
            this.m_Accept = (Symbol)null;
            this.EdgeList = new FAEdgeList();
        }

        public Symbol Accept
        {
            get
            {
                return this.m_Accept;
            }
            set
            {
                this.m_Accept = value;
            }
        }

        public int AcceptIndex()
        {
            if (this.m_Accept == null)
                return -1;
            return (int)this.m_Accept.TableIndex;
        }

        public void AddEdge(FAEdge Edge)
        {
            if (Edge.Characters.Count() == 0)
            {
                this.EdgeList.Add(Edge);
            }
            else
            {
                short num1 = -1;
                short num2 = 0;
                while ((int)num2 < this.EdgeList.Count() & num1 == (short)-1)
                {
                    if (this.EdgeList[(int)num2].Target == Edge.Target)
                        num1 = num2;
                    checked { ++num2; }
                }
                if (num1 == (short)-1)
                    this.EdgeList.Add(Edge);
                else
                    this.EdgeList[(int)num1].Characters.UnionWith((NumberSet)Edge.Characters);
            }
        }

        public void AddEdge(CharacterSet CharSet, int Target)
        {
            this.AddEdge(new FAEdge(CharSet, Target));
        }

        public FAEdgeList Edges()
        {
            return this.EdgeList;
        }
    }
}