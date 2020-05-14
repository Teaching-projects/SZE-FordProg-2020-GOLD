using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;

namespace GOLD.Builder
{

    internal class NumberSet
    {
        private short blockSize;
        private int arraySize;
        private int[] list;
        private int count;

        public short Blocksize
        {
            get
            {
                return blockSize;
            }
            set
            {
                if (value >= 1)
                    blockSize = value;
                else
                    blockSize = 1;
            }
        }

        public NumberSet(params int[] numbers)
        {
            arraySize = 0;
            blockSize = 16;
            count = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                AddItem(numbers[i]);
            }
        }

        public NumberSet(NumberSet numbers)
        {
            count = numbers.Count();
            blockSize = 16;
            ResizeArray(count);
            for (int i = 0; i < count; i++)
            {
                list[i] = numbers[i];
            }
        }

        public Compare CompareTo(NumberSet numberSet)
        {
            int count = this.count;
            int num = numberSet.Count();
            if (count == num)
            {
                for (int i = 0; i < count; i++)
                {
                    if (list[i] != numberSet[i])
                        return Compare.UnEqual;
                }
                return Compare.Equal;
            }
            else if (count < num)
            {
                int index1 = 0;
                int index2 = 0;
                while (index1 < count && index2 < num)
                {
                    if (list[index1] == numberSet[index2])
                    {
                        index1++;
                        index2++;
                    }
                    else
                        index2++;
                }
                return index1 <= count - 1 ? Compare.UnEqual : Compare.Subset;
            }
            else
                return Compare.UnEqual;
        }

        public void Remove(params int[] numbers)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                int num1 = ItemIndex(numbers[i]);
                if (num1 != -1)
                {
                    for (int j = num1 + 1; j < count; j++)
                    {
                        list[j - 1] = list[j];
                    }
                    count--;
                    ResizeArray(count);
                }
            }
        }

        public void Remove(NumberSet numbers)
        {
            DifferenceWith(numbers);
        }

        private void ResizeArray(int requiredSize)
        {
            if (!(requiredSize > arraySize || Math.Abs(requiredSize - arraySize) > blockSize))
                return;

            arraySize = requiredSize / blockSize + blockSize;
            list = (int[])Utils.CopyArray(list, new int[arraySize]);
        }

        public void Copy(NumberSet list)
        {
            Clear();
            if (list.Count() <= 0)
                return;

            count = list.Count();
            ResizeArray(count);
            for (int i = 0; i < list.Count(); i++)
            {
                list[i] = list[i];
            }
        }

        public void Clear()
        {
            list = null;
            arraySize = 0;
            count = 0;
        }

        public bool UnionWith(NumberSet numberSet)
        {
            int index1 = 0;
            int index2 = 0;
            bool flag = false;
            int num1 = 0;
            int[] numArray = new int[checked(count + numberSet.Count() + 1)];
            while (index1 < count | index2 < numberSet.Count())
            {
                int num2;
                if (index1 >= count)
                {
                    num2 = numberSet[index2];
                    checked { ++index2; }
                    flag = true;
                }
                else if (index2 >= numberSet.Count())
                {
                    num2 = list[index1];
                    checked { ++index1; }
                }
                else if (list[index1] == numberSet[index2])
                {
                    num2 = list[index1];
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else if (list[index1] < numberSet[index2])
                {
                    num2 = list[index1];
                    checked { ++index1; }
                }
                else
                {
                    num2 = numberSet[index2];
                    checked { ++index2; }
                    flag = true;
                }
                checked { ++num1; }
                numArray[checked(num1 - 1)] = num2;
            }
            if (flag)
            {
                count = num1;
                list = new int[checked(count - 1 + 1)];
                int num2 = checked(count - 1);
                int index3 = 0;
                while (index3 <= num2)
                {
                    list[index3] = numArray[index3];
                    checked { ++index3; }
                }
            }
            return flag;
        }

        public NumberSet Union(ref NumberSet SetB)
        {
            NumberSet numberSet = new NumberSet(new int[0]);
            int index1 = 0;
            int index2 = 0;
            while (index1 < this.count | index2 < SetB.Count())
            {
                int num;
                if (index1 >= this.count)
                {
                    num = SetB[index2];
                    checked { ++index2; }
                }
                else if (index2 >= SetB.Count())
                {
                    num = this.list[index1];
                    checked { ++index1; }
                }
                else if (this.list[index1] == SetB[index2])
                {
                    num = this.list[index1];
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else if (this.list[index1] < SetB[index2])
                {
                    num = this.list[index1];
                    checked { ++index1; }
                }
                else
                {
                    num = SetB[index2];
                    checked { ++index2; }
                }
                numberSet.Add(num);
            }
            return numberSet;
        }

        public NumberSet Intersection(ref NumberSet SetB)
        {
            NumberSet numberSet = new NumberSet(new int[0]);
            int index1 = 0;
            int index2 = 0;
            while (index1 < this.count & index2 < SetB.Count())
            {
                if (this.list[index1] == SetB[index2])
                {
                    numberSet.Add(this.list[index1]);
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else if (this.list[index1] < SetB[index2])
                    checked { ++index1; }
                else
                    checked { ++index2; }
            }
            return numberSet;
        }

        public NumberSet Difference(ref NumberSet SetB)
        {
            NumberSet numberSet = new NumberSet(new int[0]);
            int index1 = 0;
            int index2 = 0;
            while (index1 < this.count | index2 < SetB.Count())
            {
                if (index1 >= this.count)
                    checked { ++index2; }
                else if (index2 >= SetB.Count())
                {
                    numberSet.Add(this.list[index1]);
                    checked { ++index1; }
                }
                else if (this.list[index1] == SetB[index2])
                {
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else if (this.list[index1] < SetB[index2])
                {
                    numberSet.Add(this.list[index1]);
                    checked { ++index1; }
                }
                else
                    checked { ++index2; }
            }
            return numberSet;
        }

        public bool DifferenceWith(NumberSet SetB)
        {
            int index1 = 0;
            int index2 = 0;
            int index3 = 0;
            while (index1 < this.count | index2 < SetB.Count())
            {
                if (index1 >= this.count)
                    checked { ++index2; }
                else if (index2 >= SetB.Count())
                {
                    this.list[index3] = this.list[index1];
                    checked { ++index3; }
                    checked { ++index1; }
                }
                else if (this.list[index1] == SetB[index2])
                {
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else if (this.list[index1] < SetB[index2])
                {
                    this.list[index3] = this.list[index1];
                    checked { ++index3; }
                    checked { ++index1; }
                }
                else
                    checked { ++index2; }
            }
            bool flag = this.count != index3;
            this.count = index3;
            this.ResizeArray(this.count);
            return flag;
        }

        public int Count()
        {
            return count;
        }

        public bool Contains(int Number)
        {
            return this.ItemIndex(Number) != -1;
        }

        private int ItemIndex(int Number)
        {
            int num1;
            if (this.count == 0)
                num1 = -1;
            else if (Number < this.list[0] | Number > this.list[checked(this.count - 1)])
            {
                num1 = -1;
            }
            else
            {
                int num2 = checked(this.count - 1);
                int num3 = 0;
                num1 = -1;
                bool flag = false;
                do
                {
                    int index = checked((int)Math.Round(unchecked((double)checked(num3 + num2) / 2.0)));
                    if (num3 > num2)
                        flag = true;
                    else if (this.list[index] == Number)
                    {
                        num1 = index;
                        flag = true;
                    }
                    else if (this.list[index] < Number)
                        num3 = checked(index + 1);
                    else if (this.list[index] > Number)
                        num2 = checked(index - 1);
                }
                while (!flag);
            }
            return num1;
        }

        public bool IsEqualSet(NumberSet SetB)
        {
            bool flag1;
            if (this.count != SetB.Count())
            {
                flag1 = false;
            }
            else
            {
                int index = 0;
                bool flag2 = false;
                while (index <= checked(this.Count() - 1) & !flag2)
                {
                    if (this.list[index] != SetB[index])
                        flag2 = true;
                    checked { ++index; }
                }
                flag1 = !flag2;
            }
            return flag1;
        }

        public bool IsProperSubsetOf(ref NumberSet SetB)
        {
            int index1 = 0;
            int index2 = 0;
            while (index1 < this.count & index2 < SetB.Count())
            {
                if (this.list[index1] == SetB[index2])
                {
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else
                    checked { ++index2; }
            }
            return index1 >= this.count;
        }

        public int this[int Index]
        {
            get
            {
                int num = 0;
                if (Index >= 0 & Index < this.count)
                    num = this.list[Index];
                return num;
            }
            set
            {
                if (!(Index >= 0 & Index < this.count))
                    return;
                this.list[Index] = value;
            }
        }

        public void AddRange(int StartValue, int EndValue)
        {
            if (StartValue > EndValue)
                this.Swap(ref StartValue, ref EndValue);
            int num1 = checked(EndValue - StartValue + 1);
            if (this.count == 0)
            {
                this.count = num1;
                this.ResizeArray(this.count);
                int num2 = checked(this.count - 1);
                int index = 0;
                while (index <= num2)
                {
                    this.list[index] = checked(index + StartValue);
                    checked { ++index; }
                }
            }
            else if (this.list[checked(this.count - 1)] < StartValue)
            {
                this.ResizeArray(checked(this.count + num1));
                int num2 = checked(num1 - 1);
                int num3 = 0;
                while (num3 <= num2)
                {
                    this.list[checked(this.count + num3)] = checked(StartValue + num3);
                    checked { ++num3; }
                }
                checked { this.count += num1; }
            }
            else
            {
                int num2 = StartValue;
                int num3 = EndValue;
                int Number = num2;
                while (Number <= num3)
                {
                    this.AddItem(Number);
                    checked { ++Number; }
                }
            }
        }

        private void AddItem(int Number)
        {
            int index1 = this.InsertIndex(Number);
            if (index1 == -1)
                return;
            checked { ++this.count; }
            this.ResizeArray(this.count);
            int num1 = checked(this.count - 1);
            int num2 = checked(index1 + 1);
            int index2 = num1;
            while (index2 >= num2)
            {
                this.list[index2] = this.list[checked(index2 - 1)];
                checked { index2 += -1; }
            }
            this.list[index1] = Number;
        }

        public void Add(NumberSet Numbers)
        {
            this.UnionWith(Numbers);
        }

        public void Add(params int[] Numbers)
        {
            int upperBound = Numbers.GetUpperBound(0);
            int index = 0;
            while (index <= upperBound)
            {
                this.AddItem(Numbers[index]);
                checked { ++index; }
            }
        }

        private void Swap(ref int a, ref int b)
        {
            int num = a;
            a = b;
            b = num;
        }

        private int InsertIndex(int Number)
        {
            int num1;
            if (this.count == 0)
                num1 = 0;
            else if (Number < this.list[0])
                num1 = 0;
            else if (Number > this.list[checked(this.count - 1)])
            {
                num1 = this.count;
            }
            else
            {
                int num2 = checked(this.count - 1);
                int num3 = 0;
                num1 = -1;
                bool flag = false;
                do
                {
                    int index = checked((int)Math.Round(unchecked((double)checked(num3 + num2) / 2.0)));
                    if (num3 > num2)
                    {
                        num1 = num3;
                        flag = true;
                    }
                    else if (this.list[index] == Number)
                    {
                        num1 = -1;
                        flag = true;
                    }
                    else if (this.list[index] < Number)
                        num3 = checked(index + 1);
                    else if (this.list[index] > Number)
                        num2 = checked(index - 1);
                }
                while (!flag);
            }
            return num1;
        }

        public string Text(string Separator = ", ")
        {
            string str = "";
            if (this.count >= 1)
            {
                str = this.list[0].ToString();
                int num = checked(this.count - 1);
                int index = 1;
                while (index <= num)
                {
                    str = str + Separator + this.list[index].ToString();
                    checked { ++index; }
                }
            }
            return str;
        }

        public override string ToString()
        {
            string Separator = ", ";
            return this.Text(Separator);
        }

        public string RangeText(string RangeChars, string Separator = ",", string Prefix = "", bool HexFormat = false)
        {
            string str = "";
            if (this.count >= 1)
            {
                int First = this.list[0];
                int Last = First;
                int num1 = checked(this.count - 1);
                int index = 1;
                while (index <= num1)
                {
                    int num2 = this.list[index];
                    if (num2 != checked(Last + 1))
                    {
                        str = str + this.RangeItemText(First, Last, RangeChars, Prefix, HexFormat) + Separator;
                        First = num2;
                    }
                    Last = num2;
                    checked { ++index; }
                }
                str += this.RangeItemText(First, Last, RangeChars, Prefix, HexFormat);
            }
            return str;
        }

        public NumberRangeList RangeList()
        {
            NumberRangeList numberRangeList = new NumberRangeList();
            if (this.count >= 1)
            {
                int First = this.list[0];
                int Last = First;
                int num1 = checked(this.count - 1);
                int index = 1;
                while (index <= num1)
                {
                    int num2 = this.list[index];
                    if (num2 != checked(Last + 1))
                    {
                        numberRangeList.Add(new NumberRange(First, Last));
                        First = num2;
                    }
                    Last = num2;
                    checked { ++index; }
                }
                numberRangeList.Add(new NumberRange(First, Last));
            }
            return numberRangeList;
        }

        private string RangeItemText(int First, int Last, string RangeChars, string NumberPrefix = "", bool HexFormat = false)
        {
            string str;
            if (First == Last)
                str = !HexFormat ? NumberPrefix + First.ToString() : NumberPrefix + this.HexByte(First);
            else if (HexFormat)
                str = NumberPrefix + this.HexByte(First) + RangeChars + NumberPrefix + this.HexByte(Last);
            else
                str = NumberPrefix + First.ToString() + RangeChars + NumberPrefix + Last.ToString();
            return str;
        }

        private string HexByte(int Value)
        {
            string str = Conversion.Hex(Value);
            if (str.Length % 2 == 1)
                str = "0" + str;
            return str;
        }

        public enum Compare
        {
            Equal,
            UnEqual,
            Subset
        }
    }
}