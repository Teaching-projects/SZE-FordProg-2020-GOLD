using System.Collections.Generic;

namespace GOLD.Engine
{
    internal class Group
    {
        internal short TableIndex { get; set; }
        internal string Name { get; set; }
        internal Symbol Container { get; set; }
        internal Symbol Start { get; set; }
        internal Symbol End { get; set; }
        internal AdvanceMode Advance { get; set; }
        internal EndingMode Ending { get; set; }
        internal List<int> Nesting { get; set; }

        internal Group()
        {
            Advance = AdvanceMode.Character;
            Ending = EndingMode.Closed;
            Nesting = new List<int>();
        }

        public enum AdvanceMode
        {
            Token,
            Character
        }

        public enum EndingMode
        {
            Open,
            Closed
        }
    }
}