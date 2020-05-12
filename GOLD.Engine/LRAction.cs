namespace GOLD.Engine
{
    internal class LRAction
    {
        public Symbol Symbol { get; set; }
        public LRActionType Type { get; set; }
        public short Value { get; set; }

        public LRAction(Symbol symbol, LRActionType type, short value)
        {
            Symbol = symbol;
            Type = type;
            Value = value;
        }
    }
}