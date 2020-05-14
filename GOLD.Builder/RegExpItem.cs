using Microsoft.VisualBasic.CompilerServices;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GOLD.Builder
{
    internal class RegExpItem
    {
        private object m_Data;
        private string m_Kleene;

        public RegExpItem()
        {
            this.m_Data = (object)null;
            this.m_Kleene = "";
        }

        public RegExpItem(object Data, string Kleene)
        {
            this.m_Data = RuntimeHelpers.GetObjectValue(Data);
            this.m_Kleene = Kleene;
        }

        public object Data
        {
            get
            {
                return this.m_Data;
            }
            set
            {
                this.m_Data = RuntimeHelpers.GetObjectValue(value);
            }
        }

        public string Kleene
        {
            get
            {
                return this.m_Kleene;
            }
            set
            {
                this.m_Kleene = value;
            }
        }

        public bool IsVariableLength()
        {
            string kleene = this.m_Kleene;
            return Operators.CompareString(kleene, "*", true) == 0 || Operators.CompareString(kleene, "+", true) == 0 || this.m_Data is RegExp && ((RegExp)this.m_Data).IsVariableLength();
        }

        public override string ToString()
        {
            string str1 = "";
            if (this.Data is RegExp)
                str1 = "(" + this.Data.ToString() + ")" + this.Kleene;
            else if (this.Data is SetItem)
            {
                SetItem data = (SetItem)this.Data;
                switch (data.Type)
                {
                    case SetItem.SetType.Chars:
                        CharacterSet characters = data.Characters;
                        string str2 = "..";
                        ref string local1 = ref str2;
                        string str3 = ", ";
                        ref string local2 = ref str3;
                        str1 = "{" + characters.RangeText(local1, local2, "&", true) + "}" + this.Kleene;
                        break;
                    case SetItem.SetType.Name:
                        str1 = "{" + data.Text + "}" + this.Kleene;
                        break;
                    case SetItem.SetType.Sequence:
                        str1 = this.LiteralFormat(data.Text) + this.Kleene;
                        break;
                }
            }
            return str1;
        }

        public string LiteralFormat(string Source)
        {
            string str;
            if (Operators.CompareString(Source, "'", true) == 0)
            {
                str = "''";
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
    }
}