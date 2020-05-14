using Microsoft.VisualBasic.CompilerServices;
using System.Collections;
using System.Linq;
namespace GOLD.Builder
{

    internal class VariableList : ArrayList
    {
        public string IgnorableMatchChars;

        public int Add(string Name, object Value = null)
        {
            return this.Add(new Variable()
            {
                Name = Name,
                Value = Conversions.ToString(Value)
            });
        }

        public int Add(Variable NewVar)
        {
            short num = checked((short)this.ItemIndex(NewVar.Name));
            if (num == (short)-1)
                return this.Add((object)NewVar);
            return (int)num;
        }

        public void AddList(ref VariableList List)
        {
            int num = checked(List.Count - 1);
            int index = 0;
            while (index <= num)
            {
                this.Add(List[index]);
                checked { ++index; }
            }
        }

        public void ClearValues()
        {
            int num = checked(this.Count - 1);
            int index = 0;
            while (index <= num)
            {
                ((Variable)base[index]).Value = "";
                checked { ++index; }
            }
        }

        public Variable this[string Name]
        {
            get
            {
                int index = this.ItemIndex(Name);
                if (index == -1)
                    index = this.Add((object)new Variable(Name));
                return (Variable)base[index];
            }
            set
            {
                int index = this.ItemIndex(Name);
                if (!(index >= 0 & index < this.Count))
                    return;
                this[index] = value;
            }
        }

        public Variable this[int Index]
        {
            get
            {
                if (Index >= 0 & Index < this.Count)
                    return (Variable)base[Index];
                return (Variable)null;
            }
            set
            {
                if (!(Index >= 0 & Index < this.Count))
                    return;
                this[Index] = value;
            }
        }

        public int ItemIndex(string Name)
        {
            short num1 = -1;
            short num2 = 0;
            while ((int)num2 < this.Count & num1 == (short)-1)
            {
                if (this.IsNameMatch(((Variable)base[(int)num2]).Name, Name))
                    num1 = num2;
                else
                    checked { ++num2; }
            }
            return (int)num1;
        }

        private bool IsNameMatch(string MainName, string Inquiry)
        {
            return Operators.CompareString(MainName.ToUpper(), Inquiry.ToUpper(), true) == 0 | Operators.CompareString(this.RemoveIgnorableChars(MainName).ToUpper(), Inquiry.ToUpper(), true) == 0;
        }

        private string RemoveIgnorableChars(string Text)
        {
            string str = "";
            int num = checked(Text.Count<char>() - 1);
            int index = 0;
            while (index <= num)
            {
                if (!this.IgnorableMatchChars.Contains(Conversions.ToString(Text[index])))
                    str += Conversions.ToString(Text[index]);
                checked { ++index; }
            }
            return str;
        }

        public bool Contains(string Name)
        {
            return this.ItemIndex(Name) != -1;
        }

        public VariableList()
        {
            this.IgnorableMatchChars = "";
        }

        public VariableList(string IgnorableMatchChars)
        {
            this.IgnorableMatchChars = IgnorableMatchChars;
        }
    }
}