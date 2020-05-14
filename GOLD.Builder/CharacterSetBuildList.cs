namespace GOLD.Builder
{
    internal class CharacterSetBuildList : CharacterSetList
    {
        public CharacterSetBuildList(int Size)
          : base(Size)
        {
        }

        public CharacterSetBuildList()
        {
        }

        public CharacterSetBuild this[int Index]
        {
            get
            {
                return (CharacterSetBuild)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(ref CharacterSetBuild Item)
        {
            CharacterSet characterSet = (CharacterSet)Item;
            int num = this.Add(ref characterSet);
            Item = (CharacterSetBuild)characterSet;
            return num;
        }
    }
}