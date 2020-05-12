using System.Collections.Generic;

namespace GOLD.Engine
{
    public class ProductionList : List<Production>
    {
        internal ProductionList() : base()
        {
        }

        internal ProductionList(int size) : base()
        {
            ReDimension(size);
        }

        internal void ReDimension(int size)
        {
            Clear();
            for (int n = 0; n < size; n++)
            {
                Add(null);
            }
        }
    }
}