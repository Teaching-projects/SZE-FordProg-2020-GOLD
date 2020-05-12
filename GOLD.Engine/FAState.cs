namespace GOLD.Engine
{
    internal class FAState
    {
        public FAEdgeList Edges;
        public Symbol Accept;

        public FAState(Symbol accept)
        {
            Accept = accept;
            Edges = new FAEdgeList();
        }

        public FAState()
        {
            Accept = null;
            Edges = new FAEdgeList();
        }
    }
}