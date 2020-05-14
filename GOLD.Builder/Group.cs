using Microsoft.VisualBasic.CompilerServices;
using System.Collections.Generic;

namespace GOLD.Builder
{

    internal class Group
    {
        internal short TableIndex;
        internal string Name;
        internal Symbol Container;
        internal Symbol Start;
        internal Symbol End;
        internal AdvanceMode Advance;
        internal EndingMode Ending;
        internal List<int> Nesting;

        internal Group()
        {
            this.Advance = AdvanceMode.Character;
            this.Ending = EndingMode.Closed;
            this.Nesting = new List<int>();
        }

        public string AdvanceName()
        {
            switch (this.Advance)
            {
                case AdvanceMode.Token:
                    return "Token";
                case AdvanceMode.Character:
                    return "Character";
                default:
                    return "Invalid";
            }
        }

        public string EndingName()
        {
            switch (this.Ending)
            {
                case EndingMode.Open:
                    return "Open";
                case EndingMode.Closed:
                    return "Closed";
                default:
                    return "Invalid";
            }
        }
    }
}