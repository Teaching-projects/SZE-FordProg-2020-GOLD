using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;

namespace GOLD.Builder
{
    public class SymbolList
    {
        private ArrayList m_Array;

        internal SymbolList()
        {
            this.m_Array = new ArrayList();
        }

        internal SymbolList(int Size)
        {
            this.m_Array = new ArrayList();
            this.ReDimension(Size);
        }

        public Symbol this[int Index]
        {
            get
            {
                if (Index >= 0 & Index < this.m_Array.Count)
                    return (Symbol)this.m_Array[Index];
                return (Symbol)null;
            }
            internal set
            {
                this.m_Array[Index] = (object)value;
            }
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        internal void Clear()
        {
            this.m_Array.Clear();
        }

        internal void ReDimension(int Size)
        {
            this.m_Array.Clear();
            int num1 = checked(Size - 1);
            int num2 = 0;
            while (num2 <= num1)
            {
                this.m_Array.Add((object)null);
                checked { ++num2; }
            }
        }

        internal int Add(Symbol Item)
        {
            return this.m_Array.Add((object)Item);
        }

        internal Symbol GetFirstOfType(SymbolType Type)
        {
            Symbol symbol1 = (Symbol)null;
            short num1 = 0;
            short num2 = 0;
            while (((uint)~num1 & (uint)(short)-((int)num2 < this.m_Array.Count ? 1 : 0)) > 0U)
            {
                Symbol symbol2 = (Symbol)this.m_Array[(int)num2];
                if (symbol2.Type == Type)
                {
                    num1 = (short)-1;
                    symbol1 = symbol2;
                }
                checked { ++num2; }
            }
            return symbol1;
        }

        public string Text(string Separator = ", ", bool AlwaysDelimitTerminals = false)
        {
            string str = "";
            int num = checked(this.m_Array.Count - 1);
            int index = 0;
            while (index <= num)
            {
                Symbol symbol = (Symbol)this.m_Array[index];
                str = Conversions.ToString(Operators.ConcatenateObject((object)str, Operators.ConcatenateObject(Interaction.IIf(index == 0, (object)"", (object)Separator), (object)symbol.Text(AlwaysDelimitTerminals))));
                checked { ++index; }
            }
            return str;
        }

        internal string IndexList(string Separator = ", ")
        {
            string str = "";
            if (this.m_Array.Count >= 1)
            {
                int index1 = 0;
                str = Conversions.ToString(NewLateBinding.LateGet(this.m_Array[index1], (Type)null, "TableIndex", new object[0], (string[])null, (Type[])null, (bool[])null));
                int num = checked(this.m_Array.Count - 1);
                int index2 = 1;
                while (index2 <= num)
                {
                    str = Conversions.ToString(Operators.ConcatenateObject((object)str, Operators.ConcatenateObject((object)Separator, NewLateBinding.LateGet(this.m_Array[index2], (Type)null, "TableIndex", new object[0], (string[])null, (Type[])null, (bool[])null))));
                    checked { ++index2; }
                }
            }
            return str;
        }
    }
}