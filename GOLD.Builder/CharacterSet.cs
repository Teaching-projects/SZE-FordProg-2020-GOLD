using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Text;

namespace GOLD.Builder
{

    internal class CharacterSet : NumberSet
    {
        public int TableIndex;

        public new string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int num = checked(this.Count() - 1);
            int index = 0;
            while (index <= num)
            {
                stringBuilder.Append(Strings.ChrW(this[index]));
                checked { ++index; }
            }
            return stringBuilder.ToString();
        }

        public CharacterSet()
          : base()
        {
            this.Blocksize = (short)64;
        }

        public CharacterSet(string Text)
          : base()
        {
            int num = checked(Text.Length - 1);
            int index = 0;
            while (index <= num)
            {
                this.Add((int)Text[index]);
                checked { ++index; }
            }
        }

        public CharacterSet(NumberSet CharCodes)
          : base(CharCodes)
        {
        }

        public CharacterSet(params int[] CharCodes) : base(CharCodes)
        {
        }

        public char Chars(int Index)
        {
            char ch = '\0';
            if (Index >= 0 & Index < this.Count())
                ch = Strings.ChrW(this[Index]);
            return ch;
        }

        public int Length()
        {
            return this.Count();
        }

        public string XMLText()
        {
            string str = "";
            int num = checked(this.Count() - 1);
            int index = 0;
            while (index <= num)
            {
                int CharCode = this[index];
                switch (this[index])
                {
                    case 34:
                        str += "&quot;";
                        break;
                    case 38:
                        str += "&amp;";
                        break;
                    case 60:
                        str += "&lt;";
                        break;
                    case 62:
                        str += "&gt;";
                        break;
                    default:
                        str = !(CharCode >= 32 & CharCode <= 126) ? str + "&#" + Conversions.ToString(CharCode) + ";" : str + Conversions.ToString(Strings.ChrW(CharCode));
                        break;
                }
                checked { ++index; }
            }
            return str;
        }
    }
}