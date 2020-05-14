namespace GOLD.Builder
{
    internal class LRStateBuildList : LRStateList
    {
        public LRStateBuildList()
        {
        }

        public LRStateBuildList(int Size)
          : base(Size)
        {
        }

        public LRStateBuild this[int Index]
        {
            get
            {
                return (LRStateBuild)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(ref LRStateBuild Item)
        {
            LRState lrState = (LRState)Item;
            int num = this.Add(ref lrState);
            Item = (LRStateBuild)lrState;
            return num;
        }
    }
}