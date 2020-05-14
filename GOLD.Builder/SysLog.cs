
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;

namespace GOLD.Builder
{
    public class SysLog
    {
        private SysLogItem[] m_Array;
        private short m_Count;
        private bool m_Locked;

        [DebuggerNonUserCode]
        public SysLog()
        {
        }

        public void Clear()
        {
            this.m_Array = (SysLogItem[])null;
            this.m_Count = (short)0;
        }

        public int Count()
        {
            return (int)this.m_Count;
        }

        public bool Locked
        {
            get
            {
                return this.m_Locked;
            }
            set
            {
                this.m_Locked = value;
            }
        }

        public SysLogItem this[int Index]
        {
            get
            {
                if (Index >= 0 & Index < (int)this.m_Count)
                    return this.m_Array[Index];
                return (SysLogItem)null;
            }
            set
            {
                if (!(Index >= 0 & Index < (int)this.m_Count))
                    return;
                this.m_Array[Index] = value;
            }
        }

        public void Add(SysLogItem Item)
        {
            if (this.Locked)
                return;
            checked { ++this.m_Count; }
            this.m_Array = (SysLogItem[])Utils.CopyArray((Array)this.m_Array, (Array)new SysLogItem[checked((int)this.m_Count - 1 + 1)]);
            this.m_Array[checked((int)this.m_Count - 1)] = Item;
        }

        public void Add(string Title, string Description)
        {
            if (this.Locked)
                return;
            SysLogItem sysLogItem1 = new SysLogItem();
            SysLogItem sysLogItem2 = sysLogItem1;
            sysLogItem2.Section = SysLogSection.Grammar;
            sysLogItem2.Alert = SysLogAlert.Warning;
            sysLogItem2.Title = Title;
            sysLogItem2.Description = Description;
            this.Add(sysLogItem1);
        }

        public void Add(SysLogSection Section, SysLogAlert Alert, string Title)
        {
            if (this.Locked)
                return;
            SysLogItem sysLogItem1 = new SysLogItem();
            SysLogItem sysLogItem2 = sysLogItem1;
            sysLogItem2.Section = Section;
            sysLogItem2.Alert = Alert;
            sysLogItem2.Title = Title;
            this.Add(sysLogItem1);
        }

        public void Add(
          SysLogSection Section,
          SysLogAlert Alert,
          string Title,
          string Description,
          string Index = "")
        {
            if (this.Locked)
                return;
            SysLogItem sysLogItem1 = new SysLogItem();
            SysLogItem sysLogItem2 = sysLogItem1;
            sysLogItem2.Section = Section;
            sysLogItem2.Alert = Alert;
            sysLogItem2.Title = Title;
            sysLogItem2.Description = Description;
            sysLogItem2.Index = Index;
            if (Operators.CompareString(sysLogItem2.Description, "", true) != 0 & !sysLogItem2.Description.EndsWith("."))
                sysLogItem2.Description += ".";
            this.Add(sysLogItem1);
        }

        public int AlertCount(SysLogAlert Alert)
        {
            int num1 = checked((int)this.m_Count - 1);
            int index = 0;
            int num2=0;
            while (index <= num1)
            {
                if (this.m_Array[index].Alert == Alert)
                    checked { ++num2; }
                checked { ++index; }
            }
            return num2;
        }

        public int SectionCount(SysLogSection Section)
        {
            int num1 = checked((int)this.m_Count - 1);
            int index = 0;
            int num2= 0;
            while (index <= num1)
            {
                if (this.m_Array[index].Section == Section)
                    checked { ++num2; }
                checked { ++index; }
            }
            return num2;
        }

        public string DumpSection(SysLogSection Section)
        {
            string str = "";
            int num = checked((int)this.m_Count - 1);
            int index = 0;
            while (index <= num)
            {
                if (this.m_Array[index].Section == Section)
                {
                    str = str + "* " + this.m_Array[index].Title + "\r\n";
                    if (Operators.CompareString(this.m_Array[index].Description, "", true) != 0)
                        str = str + "  " + this.m_Array[index].Title + "\r\n";
                }
                checked { ++index; }
            }
            return str;
        }
    }
}