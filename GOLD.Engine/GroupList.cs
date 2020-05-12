using System.Collections.Generic;

namespace GOLD.Engine
{
    internal class GroupList : List<Group>
    {
        public GroupList() : base()
        {
        }

        internal GroupList(int size) : base()
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