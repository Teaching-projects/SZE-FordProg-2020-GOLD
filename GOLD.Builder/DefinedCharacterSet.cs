using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.CompilerServices;

namespace GOLD.Builder
{

    internal class DefinedCharacterSet : CharacterSetBuild
    {
        private string m_Name;
        private string m_Definition;
        private string m_Comment;
        private string m_Type;
        internal NumberSet Dependacy;
        internal ISetExpression Exp;

        public DefinedCharacterSet()
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
        }

        public DefinedCharacterSet(string Name, string CharSet)
          : base(CharSet)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            this.m_Name = Name.Trim();
        }

        public DefinedCharacterSet(string Name, CharacterSetBuild CharSet)
          : base((CharacterSet)CharSet)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            this.m_Name = Name.Trim();
        }

        public DefinedCharacterSet(string Name)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            this.m_Name = Name.Trim();
        }

        public DefinedCharacterSet(string Name, params object[] Values)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            int upperBound = Values.GetUpperBound(0);
            int index1 = 0;
            while (index1 <= upperBound)
            {
                if (Values[index1] is CharacterSetRange)
                {
                    this.AddRange(Conversions.ToInteger(NewLateBinding.LateGet(Values[index1], (System.Type)null, "First", new object[0], (string[])null, (System.Type[])null, (bool[])null)), Conversions.ToInteger(NewLateBinding.LateGet(Values[index1], (System.Type)null, "last", new object[0], (string[])null, (System.Type[])null, (bool[])null)));
                }
                else
                {
                    object[] objArray1 = new object[1];
                    object[] objArray2 = objArray1;
                    object[] objArray3 = Values;
                    object[] objArray4 = objArray3;
                    int index2 = index1;
                    int index3 = index2;
                    object objectValue = RuntimeHelpers.GetObjectValue(objArray4[index3]);
                    objArray2[0] = objectValue;
                    object[] objArray5 = objArray1;
                    object[] Arguments = objArray5;
                    bool[] flagArray = new bool[1] { true };
                    bool[] CopyBack = flagArray;
                    NewLateBinding.LateCall((object)this, (System.Type)null, "Add", Arguments, (string[])null, (System.Type[])null, CopyBack, true);
                    if (flagArray[0])
                        objArray3[index2] = RuntimeHelpers.GetObjectValue(objArray5[0]);
                }
                checked { ++index1; }
            }
            this.m_Name = Name.Trim();
        }

        public DefinedCharacterSet(string Name, int StartValue, int EndValue)
        {
            this.m_Name = "";
            this.m_Definition = "";
            this.m_Comment = "";
            this.m_Type = "";
            this.Dependacy = new NumberSet(new int[0]);
            this.AddRange(StartValue, EndValue);
            this.m_Name = Name.Trim();
        }

        public string Comment
        {
            get
            {
                return this.m_Comment;
            }
            set
            {
                this.m_Comment = value;
            }
        }

        public string Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        public string Definition
        {
            get
            {
                return this.m_Definition;
            }
            set
            {
                this.m_Definition = value;
            }
        }
    }
}