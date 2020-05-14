using Microsoft.VisualBasic.CompilerServices;

namespace GOLD.Builder
{
    internal class LRAction
    {
        private Symbol m_Symbol;
        private LRActionType m_Type;
        private short m_Value;

        public LRAction(Symbol TheSymbol, LRActionType Type, short Value)
        {
            this.m_Symbol = TheSymbol;
            this.m_Type = Type;
            this.m_Value = Value;
        }

        public LRAction(Symbol TheSymbol, LRActionType Type)
        {
            this.m_Symbol = TheSymbol;
            this.m_Type = Type;
            this.m_Value = (short)0;
        }

        public LRAction()
        {
        }

        public LRActionType Type()
        {
            return this.m_Type;
        }

        public short Value()
        {
            return this.m_Value;
        }

        public string Name()
        {
            string str = "";
            switch (this.m_Type)
            {
                case LRActionType.Shift:
                    str = "Shift to State";
                    break;
                case LRActionType.Reduce:
                    str = "Reduce Production";
                    break;
                case LRActionType.Goto:
                    str = "Go to State";
                    break;
                case LRActionType.Accept:
                    str = "Accept";
                    break;
                case LRActionType.Error:
                    str = "Error";
                    break;
            }
            return str;
        }

        public string NameShort()
        {
            string str = "";
            switch (this.m_Type)
            {
                case LRActionType.Shift:
                    str = "s";
                    break;
                case LRActionType.Reduce:
                    str = "r";
                    break;
                case LRActionType.Goto:
                    str = "g";
                    break;
                case LRActionType.Accept:
                    str = "a";
                    break;
                case LRActionType.Error:
                    str = "Error";
                    break;
            }
            return str;
        }

        public Symbol Symbol
        {
            get
            {
                return this.m_Symbol;
            }
            set
            {
                this.m_Symbol = value;
            }
        }

        public short SymbolIndex()
        {
            return this.m_Symbol.TableIndex;
        }

        public string Text()
        {
            string str;
            switch (this.m_Type)
            {
                case LRActionType.Shift:
                case LRActionType.Reduce:
                case LRActionType.Goto:
                    str = this.m_Symbol.Text(false) + " " + this.Name() + " " + Conversions.ToString((int)this.m_Value);
                    break;
                default:
                    str = this.m_Symbol.Text(false) + " " + this.Name();
                    break;
            }
            return str;
        }

        public string TextShort()
        {
            string str;
            switch (this.m_Type)
            {
                case LRActionType.Shift:
                case LRActionType.Reduce:
                case LRActionType.Goto:
                    str = this.m_Symbol.Text(false) + " " + this.NameShort() + " " + Conversions.ToString((int)this.m_Value);
                    break;
                default:
                    str = this.m_Symbol.Text(false) + " " + this.NameShort();
                    break;
            }
            return str;
        }

        public LRConflict ConflictWith(LRActionType TypeB)
        {
            LRConflict lrConflict = LRConflict.None;
            switch (this.m_Type)
            {
                case LRActionType.Shift:
                    switch (TypeB)
                    {
                        case LRActionType.Shift:
                            lrConflict = LRConflict.None;
                            break;
                        case LRActionType.Reduce:
                            lrConflict = LRConflict.ShiftReduce;
                            break;
                        case LRActionType.Accept:
                            lrConflict = LRConflict.AcceptShift;
                            break;
                    }
                    break;
                case LRActionType.Reduce:
                    switch (TypeB)
                    {
                        case LRActionType.Shift:
                            lrConflict = LRConflict.ShiftReduce;
                            break;
                        case LRActionType.Reduce:
                            lrConflict = LRConflict.ReduceReduce;
                            break;
                        case LRActionType.Accept:
                            lrConflict = LRConflict.AcceptReduce;
                            break;
                    }
                    break;
            }
            return lrConflict;
        }
    }
}