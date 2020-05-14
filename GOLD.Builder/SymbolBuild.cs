
using Microsoft.VisualBasic.CompilerServices;
namespace GOLD.Builder
{
    internal class SymbolBuild : Symbol
    {
        public RegExp RegularExp;
        public bool VariableLength;
        public bool UsesDFA;
        public bool Accepted;
        public CreatorType CreatedBy;
        public bool Reclassified;
        public LookaheadSymbolSet First;
        public bool Nullable;
        public LRConfigSet PartialClosure;
        public string LinkName;

        internal SymbolBuild()
        {
            this.First = new LookaheadSymbolSet();
        }

        internal SymbolBuild(string Name, SymbolType Type)
          : base(Name, Type)
        {
            this.First = new LookaheadSymbolSet();
            this.UsesDFA = (uint)this.ImpliedDFAUsage(Type) > 0U;
            this.CreatedBy = CreatorType.Defined;
            this.Reclassified = false;
        }

        internal SymbolBuild(string Name, SymbolType Type, bool UsesDFA)
          : base(Name, Type)
        {
            this.First = new LookaheadSymbolSet();
            this.UsesDFA = UsesDFA;
            this.CreatedBy = CreatorType.Defined;
            this.Reclassified = false;
        }

        internal SymbolBuild(string Name, SymbolType Type, bool UsesDFA, CreatorType CreatedBy)
          : base(Name, Type)
        {
            this.First = new LookaheadSymbolSet();
            this.UsesDFA = UsesDFA;
            this.CreatedBy = CreatedBy;
            this.Reclassified = false;
        }

        internal SymbolBuild(string Name, SymbolType Type, int TableIndex)
          : base(Name, Type, checked((short)TableIndex))
        {
            this.First = new LookaheadSymbolSet();
            this.UsesDFA = true;
            this.CreatedBy = CreatorType.Defined;
            this.Reclassified = false;
        }

        internal SymbolBuild(string Name, SymbolType Type, RegExp Exp)
          : base(Name, Type)
        {
            this.First = new LookaheadSymbolSet();
            this.RegularExp = Exp;
            this.UsesDFA = true;
            this.CreatedBy = CreatorType.Defined;
            this.Reclassified = false;
        }

        internal string CreatedByName()
        {
            switch (this.CreatedBy)
            {
                case CreatorType.Defined:
                    return "Defined in Grammar";
                case CreatorType.Generated:
                    return "Generated";
                case CreatorType.Implicit:
                    return "Implicitly Defined";
                default:
                    return "";
            }
        }

        private SymbolType ImpliedDFAUsage(SymbolType Type)
        {
            switch (Type)
            {
                case SymbolType.Content:
                case SymbolType.Noise:
                case SymbolType.GroupStart:
                case SymbolType.GroupEnd:
                    return ~SymbolType.Nonterminal;
                default:
                    return SymbolType.Nonterminal;
            }
        }

        public GroupBuild Group
        {
            get
            {
                return (GroupBuild)this.Group;
            }
            set
            {
                this.Group = value;
            }
        }

        public bool IsLessThan(ref SymbolBuild Symbol2)
        {
            short num1 = this.SymbolKindValue(this.Type);
            short num2 = this.SymbolKindValue(Symbol2.Type);
            return (int)num1 != (int)num2 ? (int)num1 < (int)num2 : Operators.CompareString(this.Name.ToUpper(), Symbol2.Name.ToUpper(), true) < 0;
        }

        private short SymbolKindValue(SymbolType Type)
        {
            short num=0;
            switch (Type)
            {
                case SymbolType.Nonterminal:
                    num = (short)5;
                    break;
                case SymbolType.Content:
                    num = (short)4;
                    break;
                case SymbolType.Noise:
                    num = (short)2;
                    break;
                case SymbolType.End:
                case SymbolType.Error:
                    num = (short)1;
                    break;
                case SymbolType.GroupStart:
                case SymbolType.GroupEnd:
                    num = (short)3;
                    break;
            }
            return num;
        }

        public bool NeedsDeclaration()
        {
            return this.UsesDFA & this.RegularExp == null;
        }

        internal SymbolCategory Category()
        {
            switch (this.Type)
            {
                case SymbolType.Nonterminal:
                    return SymbolCategory.Nonterminal;
                case SymbolType.Content:
                case SymbolType.Noise:
                case SymbolType.GroupStart:
                case SymbolType.GroupEnd:
                    return SymbolCategory.Terminal;
                case SymbolType.End:
                case SymbolType.Error:
                    return SymbolCategory.Special;
                default:
                    return SymbolCategory.Nonterminal;
            }
        }

        internal bool IsFormalTerminal()
        {
            switch (this.Type)
            {
                case SymbolType.Content:
                case SymbolType.GroupStart:
                case SymbolType.GroupEnd:
                    return true;
                default:
                    return false;
            }
        }
    }
}