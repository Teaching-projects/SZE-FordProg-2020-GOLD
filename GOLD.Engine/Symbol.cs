using System.ComponentModel;

namespace GOLD.Engine
{
    public class Symbol
    {
        [Description("Returns the type of the symbol.")]
        public SymbolType Type { get; internal set; }
        [Description("Returns the index of the symbol in the Symbol Table,")]
        public short TableIndex { get; }
        [Description("Returns the name of the symbol.")]
        public string Name { get; }

        internal Group Group;

        internal Symbol()
        {
        }

        internal Symbol(string name, SymbolType type, short tableIndex)
        {
            Name = name;
            Type = type;
            TableIndex = tableIndex;
        }

        private string LiteralFormat(string source, bool forceDelimit)
        {
            if (source.Equals("'"))
                return "''";

            short n = 0;
            while (n < source.Length & !forceDelimit)
            {
                char ch = source[n];
                forceDelimit = !(char.IsLetter(ch) || ch.Equals('.') || ch.Equals('_') || ch.Equals('-'));
                n++;
            }
            if (forceDelimit)
                return "'" + source + "'";

            return source;
        }

        [Description("Returns the text representing the text in BNF format.")]
        public string ToBNFString(bool alwaysDelimitTerminals)
        {
            string result;
            switch (Type)
            {
                case SymbolType.Nonterminal:
                    result = "<" + Name + ">";
                    break;
                case SymbolType.Content:
                    result = LiteralFormat(Name, alwaysDelimitTerminals);
                    break;
                default:
                    result = "(" + Name + ")";
                    break;
            }
            return result;
        }

        [Description("Returns the text representing the text in BNF format.")]
        public string ToBNFString()
        {
            return ToBNFString(false);
        }

        public override string ToString()
        {
            return ToBNFString();
        }
    }
}