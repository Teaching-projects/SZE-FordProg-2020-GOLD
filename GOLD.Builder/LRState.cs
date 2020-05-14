using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Diagnostics;

namespace GOLD.Builder
{
    internal class LRState : ArrayList
    {
        [DebuggerNonUserCode]
        public LRState()
        {
        }

        public short IndexOf(Symbol Item)
        {
            short num1 = 0;
            bool flag = false;
            short num2=0;
            while (!flag & (int)num1 < this.Count)
            {
                if (Item.IsEqualTo((Symbol)NewLateBinding.LateGet(this[(int)num1], (Type)null, "Symbol", new object[0], (string[])null, (Type[])null, (bool[])null)))
                {
                    num2 = num1;
                    flag = true;
                }
                checked { ++num1; }
            }
            if (flag)
                return num2;
            return -1;
        }

        public void Add(LRAction Action)
        {
            this.Add((object)Action);
        }

        public LRAction this[short Index]
        {
            get
            {
                return (LRAction)this[(int)Index];
            }
            set
            {
                this[(int)Index] = (object)value;
            }
        }

        public LRAction this[Symbol Sym]
        {
            get
            {
                int index = (int)this.IndexOf(Sym);
                if (index != -1)
                    return (LRAction)this[index];
                return (LRAction)null;
            }
            set
            {
                int index = (int)this.IndexOf(Sym);
                if (index == -1)
                    return;
                this[index] = (object)value;
            }
        }
    }
}