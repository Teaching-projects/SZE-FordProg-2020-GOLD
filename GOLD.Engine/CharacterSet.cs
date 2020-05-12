using System.Collections.Generic;

namespace GOLD.Engine
{
    internal class CharacterSet : List<CharacterRange>
    {
        public bool Contains(int charCode)
        {
            for (int i = 0; i < Count; i++)
            {
                if (charCode >= base[i].Start && charCode <= base[i].End)
                    return true;
            }

            return false;
        }
    }
}