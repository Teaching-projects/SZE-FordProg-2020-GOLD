namespace GOLD.Builder
{
    internal class FAEdge
    {
        private CharacterSet m_Chars;
        private int m_Target;

        public FAEdge(CharacterSet CharSet, int Target)
        {
            this.m_Chars = CharSet;
            this.m_Target = Target;
        }

        public FAEdge()
        {
        }

        public CharacterSet Characters
        {
            get
            {
                return this.m_Chars;
            }
            set
            {
                this.m_Chars = value;
            }
        }

        public int Target
        {
            get
            {
                return this.m_Target;
            }
            set
            {
                this.m_Target = value;
            }
        }

        public bool Contains(ref int CharCode)
        {
            return this.Characters.Contains(CharCode);
        }
    }
}