using Microsoft.VisualBasic.CompilerServices;
using System.Linq;

namespace GOLD.Builder
{
    public class Symbol
    {
        private string m_Name;
        private SymbolType m_Type;
        private short m_TableIndex;
        internal Group Group;

        internal Symbol(string Name, SymbolType Type)
        {
            this.m_Name = Name;
            this.m_Type = Type;
            this.m_TableIndex = (short)-1;
        }

        internal Symbol(string Name, SymbolType Type, short TableIndex)
        {
            this.m_Name = Name;
            this.m_Type = Type;
            this.m_TableIndex = TableIndex;
        }

        internal Symbol()
        {
            this.m_Name = "";
            this.m_Type = SymbolType.Content;
            this.m_TableIndex = (short)-1;
        }

        public SymbolType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }

        public short TableIndex
        {
            get
            {
                return this.m_TableIndex;
            }
        }

        internal void SetTableIndex(short Value)
        {
            this.m_TableIndex = Value;
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        public string Text(bool AlwaysDelimitTerminals = false)
        {
            string str;
            switch (this.m_Type)
            {
                case SymbolType.Nonterminal:
                    str = "<" + this.Name + ">";
                    break;
                case SymbolType.End:
                case SymbolType.Error:
                    str = "(" + this.Name + ")";
                    break;
                default:
                    str = this.LiteralFormat(this.Name, AlwaysDelimitTerminals);
                    break;
            }
            return str;
        }

        public string LiteralFormat(string Source, bool AlwaysDelimit)
        {
            string str;
            if (Operators.CompareString(Source, "'", true) == 0)
                str = "''";
            else if (AlwaysDelimit)
            {
                str = "'" + Source + "'";
            }
            else
            {
                bool flag = false;
                short num = 0;
                while ((int)num < Source.Count<char>() & !flag)
                {
                    flag = !char.IsLetter(Source[(int)num]);
                    checked { ++num; }
                }
                str = !flag ? Source : "'" + Source + "'";
            }
            return str;
        }

        internal bool IsEqualTo(Symbol Sym)
        {
            return Operators.CompareString(this.m_Name.ToUpper(), Sym.Name.ToUpper(), true) == 0 & this.m_Type == Sym.Type;
        }

        public string TypeName()
        {
            string str = "";
            switch (this.m_Type)
            {
                case SymbolType.Nonterminal:
                    str = "Nonterminal";
                    break;
                case SymbolType.Content:
                    str = "Content";
                    break;
                case SymbolType.Noise:
                    str = "Noise";
                    break;
                case SymbolType.End:
                    str = "End of File";
                    break;
                case SymbolType.GroupStart:
                    str = "Lexical Group Start";
                    break;
                case SymbolType.GroupEnd:
                    str = "Lexical Group End";
                    break;
                case SymbolType.LEGACYCommentLine:
                    str = "Comment Line (LEGACY)";
                    break;
                case SymbolType.Error:
                    str = "Runtime Error Symbol";
                    break;
            }
            return str;
        }
    }
}