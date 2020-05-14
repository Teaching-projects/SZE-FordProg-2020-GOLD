using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GOLD.Builder
{
    internal class DictionarySet
    {
        protected DictionarySet.SortArray MyList;

        public DictionarySet(DictionarySet Dictionary)
        {
            this.MyList = new DictionarySet.SortArray();
            this.MyList.ResizeArray(Dictionary.Count());
            int num = checked(Dictionary.Count() - 1);
            int index = 0;
            while (index <= num)
            {
                this.MyList.Add(Dictionary[index]);
                checked { ++index; }
            }
        }

        public DictionarySet(DictionarySet A, DictionarySet B)
        {
            this.MyList = new DictionarySet.SortArray();
            this.MyList = this.DoUnion(A, B).List;
        }

        public DictionarySet()
        {
            this.MyList = new DictionarySet.SortArray();
        }

        public DictionarySet.Compare CompareTo(DictionarySet SetB)
        {
            int num1 = checked((int)this.MyList.Count());
            int num2 = SetB.Count();
            DictionarySet.Compare compare;
            if (num1 == num2)
            {
                bool flag = false;
                int index = 0;
                while (index < num1 & !flag)
                {
                    if (this.MyList[index].Key().CompareTo((object)SetB[index].Key()) != 0)
                        flag = true;
                    else
                        checked { ++index; }
                }
                compare = !flag ? DictionarySet.Compare.Equal : DictionarySet.Compare.UnEqual;
            }
            else if (num1 < num2)
            {
                int index1 = 0;
                int index2 = 0;
                while (index1 < num1 & index2 < num2)
                {
                    if (this.MyList[index1].Key().CompareTo((object)SetB[index2].Key()) == 0)
                    {
                        checked { ++index1; }
                        checked { ++index2; }
                    }
                    else
                        checked { ++index2; }
                }
                compare = index1 <= checked(num1 - 1) ? DictionarySet.Compare.UnEqual : DictionarySet.Compare.Subset;
            }
            else
                compare = DictionarySet.Compare.UnEqual;
            return compare;
        }

        public void Remove(params DictionarySet.IMember[] Item)
        {
            int num = checked(((IEnumerable<DictionarySet.IMember>)Item).Count<DictionarySet.IMember>() - 1);
            int index = 0;
            while (index <= num)
            {
                int Index = this.IndexOf(Item[index].Key());
                if (Index != -1)
                    this.MyList.Remove(Index);
                checked { ++index; }
            }
            this.MyList.RemoveNullItems();
        }

        public bool Remove(DictionarySet SetB)
        {
            int Index = 0;
            int index = 0;
            int num = checked((int)this.MyList.Count());
            while ((long)Index < this.MyList.Count() | index < SetB.Count())
            {
                if ((long)Index >= this.MyList.Count())
                    checked { ++index; }
                else if (index >= SetB.Count())
                    checked { ++Index; }
                else if (this.MyList[Index].Key().CompareTo((object)SetB[index].Key()) == 0)
                {
                    this.MyList.Remove(Index);
                    checked { ++Index; }
                    checked { ++index; }
                }
                else if (this.MyList[Index].Key().CompareTo((object)SetB[index].Key()) < 0)
                    checked { ++Index; }
                else
                    checked { ++index; }
            }
            this.MyList.RemoveNullItems();
            bool flag = false;
            if ((long)num != this.MyList.Count())
                flag = true;
            return flag;
        }

        public void Copy(DictionarySet List)
        {
            this.Clear();
            if (List.Count() <= 0)
                return;
            int num = checked(List.Count() - 1);
            int index = 0;
            while (index <= num)
            {
                this.MyList.Add(List[index]);
                checked { ++index; }
            }
        }

        public void Clear()
        {
            this.MyList.Clear();
        }

        public bool UnionWith(DictionarySet SetB)
        {
            int num = checked((int)this.MyList.Count());
            DictionarySet.SetOperation setOperation = this.DoUnion(this, SetB);
            this.MyList = setOperation.List;
            return setOperation.Changed | this.MyList.Count() != (long)num;
        }

        public DictionarySet Union(ref DictionarySet SetB)
        {
            DictionarySet dictionarySet = new DictionarySet();
            DictionarySet.SetOperation setOperation = this.DoUnion(this, SetB);
            dictionarySet.MyList = setOperation.List;
            return dictionarySet;
        }

        private DictionarySet.SetOperation DoUnion(DictionarySet SetA, DictionarySet SetB)
        {
            DictionarySet.SetOperation setOperation = new DictionarySet.SetOperation();
            int index1 = 0;
            int index2 = 0;
            setOperation.Changed = false;
            while (index1 < SetA.Count() | index2 < SetB.Count())
            {
                DictionarySet.IMember newObject;
                if (index1 >= SetA.Count())
                {
                    newObject = SetB[index2];
                    checked { ++index2; }
                }
                else if (index2 >= SetB.Count())
                {
                    newObject = SetA[index1];
                    checked { ++index1; }
                }
                else if (SetA[index1].Key().CompareTo((object)SetB[index2].Key()) == 0)
                {
                    DictionarySet.MemberResult memberResult = SetA[index1].Union(SetB[index2]);
                    if (memberResult == null)
                    {
                        newObject = SetA[index1];
                    }
                    else
                    {
                        newObject = memberResult.NewObject;
                        if (memberResult.SetChanged)
                            setOperation.Changed = true;
                    }
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else if (SetA[index1].Key().CompareTo((object)SetB[index2].Key()) < 0)
                {
                    newObject = SetA[index1];
                    checked { ++index1; }
                }
                else
                {
                    newObject = SetB[index2];
                    checked { ++index2; }
                }
                setOperation.List.Add(newObject);
            }
            setOperation.List.RemoveNullItems();
            return setOperation;
        }

        public DictionarySet Intersection(ref DictionarySet SetB)
        {
            DictionarySet dictionarySet = new DictionarySet();
            DictionarySet.SetOperation setOperation = this.DoIntersection(this, SetB);
            dictionarySet.MyList = setOperation.List;
            return dictionarySet;
        }

        public bool IntersectionWith(DictionarySet SetB)
        {
            int num = checked((int)this.MyList.Count());
            DictionarySet.SetOperation setOperation = this.DoIntersection(this, SetB);
            this.MyList = setOperation.List;
            return setOperation.Changed | this.MyList.Count() != (long)num;
        }

        private DictionarySet.SetOperation DoIntersection(
          DictionarySet SetA,
          DictionarySet SetB)
        {
            DictionarySet.SetOperation setOperation = new DictionarySet.SetOperation();
            int index1 = 0;
            int index2 = 0;
            setOperation.Changed = false;
            while (index1 < SetA.Count() & index2 < SetB.Count())
            {
                if (SetA[index1].Key().CompareTo((object)SetB[index2].Key()) == 0)
                {
                    DictionarySet.MemberResult memberResult = SetA[index1].Intersect(SetB[index2]);
                    if (memberResult != null)
                    {
                        setOperation.List.Add(memberResult.NewObject);
                        if (memberResult.SetChanged)
                            setOperation.Changed = true;
                    }
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else if (SetA[index1].Key().CompareTo((object)SetB[index2].Key()) < 0)
                    checked { ++index1; }
                else
                    checked { ++index2; }
            }
            setOperation.List.RemoveNullItems();
            return setOperation;
        }

        public DictionarySet Difference(ref DictionarySet SetB)
        {
            DictionarySet dictionarySet = new DictionarySet();
            DictionarySet.SetOperation setOperation = this.DoDifference(this, ref SetB);
            dictionarySet.MyList = setOperation.List;
            return dictionarySet;
        }

        public bool DifferenceWith(DictionarySet SetB)
        {
            int num = checked((int)this.MyList.Count());
            DictionarySet.SetOperation setOperation = this.DoDifference(this, ref SetB);
            this.MyList = setOperation.List;
            return setOperation.Changed | this.MyList.Count() != (long)num;
        }

        private DictionarySet.SetOperation DoDifference(
          DictionarySet SetA,
          ref DictionarySet SetB)
        {
            DictionarySet.SetOperation setOperation = new DictionarySet.SetOperation();
            int index1 = 0;
            int index2 = 0;
            setOperation.Changed = false;
            while (index1 < SetA.Count() | index2 < SetB.Count())
            {
                if (index1 >= SetA.Count())
                    checked { ++index2; }
                else if (index2 >= SetB.Count())
                {
                    setOperation.List.Add(this.MyList[index1]);
                    checked { ++index1; }
                }
                else if (SetA[index1].Key().CompareTo((object)SetB[index2].Key()) == 0)
                {
                    DictionarySet.MemberResult memberResult = SetA[index1].Difference(SetB[index2]);
                    if (memberResult != null)
                    {
                        setOperation.List.Add(memberResult.NewObject);
                        if (memberResult.SetChanged)
                            setOperation.Changed = true;
                    }
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else if (SetA[index1].Key().CompareTo((object)SetB[index2].Key()) < 0)
                {
                    setOperation.List.Add(this.MyList[index1]);
                    checked { ++index1; }
                }
                else
                    checked { ++index2; }
            }
            setOperation.List.RemoveNullItems();
            return setOperation;
        }

        public int Count()
        {
            return checked((int)this.MyList.Count());
        }

        public bool Contains(IComparable Key)
        {
            return this.IndexOf(Key) != -1;
        }

        public int IndexOf(IComparable Key)
        {
            int num1;
            if (this.MyList.Count() == 0L)
                num1 = -1;
            else if (Key.CompareTo((object)this.MyList[0].Key()) < 0)
                num1 = -1;
            else if (Key.CompareTo((object)this.MyList[checked((int)(this.MyList.Count() - 1L))].Key()) > 0)
            {
                num1 = -1;
            }
            else
            {
                int num2 = checked((int)(this.MyList.Count() - 1L));
                int num3 = 0;
                num1 = -1;
                bool flag = false;
                do
                {
                    int index = checked((int)Math.Round(unchecked((double)checked(num3 + num2) / 2.0)));
                    if (num3 > num2)
                        flag = true;
                    else if (this.MyList[index].Key().CompareTo((object)Key) == 0)
                    {
                        num1 = index;
                        flag = true;
                    }
                    else if (this.MyList[index].Key().CompareTo((object)Key) < 0)
                        num3 = checked(index + 1);
                    else if (this.MyList[index].Key().CompareTo((object)Key) > 0)
                        num2 = checked(index - 1);
                }
                while (!flag);
            }
            return num1;
        }

        public bool IsEqualSet(DictionarySet SetB)
        {
            bool flag;
            if (this.MyList.Count() != (long)SetB.Count())
            {
                flag = true;
            }
            else
            {
                int index = 0;
                flag = false;
                while (index <= checked(this.Count() - 1) & !flag)
                {
                    if (this.MyList[index].Key().CompareTo((object)SetB[index].Key()) != 0)
                        flag = true;
                    checked { ++index; }
                }
            }
            return !flag;
        }

        public bool IsProperSubsetOf(ref DictionarySet SetB)
        {
            int index1 = 0;
            int index2 = 0;
            while ((long)index1 < this.MyList.Count() & index2 < SetB.Count())
            {
                if (this.MyList[index1].Key().CompareTo((object)SetB[index2].Key()) == 0)
                {
                    checked { ++index1; }
                    checked { ++index2; }
                }
                else
                    checked { ++index2; }
            }
            return (long)index1 >= this.MyList.Count();
        }

        public DictionarySet.IMember this[int Index]
        {
            get
            {
                if (Index >= 0 & (long)Index < this.MyList.Count())
                    return this.MyList[Index];
                return (DictionarySet.IMember)null;
            }
        }

        public DictionarySet.IMember get_ByKey(IComparable Key)
        {
            int index = this.IndexOf(Key);
            if (index >= 0 & (long)index < this.MyList.Count())
                return this.MyList[index];
            return (DictionarySet.IMember)null;
        }

        public bool Add(DictionarySet SetB)
        {
            return this.UnionWith(SetB);
        }

        public bool Add(params DictionarySet.IMember[] Items)
        {
            bool flag = false;
            int num = checked(((IEnumerable<DictionarySet.IMember>)Items).Count<DictionarySet.IMember>() - 1);
            int index = 0;
            while (index <= num)
            {
                if (this.MyList.Add(Items[index]))
                    flag = true;
                checked { ++index; }
            }
            return flag;
        }

        internal interface IMember
        {
            IComparable Key();

            DictionarySet.MemberResult Union(DictionarySet.IMember Item);

            DictionarySet.MemberResult Difference(DictionarySet.IMember Item);

            DictionarySet.MemberResult Intersect(DictionarySet.IMember Item);
        }


        internal class SortArray
        {
            private int m_ArraySize;
            private DictionarySet.IMember[] m_List;
            private int m_Count;
            private short m_BlockSize;

            public SortArray()
            {
                this.m_Count = 0;
                this.m_ArraySize = 0;
                this.m_BlockSize = (short)100;
                this.ResizeArray(this.m_Count);
            }

            public DictionarySet.IMember this[int Index]
            {
                get
                {
                    if (Index >= 0 & Index < this.m_Count)
                        return this.m_List[Index];
                    return (DictionarySet.IMember)null;
                }
            }

            internal void Remove(int Index)
            {
                if (!(Index >= 0 & Index < this.m_Count))
                    return;
                this.m_List[Index] = (DictionarySet.IMember)null;
            }

            public long Count()
            {
                return (long)this.m_Count;
            }

            public void ResizeArray(int RequiredSize)
            {
                if (RequiredSize > this.m_ArraySize | Math.Abs(checked(RequiredSize - this.m_ArraySize)) > (int)this.m_BlockSize)
                {
                    this.m_ArraySize = checked(unchecked(RequiredSize / (int)this.m_BlockSize) + 1 * (int)this.m_BlockSize);
                    this.m_List = (DictionarySet.IMember[])Utils.CopyArray((Array)this.m_List, (Array)new DictionarySet.IMember[checked(this.m_ArraySize - 1 + 1)]);
                }
                this.m_Count = RequiredSize;
            }

            public void Clear()
            {
                this.m_ArraySize = 0;
                this.m_Count = 0;
                this.m_List = new DictionarySet.IMember[2];
            }

            private DictionarySet.SortArray.InsertIndexInfo InsertIndex(ref IComparable Key)
            {
                DictionarySet.SortArray.InsertIndexInfo insertIndexInfo = new DictionarySet.SortArray.InsertIndexInfo();
                insertIndexInfo.Found = 0;
                if (this.m_Count == 0)
                    insertIndexInfo.Index = 0;
                else if (Key.CompareTo((object)this.m_List[0].Key()) < 0)
                    insertIndexInfo.Index = 0;
                else if (Key.CompareTo((object)this.m_List[checked(this.m_Count - 1)].Key()) > 0)
                {
                    insertIndexInfo.Index = this.m_Count;
                }
                else
                {
                    int num1 = checked(this.m_Count - 1);
                    int num2 = 0;
                    bool flag = false;
                    do
                    {
                        int index = checked((int)Math.Round(unchecked((double)checked(num2 + num1) / 2.0)));
                        if (num2 > num1)
                        {
                            insertIndexInfo.Index = num2;
                            flag = true;
                        }
                        else if (this.m_List[index].Key().CompareTo((object)Key) == 0)
                        {
                            insertIndexInfo.Index = index;
                            insertIndexInfo.Found = -1;
                            flag = true;
                        }
                        else if (this.m_List[index].Key().CompareTo((object)Key) < 0)
                            num2 = checked(index + 1);
                        else
                            num1 = checked(index - 1);
                    }
                    while (!flag);
                }
                return insertIndexInfo;
            }

            public bool Add(DictionarySet.IMember Item)
            {
                IComparable Key = Item.Key();
                DictionarySet.SortArray.InsertIndexInfo insertIndexInfo = this.InsertIndex(ref Key);
                bool flag = true;
                if ((uint)insertIndexInfo.Found > 0U)
                {
                    DictionarySet.MemberResult memberResult = this.m_List[insertIndexInfo.Index].Union(Item);
                    if (memberResult != null && memberResult.NewObject != null)
                    {
                        this.m_List[insertIndexInfo.Index] = memberResult.NewObject;
                        flag = memberResult.SetChanged;
                    }
                }
                else
                {
                    flag = true;
                    this.ResizeArray(checked(this.m_Count + 1));
                    int num1 = checked(this.m_Count - 1);
                    int num2 = checked(insertIndexInfo.Index + 1);
                    int index = num1;
                    while (index >= num2)
                    {
                        this.m_List[index] = this.m_List[checked(index - 1)];
                        checked { index += -1; }
                    }
                    this.m_List[insertIndexInfo.Index] = Item;
                }
                return flag;
            }

            public short Blocksize
            {
                get
                {
                    return this.m_BlockSize;
                }
                set
                {
                    if (value >= (short)1)
                        this.m_BlockSize = value;
                    else
                        this.m_BlockSize = (short)1;
                }
            }

            internal void RemoveNullItems()
            {
                int index = 0;
                int RequiredSize = 0;
                while (index < this.m_Count)
                {
                    if (this.m_List[index] == null)
                        checked { ++index; }
                    else if (RequiredSize == index)
                    {
                        checked { ++RequiredSize; }
                        checked { ++index; }
                    }
                    else
                    {
                        this.m_List[RequiredSize] = this.m_List[index];
                        checked { ++RequiredSize; }
                        checked { ++index; }
                    }
                }
                this.ResizeArray(RequiredSize);
            }


            private class InsertIndexInfo
            {
                public int Found;
                public int Index;

                [DebuggerNonUserCode]
                public InsertIndexInfo()
                {
                }
            }
        }


        internal class MemberResult
        {
            public DictionarySet.IMember NewObject;
            public bool SetChanged;

            public MemberResult(DictionarySet.IMember NewObject)
            {
                this.NewObject = NewObject;
                this.SetChanged = false;
            }

            public MemberResult(DictionarySet.IMember NewObject, bool SetChanged)
            {
                this.NewObject = NewObject;
                this.SetChanged = SetChanged;
            }
        }


        private class SetOperation
        {
            public bool Changed;
            public DictionarySet.SortArray List;

            public SetOperation()
            {
                this.List = new DictionarySet.SortArray();
                this.Changed = false;
            }
        }


        public enum Compare
        {
            Equal,
            UnEqual,
            Subset
        }
    }
}