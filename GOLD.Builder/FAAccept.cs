namespace GOLD.Builder
{

    internal class FAAccept
    {
        private short m_SymbolIndex;
        private short m_Priority;

        public FAAccept()
        {
            this.m_SymbolIndex = (short)-1;
        }

        public FAAccept(short SymbolIndex, short Priority)
        {
            this.m_SymbolIndex = SymbolIndex;
            this.m_Priority = Priority;
        }

        public short SymbolIndex
        {
            get
            {
                return this.m_SymbolIndex;
            }
            set
            {
                this.m_SymbolIndex = value;
            }
        }

        public short Priority
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
    }
}