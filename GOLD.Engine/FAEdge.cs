namespace GOLD.Engine
{
    internal class FAEdge
    {
        public CharacterSet Characters;
        public int Target;

        public FAEdge()
        {
        }

        public FAEdge(CharacterSet charSet, int target)
        {
            Characters = charSet;
            Target = target;
        }
    }
}