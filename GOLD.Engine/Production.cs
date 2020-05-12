using System.ComponentModel;

namespace GOLD.Engine
{
    public class Production
    {
        [Description("Returns the head of the production.")]
        public Symbol Head { get; }
        [Description("Returns the symbol list containing the handle (body) of the production.")]
        public SymbolList Handle { get; }
        [Description("Returns the index of the production in the Production Table.")]
        public short TableIndex { get; }

        internal Production()
        {
        }

        internal Production(Symbol head, short tableIndex)
        {
            Head = head;
            TableIndex = tableIndex;
            Handle = new SymbolList();
        }

        public override string ToString()
        {
            return Text(false);
        }

        [Description("Returns the production in BNF.")]
        public string Text(bool alwaysDelimitTerminals = false)
        {
            return Head.ToBNFString() + " ::= " + Handle.ToBNFString(" ", alwaysDelimitTerminals);
        }

        internal bool ContainsOneNonTerminal()
        {
            if (Handle.Count == 1 && Handle[0].Type == SymbolType.Nonterminal)
                return true;

            return false;
        }
    }
}