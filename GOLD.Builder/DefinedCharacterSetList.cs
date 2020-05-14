using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;

namespace GOLD.Builder
{
    internal class DefinedCharacterSetList : ArrayList
    {
        public DefinedCharacterSetList()
        {
        }

        public DefinedCharacterSetList(int Size)
          : base(Size)
        {
        }

        public DefinedCharacterSet this[int Index]
        {
            get
            {
                return (DefinedCharacterSet)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public DefinedCharacterSet this[string Name]
        {
            get
            {
                return (DefinedCharacterSet)base[this.ItemIndex(Name)];
            }
            set
            {
                this[this.ItemIndex(Name)] = value;
            }
        }

        public int ItemIndex(string Name)
        {
            short num1 = -1;
            short num2 = 0;
            while ((int)num2 < this.Count & num1 == (short)-1)
            {
                if (Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(NewLateBinding.LateGet(base[(int)num2], (Type)null, nameof(Name), new object[0], (string[])null, (Type[])null, (bool[])null), (Type)null, "ToUpper", new object[0], (string[])null, (Type[])null, (bool[])null), (object)Name.ToUpper(), true))
                    num1 = num2;
                else
                    checked { ++num2; }
            }
            return (int)num1;
        }

        public bool Contains(DefinedCharacterSet Item)
        {
            return this.ItemIndex(Item.Name) != -1;
        }

        public int Add(ref DefinedCharacterSet Item)
        {
            return this.Add((object)Item);
        }

        public int Add(string Name, params object[] Values)
        {
            DefinedCharacterSet definedCharacterSet = new DefinedCharacterSet(Name, Values);
            return this.Add(ref definedCharacterSet);
        }
    }
}