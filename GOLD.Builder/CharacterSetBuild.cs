namespace GOLD.Builder
{
    internal class CharacterSetBuild : CharacterSet
    {
        public CharacterSetBuild()
        {
        }

        public CharacterSetBuild(string CharSet) : base(CharSet)
        {
        }

        public CharacterSetBuild(CharacterSet CharSet) : base((NumberSet)CharSet)
        {
        }
    }
}