namespace GOLD.Builder
{

    internal class FAStateBuildList : FAStateList
    {
        public FAStateBuildList()
        {
        }

        public FAStateBuildList(int Size)
          : base(Size)
        {
        }

        public FAStateBuild this[int Index]
        {
            get
            {
                return (FAStateBuild)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(FAStateBuild Item)
        {
            return this.Add((FAState)Item);
        }
    }
}