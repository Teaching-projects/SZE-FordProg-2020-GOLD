namespace GOLD.Builder
{

    internal class ProductionBuildList : ProductionList
    {
        public ProductionBuildList()
        {
        }

        public ProductionBuildList(int Size)
          : base(Size)
        {
        }

        public ProductionBuild this[int Index]
        {
            get
            {
                return (ProductionBuild)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(ref ProductionBuild Item)
        {
            return this.Add((Production)Item);
        }
    }
}