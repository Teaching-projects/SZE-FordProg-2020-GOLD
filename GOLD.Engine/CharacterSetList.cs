using System.Collections.Generic;

namespace GOLD.Engine
{
    internal class CharacterSetList : List<CharacterSet>
    {
        public CharacterSetList() : base()
        {
        }

        internal CharacterSetList(int size) : base()
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