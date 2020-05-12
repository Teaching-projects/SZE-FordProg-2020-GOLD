using System.ComponentModel;

namespace GOLD.Engine
{
    public class Reduction : TokenList
    {

        [Description("Returns the parent production.")]
        public Production Parent { get; internal set; }
        [Description("Returns/sets any additional user-defined data to this object.")]
        public object Tag { get; set; }

        public Reduction() : base()
        {
        }

        internal Reduction(int size) : base()
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