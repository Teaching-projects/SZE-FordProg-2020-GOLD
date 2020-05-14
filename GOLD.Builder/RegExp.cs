using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GOLD.Builder
{

    internal class RegExp
    {
        private ArrayList m_Array;

        public RegExp()
        {
            this.m_Array = new ArrayList();
        }

        public bool IsVariableLength()
        {
            bool flag = false;
            short num = 0;
            while ((int)num < this.m_Array.Count & !flag)
            {
                if (((RegExpSeq)this.m_Array[(int)num]).IsVariableLength())
                    flag = true;
                checked { ++num; }
            }
            return flag;
        }

        public int Count()
        {
            return this.m_Array.Count;
        }

        public RegExpSeq this[int Index]
        {
            get
            {
                if (Index >= 0 & Index < this.m_Array.Count)
                    return (RegExpSeq)this.m_Array[Index];
                return (RegExpSeq)null;
            }
            set
            {
                this.m_Array[Index] = (object)value;
            }
        }

        public void Add(ref RegExpSeq Item)
        {
            this.m_Array.Add((object)Item);
        }

        public override string ToString()
        {
            string str = "";
            if (this.m_Array.Count >= 1)
            {
                str = this.m_Array[0].ToString();
                int num = checked(this.m_Array.Count - 1);
                int index = 1;
                while (index <= num)
                {
                    str = str + " | " + this.m_Array[index].ToString();
                    checked { ++index; }
                }
            }
            return str;
        }

        public void AddTextExp(string Expression)
        {
            RegExpSeq regExpSeq1 = new RegExpSeq();
            string Text = "";
            string[] strArray = Microsoft.VisualBasic.Strings.Split(Expression, "|", -1, CompareMethod.Text);
            int num1 = checked(((IEnumerable<string>)strArray).Count<string>() - 1);
            int index1 = 0;
            while (index1 <= num1)
            {
                strArray[index1] = strArray[index1].Trim();
                checked { ++index1; }
            }
            int num2 = checked(((IEnumerable<string>)strArray).Count<string>() - 1);
            int index2 = 0;
            while (index2 <= num2)
            {
                string source = strArray[index2];
                int startIndex = 0;
                RegExpSeq regExpSeq2 = new RegExpSeq();
                while (startIndex < source.Count<char>())
                {
                    if (source[startIndex] == '{')
                    {
                        int num3 = source.IndexOf("}", startIndex);
                        Text = source.Substring(checked(startIndex + 1), checked(num3 - startIndex - 1));
                        startIndex = checked(num3 + 1);
                    }
                    string Kleene = "";
                    if (startIndex < source.Count<char>())
                    {
                        string Left = source.Substring(startIndex, 1);
                        if (Operators.CompareString(Left, "+", true) == 0 || Operators.CompareString(Left, "?", true) == 0 || Operators.CompareString(Left, "*", true) == 0)
                        {
                            Kleene = source.Substring(startIndex, 1);
                            checked { ++startIndex; }
                        }
                    }
                    RegExpSeq regExpSeq3 = regExpSeq2;
                    RegExpItem regExpItem = new RegExpItem((object)new SetItem(SetItem.SetType.Name, Text), Kleene);
                    ref RegExpItem local = ref regExpItem;
                    regExpSeq3.Add(ref local);
                }
                this.m_Array.Add((object)regExpSeq2);
                checked { ++index2; }
            }
        }
    }
}