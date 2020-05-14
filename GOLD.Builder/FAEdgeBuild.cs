namespace GOLD.Builder
{
    internal class FAEdgeBuild : FAEdge
    {
        public FAEdgeBuild(CharacterSetBuild CharSet, int Target)
          : base((CharacterSet)CharSet, Target)
        {
        }

        public CharacterSetBuild Characters
        {
            get
            {
                return (CharacterSetBuild)base.Characters;
            }
            set
            {
                this.Characters = value;
            }
        }
    }
}