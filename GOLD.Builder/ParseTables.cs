using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;

namespace GOLD.Builder
{
    internal class ParseTables
    {
        protected VariableList m_Properties;
        protected SymbolList m_Symbol;
        protected CharacterSetList m_CharSet;
        protected GroupList m_Group;
        protected FAStateList m_DFA;
        protected ProductionList m_Production;
        protected LRStateList m_LALR;
        protected Symbol m_StartSymbol;

        public Symbol StartSymbol
        {
            get
            {
                return this.m_StartSymbol;
            }
            set
            {
                this.m_StartSymbol = value;
            }
        }

        public ParseTables()
        {
            this.m_Properties = new VariableList();
            this.m_Symbol = new SymbolList();
            this.m_CharSet = new CharacterSetList();
            this.m_Group = new GroupList();
            this.m_DFA = new FAStateList();
            this.m_Production = new ProductionList();
            this.m_LALR = new LRStateList();
        }

        public bool IsLoaded()
        {
            return this.m_DFA.Count >= 1 & this.m_LALR.Count >= 1;
        }

        public VariableList Properties
        {
            get
            {
                return this.m_Properties;
            }
            set
            {
                this.m_Properties = value;
            }
        }

        public SymbolList Symbol
        {
            get
            {
                return this.m_Symbol;
            }
            set
            {
                this.m_Symbol = value;
            }
        }

        public CharacterSetList CharSet
        {
            get
            {
                return this.m_CharSet;
            }
            set
            {
                this.m_CharSet = value;
            }
        }

        public ProductionList Production
        {
            get
            {
                return this.m_Production;
            }
            set
            {
                this.m_Production = value;
            }
        }

        public FAStateList DFA
        {
            get
            {
                return this.m_DFA;
            }
            set
            {
                this.m_DFA = value;
            }
        }

        public LRStateList LALR
        {
            get
            {
                return this.m_LALR;
            }
            set
            {
                this.m_LALR = value;
            }
        }

        public GroupList Group
        {
            get
            {
                return this.m_Group;
            }
            set
            {
                this.m_Group = value;
            }
        }

        public void Clear()
        {
            this.m_Properties.Clear();
            this.m_Symbol.Clear();
            this.m_CharSet.Clear();
            this.m_Group.Clear();
            this.m_DFA.Clear();
            this.m_Production.Clear();
            this.m_LALR.Clear();
        }

        public bool Load(ref string Path)
        {
            SimpleDB.Reader reader = new SimpleDB.Reader();
            this.Clear();
            reader.Open(Path);
            string Left = reader.Header();
            bool flag = Operators.CompareString(Left, "GOLD Parser Tables/v1.0", true) != 0 ? Operators.CompareString(Left, "GOLD Parser Tables/v5.0", true) == 0 && this.LoadVer5(reader) : this.LoadVer1(reader);
            reader.Close();
            return flag;
        }

        private bool LoadVer5(SimpleDB.Reader EGT)
        {
            bool flag1;
            try
            {
                flag1 = true;
                while (!(EGT.EndOfFile() | !flag1))
                {
                    EGT.GetNextRecord();
                    ParseTables.EGTRecord egtRecord = (ParseTables.EGTRecord)EGT.RetrieveByte();
                    switch (egtRecord)
                    {
                        case ParseTables.EGTRecord.DFAState:
                            int index1 = EGT.RetrieveInt16();
                            bool flag2 = EGT.RetrieveBoolean();
                            int index2 = EGT.RetrieveInt16();
                            EGT.RetrieveEntry();
                            this.m_DFA[index1] = !flag2 ? new FAState() : new FAState(this.m_Symbol[index2]);
                            while (!EGT.RecordComplete())
                            {
                                int index3 = EGT.RetrieveInt16();
                                int Target = EGT.RetrieveInt16();
                                EGT.RetrieveEntry();
                                this.m_DFA[index1].Edges().Add(new FAEdge(this.m_CharSet[index3], Target));
                            }
                            break;
                        case ParseTables.EGTRecord.InitialStates:
                            this.m_DFA.InitialState = checked((short)EGT.RetrieveInt16());
                            this.m_LALR.InitialState = checked((short)EGT.RetrieveInt16());
                            break;
                        case ParseTables.EGTRecord.LRState:
                            int index4 = EGT.RetrieveInt16();
                            EGT.RetrieveEntry();
                            this.m_LALR[index4] = new LRState();
                            while (!EGT.RecordComplete())
                            {
                                int index3 = EGT.RetrieveInt16();
                                int num1 = EGT.RetrieveInt16();
                                int num2 = EGT.RetrieveInt16();
                                EGT.RetrieveEntry();
                                this.m_LALR[index4].Add(new LRAction(this.m_Symbol[index3], (LRActionType)num1, checked((short)num2)));
                            }
                            break;
                        case ParseTables.EGTRecord.Production:
                            int index5 = EGT.RetrieveInt16();
                            int index6 = EGT.RetrieveInt16();
                            EGT.RetrieveEntry();
                            this.m_Production[index5] = new Production(this.m_Symbol[index6], checked((short)index5));
                            while (!EGT.RecordComplete())
                            {
                                int index3 = EGT.RetrieveInt16();
                                this.m_Production[index5].Handle().Add(this.m_Symbol[index3]);
                            }
                            break;
                        case ParseTables.EGTRecord.Symbol:
                            int index7 = EGT.RetrieveInt16();
                            string Name = EGT.RetrieveString();
                            SymbolType Type = (SymbolType)EGT.RetrieveInt16();
                            this.m_Symbol[index7] = new Symbol(Name, Type, checked((short)index7));
                            break;
                        case ParseTables.EGTRecord.CharRanges:
                            CharacterSet characterSet = new CharacterSet();
                            int index8 = EGT.RetrieveInt16();
                            EGT.RetrieveInt16();
                            EGT.RetrieveInt16();
                            EGT.RetrieveEntry();
                            while (!EGT.RecordComplete())
                                characterSet.AddRange(EGT.RetrieveInt16(), EGT.RetrieveInt16());
                            characterSet.TableIndex = index8;
                            this.m_CharSet[index8] = characterSet;
                            break;
                        case ParseTables.EGTRecord.Group:
                            Group group1 = new Group();
                            Group group2 = group1;
                            int index9 = EGT.RetrieveInt16();
                            group2.Name = EGT.RetrieveString();
                            group2.Container = this.m_Symbol[EGT.RetrieveInt16()];
                            group2.Start = this.m_Symbol[EGT.RetrieveInt16()];
                            group2.End = this.m_Symbol[EGT.RetrieveInt16()];
                            group2.Advance = (AdvanceMode)EGT.RetrieveInt16();
                            group2.Ending = (EndingMode)EGT.RetrieveInt16();
                            EGT.RetrieveEntry();
                            int num3 = EGT.RetrieveInt16();
                            int num4 = 1;
                            while (num4 <= num3)
                            {
                                group2.Nesting.Add(EGT.RetrieveInt16());
                                checked { ++num4; }
                            }
                            group1.Container.Group = group1;
                            group1.Start.Group = group1;
                            group1.End.Group = group1;
                            this.m_Group[index9] = group1;
                            break;
                        case ParseTables.EGTRecord.Property:
                            EGT.RetrieveInt16();
                            this.m_Properties.Add(EGT.RetrieveString(), (object)EGT.RetrieveString());
                            break;
                        case ParseTables.EGTRecord.TableCounts:
                            this.m_Symbol = new SymbolList(EGT.RetrieveInt16());
                            this.m_CharSet = new CharacterSetList(EGT.RetrieveInt16());
                            this.m_Production = new ProductionList(EGT.RetrieveInt16());
                            this.m_DFA = new FAStateList(EGT.RetrieveInt16());
                            this.m_LALR = new LRStateList(EGT.RetrieveInt16());
                            this.m_Group = new GroupList(EGT.RetrieveInt16());
                            break;
                        default:
                            throw new Exception("File Error. A record of type '" + Conversions.ToString(Strings.ChrW((int)egtRecord)) + "' was read. This is not a valid code.");
                    }
                }
                EGT.Close();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                throw ex;
            }
            return flag1;
        }

        private bool LoadVer1(SimpleDB.Reader CGT)
        {
            bool flag1 = true;
            while (!(CGT.EndOfFile() | !flag1))
            {
                CGT.GetNextRecord();
                switch (CGT.RetrieveByte())
                {
                    case 67:
                        int index1 = CGT.RetrieveInt16();
                        this.m_CharSet[index1] = new CharacterSet(CGT.RetrieveString())
                        {
                            TableIndex = index1
                        };
                        break;
                    case 68:
                        FAState faState = new FAState();
                        int index2 = CGT.RetrieveInt16();
                        bool flag2 = CGT.RetrieveBoolean();
                        int index3 = CGT.RetrieveInt16();
                        CGT.RetrieveEntry();
                        this.m_DFA[index2] = !flag2 ? new FAState() : new FAState(this.m_Symbol[index3]);
                        while (!CGT.RecordComplete())
                        {
                            int index4 = CGT.RetrieveInt16();
                            int Target = CGT.RetrieveInt16();
                            CGT.RetrieveEntry();
                            this.m_DFA[index2].AddEdge(new FAEdge(this.m_CharSet[index4], Target));
                        }
                        break;
                    case 73:
                        this.m_DFA.InitialState = checked((short)CGT.RetrieveInt16());
                        this.m_LALR.InitialState = checked((short)CGT.RetrieveInt16());
                        break;
                    case 76:
                        int index5 = CGT.RetrieveInt16();
                        CGT.RetrieveEntry();
                        this.m_LALR[index5] = new LRState();
                        while (!CGT.RecordComplete())
                        {
                            int index4 = CGT.RetrieveInt16();
                            int num1 = CGT.RetrieveInt16();
                            int num2 = CGT.RetrieveInt16();
                            CGT.RetrieveEntry();
                            this.m_LALR[index5].Add(new LRAction(this.m_Symbol[index4], (LRActionType)num1, checked((short)num2)));
                        }
                        break;
                    case 80:
                        this.m_Properties["Name"].Value = CGT.RetrieveString();
                        this.m_Properties["Version"].Value = CGT.RetrieveString();
                        this.m_Properties["Author"].Value = CGT.RetrieveString();
                        this.m_Properties["About"].Value = CGT.RetrieveString();
                        this.m_Properties["Case Sensitive"].Value = Conversions.ToString(CGT.RetrieveBoolean());
                        CGT.RetrieveInt16();
                        break;
                    case 82:
                        int index6 = CGT.RetrieveInt16();
                        int index7 = CGT.RetrieveInt16();
                        CGT.RetrieveEntry();
                        this.m_Production[index6] = new Production(this.m_Symbol[index7], checked((short)index6));
                        while (!CGT.RecordComplete())
                        {
                            int index4 = CGT.RetrieveInt16();
                            this.m_Production[index6].Handle().Add(this.m_Symbol[index4]);
                        }
                        break;
                    case 83:
                        int index8 = CGT.RetrieveInt16();
                        string Name = CGT.RetrieveString();
                        SymbolType Type = (SymbolType)CGT.RetrieveInt16();
                        this.m_Symbol[index8] = new Symbol(Name, Type, checked((short)index8));
                        break;
                    case 84:
                        this.m_Symbol = new SymbolList(CGT.RetrieveInt16());
                        this.m_CharSet = new CharacterSetList(CGT.RetrieveInt16());
                        this.m_Production = new ProductionList(CGT.RetrieveInt16());
                        this.m_DFA = new FAStateList(CGT.RetrieveInt16());
                        this.m_LALR = new LRStateList(CGT.RetrieveInt16());
                        break;
                    default:
                        flag1 = false;
                        break;
                }
            }
            Symbol symbol1 = (Symbol)null;
            Symbol symbol2 = (Symbol)null;
            Symbol symbol3 = (Symbol)null;
            int num = checked(this.m_Symbol.Count() - 1);
            int index = 0;
            while (index <= num)
            {
                Symbol symbol4 = this.m_Symbol[index];
                switch (symbol4.Type)
                {
                    case SymbolType.Noise:
                        if (symbol3 == null)
                            symbol3 = symbol4;
                        break;
                    case SymbolType.GroupStart:
                        symbol1 = symbol4;
                        break;
                    case SymbolType.GroupEnd:
                        symbol2 = symbol4;
                        break;
                }
                checked { ++index; }
            }
            if (symbol1 != null)
            {
                Group group = new Group();
                this.m_Group.Add(group);
                group.TableIndex = (short)0;
                group.Name = "Comment Block";
                group.Container = symbol3;
                group.Nesting.Add((int)group.TableIndex);
                group.Advance = AdvanceMode.Token;
                group.Ending = EndingMode.Closed;
                group.Start = symbol1;
                group.End = symbol2;
                group.Start.Group = group;
                group.End.Group = group;
            }
            return flag1;
        }

        internal bool SaveVer1(ref string Path)
        {
            SimpleDB.Writer writer1 = new SimpleDB.Writer();
            CharacterSet characterSet1 = new CharacterSet(new int[1]
            {
      0
            });
            int num1 = -1;
            int num2 = -1;
            int num3 = -1;
            short num4 = checked((short)(this.m_Group.Count - 1));
            short num5 = 0;
            while ((int)num5 <= (int)num4)
            {
                Group group = this.m_Group[(int)num5];
                if (Operators.CompareString(group.Name.ToUpper(), "COMMENT LINE", true) == 0)
                    num1 = (int)group.Start.TableIndex;
                else if (Operators.CompareString(group.Name.ToUpper(), "COMMENT BLOCK", true) == 0)
                {
                    num2 = (int)group.Start.TableIndex;
                    num3 = (int)group.End.TableIndex;
                }
                checked { ++num5; }
            }
            bool flag;
            try
            {
                writer1.Open(ref Path, "GOLD Parser Tables/v1.0");
                writer1.NewRecord();
                writer1.StoreByte((byte)80);
                writer1.StoreString(ref this.Properties["Name"].Value);
                writer1.StoreString(ref this.Properties["Version"].Value);
                writer1.StoreString(ref this.Properties["Author"].Value);
                writer1.StoreString(ref this.Properties["About"].Value);
                writer1.StoreBoolean(Operators.CompareString(Strings.UCase(this.Properties["Case Sensitive"].Value), "TRUE", true) == 0);
                writer1.StoreInt16((int)this.StartSymbol.TableIndex);
                writer1.NewRecord();
                writer1.StoreByte((byte)84);
                writer1.StoreInt16(this.m_Symbol.Count());
                writer1.StoreInt16(this.m_CharSet.Count);
                writer1.StoreInt16(this.m_Production.Count());
                writer1.StoreInt16(this.m_DFA.Count);
                writer1.StoreInt16(this.m_LALR.Count);
                writer1.NewRecord();
                writer1.StoreByte((byte)73);
                writer1.StoreInt16((int)this.m_DFA.InitialState);
                writer1.StoreInt16((int)this.m_LALR.InitialState);
                short num6 = checked((short)(this.m_CharSet.Count - 1));
                short num7 = 0;
                while ((int)num7 <= (int)num6)
                {
                    CharacterSet characterSet2 = new CharacterSet((NumberSet)this.m_CharSet[(int)num7]);
                    characterSet2.DifferenceWith((NumberSet)characterSet1);
                    writer1.NewRecord();
                    writer1.StoreByte((byte)67);
                    writer1.StoreInt16((int)num7);
                    SimpleDB.Writer writer2 = writer1;
                    string str = characterSet2.ToString();
                    ref string local = ref str;
                    writer2.StoreString(ref local);
                    checked { ++num7; }
                }
                short num8 = checked((short)(this.m_Symbol.Count() - 1));
                short num9 = 0;
                while ((int)num9 <= (int)num8)
                {
                    Symbol symbol1 = this.m_Symbol[(int)num9];
                    SymbolType symbolType;
                    switch (symbol1.Type)
                    {
                        case SymbolType.GroupStart:
                            symbolType = (int)num9 != num1 ? ((int)num9 != num2 ? SymbolType.Content : SymbolType.GroupStart) : SymbolType.LEGACYCommentLine;
                            break;
                        case SymbolType.GroupEnd:
                            symbolType = (int)num9 != num3 ? SymbolType.Content : SymbolType.GroupEnd;
                            break;
                        default:
                            symbolType = symbol1.Type;
                            break;
                    }
                    writer1.NewRecord();
                    writer1.StoreByte((byte)83);
                    writer1.StoreInt16((int)num9);
                    SimpleDB.Writer writer2 = writer1;
                    Symbol symbol2 = symbol1;
                    string name = symbol2.Name;
                    ref string local = ref name;
                    writer2.StoreString(ref local);
                    symbol2.Name = name;
                    writer1.StoreInt16((int)symbolType);
                    checked { ++num9; }
                }
                short num10 = checked((short)(this.m_Production.Count() - 1));
                short num11 = 0;
                while ((int)num11 <= (int)num10)
                {
                    writer1.NewRecord();
                    writer1.StoreByte((byte)82);
                    writer1.StoreInt16((int)num11);
                    writer1.StoreInt16((int)this.m_Production[(int)num11].Head.TableIndex);
                    writer1.StoreEmpty();
                    short num12 = checked((short)(this.m_Production[(int)num11].Handle().Count() - 1));
                    short num13 = 0;
                    while ((int)num13 <= (int)num12)
                    {
                        writer1.StoreInt16((int)this.m_Production[(int)num11].Handle()[(int)num13].TableIndex);
                        checked { ++num13; }
                    }
                    checked { ++num11; }
                }
                short num14 = checked((short)(this.DFA.Count - 1));
                short num15 = 0;
                while ((int)num15 <= (int)num14)
                {
                    writer1.NewRecord();
                    writer1.StoreByte((byte)68);
                    writer1.StoreInt16((int)num15);
                    if (this.DFA[(int)num15].Accept == null)
                    {
                        writer1.StoreBoolean(false);
                        writer1.StoreInt16(-1);
                    }
                    else
                    {
                        writer1.StoreBoolean(true);
                        writer1.StoreInt16((int)this.DFA[(int)num15].Accept.TableIndex);
                    }
                    writer1.StoreEmpty();
                    short num12 = checked((short)(this.DFA[(int)num15].Edges().Count() - 1));
                    short num13 = 0;
                    while ((int)num13 <= (int)num12)
                    {
                        writer1.StoreInt16(this.DFA[(int)num15].Edges()[(int)num13].Characters.TableIndex);
                        writer1.StoreInt16(this.DFA[(int)num15].Edges()[(int)num13].Target);
                        writer1.StoreEmpty();
                        checked { ++num13; }
                    }
                    checked { ++num15; }
                }
                short num16 = checked((short)(this.LALR.Count - 1));
                short num17 = 0;
                while ((int)num17 <= (int)num16)
                {
                    writer1.NewRecord();
                    writer1.StoreByte((byte)76);
                    writer1.StoreInt16((int)num17);
                    writer1.StoreEmpty();
                    short num12 = checked((short)(this.LALR[(int)num17].Count - 1));
                    short index = 0;
                    while ((int)index <= (int)num12)
                    {
                        writer1.StoreInt16((int)this.LALR[(int)num17][index].SymbolIndex());
                        writer1.StoreInt16((int)this.LALR[(int)num17][index].Type());
                        writer1.StoreInt16((int)this.LALR[(int)num17][index].Value());
                        writer1.StoreEmpty();
                        checked { ++index; }
                    }
                    checked { ++num17; }
                }
                writer1.Close();
                flag = true;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                flag = false;
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        internal bool SaveVer5(ref string Path)
        {
            SimpleDB.Writer writer1 = new SimpleDB.Writer();
            bool flag;
            try
            {
                writer1.Open(ref Path, "GOLD Parser Tables/v5.0");
                writer1.NewRecord();
                writer1.StoreByte((byte)112);
                writer1.StoreInt16(0);
                SimpleDB.Writer writer2 = writer1;
                string str = "Name";
                ref string local1 = ref str;
                writer2.StoreString(ref local1);
                writer1.StoreString(ref this.Properties["Name"].Value);
                writer1.NewRecord();
                writer1.StoreByte((byte)112);
                writer1.StoreInt16(1);
                SimpleDB.Writer writer3 = writer1;
                str = "Version";
                ref string local2 = ref str;
                writer3.StoreString(ref local2);
                writer1.StoreString(ref this.Properties["Version"].Value);
                writer1.NewRecord();
                writer1.StoreByte((byte)112);
                writer1.StoreInt16(2);
                SimpleDB.Writer writer4 = writer1;
                str = "Author";
                ref string local3 = ref str;
                writer4.StoreString(ref local3);
                writer1.StoreString(ref this.Properties["Author"].Value);
                writer1.NewRecord();
                writer1.StoreByte((byte)112);
                writer1.StoreInt16(3);
                SimpleDB.Writer writer5 = writer1;
                str = "About";
                ref string local4 = ref str;
                writer5.StoreString(ref local4);
                writer1.StoreString(ref this.Properties["About"].Value);
                writer1.NewRecord();
                writer1.StoreByte((byte)112);
                writer1.StoreInt16(4);
                SimpleDB.Writer writer6 = writer1;
                str = "Character Set";
                ref string local5 = ref str;
                writer6.StoreString(ref local5);
                writer1.StoreString(ref this.Properties["Character Set"].Value);
                writer1.NewRecord();
                writer1.StoreByte((byte)112);
                writer1.StoreInt16(5);
                SimpleDB.Writer writer7 = writer1;
                str = "Character Mapping";
                ref string local6 = ref str;
                writer7.StoreString(ref local6);
                writer1.StoreString(ref this.Properties["Character Mapping"].Value);
                writer1.NewRecord();
                writer1.StoreByte((byte)112);
                writer1.StoreInt16(6);
                SimpleDB.Writer writer8 = writer1;
                str = "Generated By";
                ref string local7 = ref str;
                writer8.StoreString(ref local7);
                writer1.StoreString(ref this.Properties["Generated By"].Value);
                writer1.NewRecord();
                writer1.StoreByte((byte)112);
                writer1.StoreInt16(7);
                SimpleDB.Writer writer9 = writer1;
                str = "Generated Date";
                ref string local8 = ref str;
                writer9.StoreString(ref local8);
                writer1.StoreString(ref this.Properties["Generated Date"].Value);
                writer1.NewRecord();
                writer1.StoreByte((byte)116);
                writer1.StoreInt16(this.m_Symbol.Count());
                writer1.StoreInt16(this.m_CharSet.Count);
                writer1.StoreInt16(this.m_Production.Count());
                writer1.StoreInt16(this.m_DFA.Count);
                writer1.StoreInt16(this.m_LALR.Count);
                writer1.StoreInt16(this.m_Group.Count);
                writer1.NewRecord();
                writer1.StoreByte((byte)73);
                writer1.StoreInt16((int)this.m_DFA.InitialState);
                writer1.StoreInt16((int)this.m_LALR.InitialState);
                short num1 = checked((short)(this.m_CharSet.Count - 1));
                short num2 = 0;
                while ((int)num2 <= (int)num1)
                {
                    NumberRangeList numberRangeList = this.m_CharSet[(int)num2].RangeList();
                    writer1.NewRecord();
                    writer1.StoreByte((byte)99);
                    writer1.StoreInt16((int)num2);
                    writer1.StoreInt16(0);
                    writer1.StoreInt16(numberRangeList.Count);
                    writer1.StoreEmpty();
                    short num3 = checked((short)(numberRangeList.Count - 1));
                    short num4 = 0;
                    while ((int)num4 <= (int)num3)
                    {
                        writer1.StoreInt16(numberRangeList[(int)num4].First);
                        writer1.StoreInt16(numberRangeList[(int)num4].Last);
                        checked { ++num4; }
                    }
                    checked { ++num2; }
                }
                short num5 = checked((short)(this.m_Symbol.Count() - 1));
                short num6 = 0;
                while ((int)num6 <= (int)num5)
                {
                    writer1.NewRecord();
                    writer1.StoreByte((byte)83);
                    writer1.StoreInt16((int)num6);
                    Symbol symbol1 = this.m_Symbol[(int)num6];
                    SimpleDB.Writer writer10 = writer1;
                    Symbol symbol2 = symbol1;
                    str = symbol2.Name;
                    ref string local9 = ref str;
                    writer10.StoreString(ref local9);
                    symbol2.Name = str;
                    writer1.StoreInt16((int)symbol1.Type);
                    checked { ++num6; }
                }
                short num7 = checked((short)(this.m_Group.Count - 1));
                short num8 = 0;
                while ((int)num8 <= (int)num7)
                {
                    writer1.NewRecord();
                    writer1.StoreByte((byte)103);
                    Group group = this.m_Group[(int)num8];
                    writer1.StoreInt16((int)num8);
                    writer1.StoreString(ref group.Name);
                    writer1.StoreInt16((int)group.Container.TableIndex);
                    writer1.StoreInt16((int)group.Start.TableIndex);
                    writer1.StoreInt16((int)group.End.TableIndex);
                    writer1.StoreInt16((int)group.Advance);
                    writer1.StoreInt16((int)group.Ending);
                    writer1.StoreEmpty();
                    writer1.StoreInt16(group.Nesting.Count);
                    short num3 = checked((short)(group.Nesting.Count - 1));
                    short num4 = 0;
                    while ((int)num4 <= (int)num3)
                    {
                        writer1.StoreInt16(group.Nesting[(int)num4]);
                        checked { ++num4; }
                    }
                    checked { ++num8; }
                }
                short num9 = checked((short)(this.m_Production.Count() - 1));
                short num10 = 0;
                while ((int)num10 <= (int)num9)
                {
                    writer1.NewRecord();
                    writer1.StoreByte((byte)82);
                    writer1.StoreInt16((int)num10);
                    writer1.StoreInt16((int)this.m_Production[(int)num10].Head.TableIndex);
                    writer1.StoreEmpty();
                    short num3 = checked((short)(this.m_Production[(int)num10].Handle().Count() - 1));
                    short num4 = 0;
                    while ((int)num4 <= (int)num3)
                    {
                        writer1.StoreInt16((int)this.m_Production[(int)num10].Handle()[(int)num4].TableIndex);
                        checked { ++num4; }
                    }
                    checked { ++num10; }
                }
                short num11 = checked((short)(this.DFA.Count - 1));
                short num12 = 0;
                while ((int)num12 <= (int)num11)
                {
                    writer1.NewRecord();
                    writer1.StoreByte((byte)68);
                    writer1.StoreInt16((int)num12);
                    if (this.DFA[(int)num12].Accept != null)
                    {
                        writer1.StoreBoolean(true);
                        writer1.StoreInt16((int)this.DFA[(int)num12].Accept.TableIndex);
                    }
                    else
                    {
                        writer1.StoreBoolean(false);
                        writer1.StoreInt16(0);
                    }
                    writer1.StoreEmpty();
                    short num3 = checked((short)(this.DFA[(int)num12].Edges().Count() - 1));
                    short num4 = 0;
                    while ((int)num4 <= (int)num3)
                    {
                        writer1.StoreInt16(this.DFA[(int)num12].Edges()[(int)num4].Characters.TableIndex);
                        writer1.StoreInt16(this.DFA[(int)num12].Edges()[(int)num4].Target);
                        writer1.StoreEmpty();
                        checked { ++num4; }
                    }
                    checked { ++num12; }
                }
                short num13 = checked((short)(this.LALR.Count - 1));
                short num14 = 0;
                while ((int)num14 <= (int)num13)
                {
                    writer1.NewRecord();
                    writer1.StoreByte((byte)76);
                    writer1.StoreInt16((int)num14);
                    writer1.StoreEmpty();
                    short num3 = checked((short)(this.LALR[(int)num14].Count - 1));
                    short index = 0;
                    while ((int)index <= (int)num3)
                    {
                        writer1.StoreInt16((int)this.LALR[(int)num14][index].SymbolIndex());
                        writer1.StoreInt16((int)this.LALR[(int)num14][index].Type());
                        writer1.StoreInt16((int)this.LALR[(int)num14][index].Value());
                        writer1.StoreEmpty();
                        checked { ++index; }
                    }
                    checked { ++num14; }
                }
                writer1.Close();
                flag = true;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                flag = false;
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        public bool SaveXML1(string FileName)
        {
            int num1 = -1;
            int num2 = checked(this.m_Group.Count - 1);
            int index1 = 0;
            while (index1 <= num2)
            {
                Group group = this.m_Group[index1];
                if (Operators.CompareString(group.Name.ToUpper(), "COMMENT LINE", true) == 0)
                    num1 = (int)group.Start.TableIndex;
                checked { ++index1; }
            }
            string str = Strings.Space(8);
            bool flag;
            try
            {
                TextWriter textWriter = (TextWriter)new StreamWriter(FileName, false);
                textWriter.WriteLine("<?GOLDParserTables version={0}1.0{0}?>", (object)'"');
                textWriter.WriteLine("<Tables>");
                textWriter.WriteLine("{0}<Parameters>", (object)str);
                textWriter.WriteLine("{0}{0}<Parameter Name={1}Name{1} Value={1}{2}{1}/>", (object)str, (object)'"', (object)BuilderUtility.XMLText(this.m_Properties["Name"].Value));
                textWriter.WriteLine("{0}{0}<Parameter Name={1}Author{1} Value={1}{2}{1}/>", (object)str, (object)'"', (object)BuilderUtility.XMLText(this.m_Properties["Author"].Value));
                textWriter.WriteLine("{0}{0}<Parameter Name={1}Version{1} Value={1}{2}{1}/>", (object)str, (object)'"', (object)BuilderUtility.XMLText(this.m_Properties["Version"].Value));
                textWriter.WriteLine("{0}{0}<Parameter Name={1}About{1} Value={1}{2}{1}/>", (object)str, (object)'"', (object)BuilderUtility.XMLText(this.m_Properties["About"].Value));
                textWriter.WriteLine("{0}{0}<Parameter Name={1}Case Sensitive{1} Value={1}{2}{1}/>", (object)str, (object)'"', (object)BuilderUtility.XMLText(this.m_Properties["Case Sensitive"].Value));
                textWriter.WriteLine("{0}{0}<Parameter Name={1}Start Symbol{1} Value={1}{2}{1}/>", (object)str, (object)'"', (object)BuilderUtility.XMLText(this.m_StartSymbol.Text(false)));
                textWriter.WriteLine("{0}</Parameters>", (object)str);
                textWriter.WriteLine("{0}<SymbolTable Count={1}{2}{1}>", (object)str, (object)'"', (object)this.m_Symbol.Count());
                int num3 = checked(this.m_Symbol.Count() - 1);
                int index2 = 0;
                while (index2 <= num3)
                {
                    Symbol symbol = this.m_Symbol[index2];
                    SymbolType symbolType = index2 != num1 ? symbol.Type : SymbolType.LEGACYCommentLine;
                    textWriter.WriteLine("{0}{0}<Symbol Index={1}{2}{1} Name={1}{3}{1} Kind={1}{4}{1}/>", (object)str, (object)'"', (object)index2, (object)BuilderUtility.XMLText(symbol.Name), (object)Conversion.Int((int)symbolType));
                    checked { ++index2; }
                }
                textWriter.WriteLine("{0}</SymbolTable>", (object)str);
                textWriter.WriteLine("{0}<RuleTable Count={1}{2}{1}>", (object)str, (object)'"', (object)this.m_Production.Count());
                int num4 = checked(this.m_Production.Count() - 1);
                int index3 = 0;
                while (index3 <= num4)
                {
                    Production production = this.m_Production[index3];
                    textWriter.WriteLine("{0}{0}<Rule Index={1}{2}{1} NonTerminalIndex={1}{3}{1} SymbolCount={1}{4}{1}>", (object)str, (object)'"', (object)index3, (object)production.Head.TableIndex, (object)production.Handle().Count());
                    int num5 = checked(production.Handle().Count() - 1);
                    int index4 = 0;
                    while (index4 <= num5)
                    {
                        textWriter.WriteLine("{0}{0}{0}<RuleSymbol SymbolIndex={1}{2}{1}/>", (object)str, (object)'"', (object)production.Handle()[index4].TableIndex);
                        checked { ++index4; }
                    }
                    textWriter.WriteLine("{0}{0}</Rule>", (object)str);
                    checked { ++index3; }
                }
                textWriter.WriteLine("{0}</RuleTable>", (object)str);
                textWriter.WriteLine("{0}<CharSetTable Count={1}{2}{1}>", (object)str, (object)'"', (object)this.m_CharSet.Count);
                int num6 = checked(this.m_CharSet.Count - 1);
                int index5 = 0;
                while (index5 <= num6)
                {
                    CharacterSet characterSet = this.m_CharSet[index5];
                    textWriter.WriteLine("{0}{0}<CharSet Index={1}{2}{1} Count={1}{3}{1}>", (object)str, (object)'"', (object)index5, (object)characterSet.Count());
                    int num5 = checked(characterSet.Count() - 1);
                    int index4 = 0;
                    while (index4 <= num5)
                    {
                        textWriter.WriteLine("{0}{0}{0}<Char UnicodeIndex={1}{2}{1}/>", (object)str, (object)'"', (object)characterSet[index4]);
                        checked { ++index4; }
                    }
                    textWriter.WriteLine("{0}{0}</CharSet>", (object)str);
                    checked { ++index5; }
                }
                textWriter.WriteLine("{0}</CharSetTable>", (object)str);
                textWriter.WriteLine("{0}<DFATable Count={1}{2}{1} InitialState={1}{3}{1}>", (object)str, (object)'"', (object)this.m_DFA.Count, (object)this.DFA.InitialState);
                int num7 = checked(this.DFA.Count - 1);
                int index6 = 0;
                while (index6 <= num7)
                {
                    FAState faState = this.DFA[index6];
                    textWriter.WriteLine("{0}{0}<DFAState Index={1}{2}{1} EdgeCount={1}{3}{1} AcceptSymbol={1}{4}{1}>", (object)str, (object)'"', (object)index6, (object)faState.Edges().Count(), (object)faState.AcceptIndex());
                    int num5 = checked(faState.Edges().Count() - 1);
                    int index4 = 0;
                    while (index4 <= num5)
                    {
                        textWriter.WriteLine("{0}{0}{0}<DFAEdge CharSetIndex={1}{2}{1} Target={1}{3}{1}/>", (object)str, (object)'"', (object)faState.Edges()[index4].Characters.TableIndex, (object)faState.Edges()[index4].Target);
                        checked { ++index4; }
                    }
                    textWriter.WriteLine("{0}{0}</DFAState>", (object)str);
                    checked { ++index6; }
                }
                textWriter.WriteLine("{0}</DFATable>", (object)str);
                textWriter.WriteLine("{0}<LALRTable Count={1}{2}{1} InitialState={1}{3}{1}>", (object)str, (object)'"', (object)this.LALR.Count, (object)this.LALR.InitialState);
                int num8 = checked(this.LALR.Count - 1);
                int index7 = 0;
                while (index7 <= num8)
                {
                    LRState lrState = this.LALR[index7];
                    textWriter.WriteLine("{0}{0}<LALRState Index={1}{2}{1} ActionCount={1}{3}{1}>", (object)str, (object)'"', (object)index7, (object)lrState.Count);
                    int num5 = checked(lrState.Count - 1);
                    int num9 = 0;
                    while (num9 <= num5)
                    {
                        textWriter.WriteLine("{0}{0}{0}<LALRAction SymbolIndex={1}{2}{1} Action={1}{3}{1} Value={1}{4}{1}/>", (object)str, (object)'"', (object)lrState[checked((short)num9)].SymbolIndex(), (object)Conversion.Int((int)lrState[checked((short)num9)].Type()), (object)lrState[checked((short)num9)].Value());
                        checked { ++num9; }
                    }
                    textWriter.WriteLine("{0}{0}</LALRState>", (object)str);
                    checked { ++index7; }
                }
                textWriter.WriteLine("{0}</LALRTable>", (object)str);
                textWriter.WriteLine("</Tables>");
                textWriter.Close();
                flag = true;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                flag = false;
                Information.Err().Clear();
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        public bool SaveXML5(string FileName)
        {
            string str = Strings.Space(8);
            bool flag;
            try
            {
                TextWriter textWriter = (TextWriter)new StreamWriter(FileName, false);
                textWriter.WriteLine("<?GOLDParserTables version={0}5.0{0}?>", (object)'"');
                textWriter.WriteLine("<Tables>");
                textWriter.WriteLine("{0}<Properties>", (object)str);
                textWriter.WriteLine("{0}{0}<Property Index=\"0\" Name=\"Name\" Value=\"{1}\"/>", (object)str, (object)BuilderUtility.XMLText(this.m_Properties["Name"].Value));
                textWriter.WriteLine("{0}{0}<Property Index=\"1\" Name=\"Author\" Value=\"{1}\"/>", (object)str, (object)BuilderUtility.XMLText(this.m_Properties["Author"].Value));
                textWriter.WriteLine("{0}{0}<Property Index=\"2\" Name=\"Version\" Value=\"{1}\"/>", (object)str, (object)BuilderUtility.XMLText(this.m_Properties["Version"].Value));
                textWriter.WriteLine("{0}{0}<Property Index=\"3\" Name=\"About\" Value=\"{1}\"/>", (object)str, (object)BuilderUtility.XMLText(this.m_Properties["About"].Value));
                textWriter.WriteLine("{0}{0}<Property Index=\"4\" Name=\"Character Set\" Value=\"{1}\"/>", (object)str, (object)BuilderUtility.XMLText(this.m_Properties["Character Set"].Value));
                textWriter.WriteLine("{0}{0}<Property Index=\"5\" Name=\"Character Mapping\" Value=\"{1}\"/>", (object)str, (object)BuilderUtility.XMLText(this.m_Properties["Character Mapping"].Value));
                textWriter.WriteLine("{0}{0}<Property Index=\"6\" Name=\"Generated By\" Value=\"{1}\"/>", (object)str, (object)BuilderUtility.XMLText(this.m_Properties["Generated By"].Value));
                textWriter.WriteLine("{0}{0}<Property Index=\"7\" Name=\"Generated Date\" Value=\"{1}\"/>", (object)str, (object)this.m_Properties["Generated Date"].Value);
                textWriter.WriteLine("{0}</Properties>", (object)str);
                textWriter.WriteLine("{0}<m_Symbol Count=\"{1}\">", (object)str, (object)this.m_Symbol.Count());
                int num1 = checked(this.m_Symbol.Count() - 1);
                int index1 = 0;
                while (index1 <= num1)
                {
                    Symbol symbol = this.m_Symbol[index1];
                    textWriter.WriteLine("{0}{0}<Symbol Index=\"{1}\" Name=\"{2}\" Type=\"{3}\"/>", (object)str, (object)index1, (object)BuilderUtility.XMLText(symbol.Name), (object)Conversion.Int((int)symbol.Type));
                    checked { ++index1; }
                }
                textWriter.WriteLine("{0}</m_Symbol>", (object)str);
                textWriter.WriteLine("{0}<m_Group Count=\"{1}\">", (object)str, (object)this.m_Group.Count);
                int num2 = checked(this.m_Group.Count - 1);
                int index2 = 0;
                while (index2 <= num2)
                {
                    Group group = this.m_Group[index2];
                    textWriter.WriteLine("{0}{0}<Group Index=\"{1}\" Name=\"{2}\" ContainerIndex=\"{3}\" StartIndex=\"{4}\" EndIndex=\"{5}\" Advance=\"{6}\" Ending=\"{7}\" NestingCount=\"{8}\">", (object)str, (object)index2, (object)BuilderUtility.XMLText(group.Name), (object)group.Container.TableIndex, (object)group.Start.TableIndex, (object)group.End.TableIndex, (object)Conversion.Int((int)group.Advance), (object)Conversion.Int((int)group.Ending), (object)group.Nesting.Count);
                    int num3 = checked(group.Nesting.Count - 1);
                    int index3 = 0;
                    while (index3 <= num3)
                    {
                        textWriter.WriteLine("{0}{0}{0}<NestedGroup Index=\"{1}\"/>", (object)str, (object)group.Nesting[index3]);
                        checked { ++index3; }
                    }
                    textWriter.WriteLine("{0}{0}</Group>", (object)str);
                    checked { ++index2; }
                }
                textWriter.WriteLine("{0}</m_Group>", (object)str);
                textWriter.WriteLine("{0}<m_Production Count=\"{1}\">", (object)str, (object)this.m_Production.Count());
                int num4 = checked(this.m_Production.Count() - 1);
                int index4 = 0;
                while (index4 <= num4)
                {
                    Production production = this.m_Production[index4];
                    textWriter.WriteLine("{0}{0}<Production Index=\"{1}\" NonTerminalIndex=\"{2}\" SymbolCount=\"{3}\">", (object)str, (object)index4, (object)production.Head.TableIndex, (object)production.Handle().Count());
                    int num3 = checked(production.Handle().Count() - 1);
                    int index3 = 0;
                    while (index3 <= num3)
                    {
                        textWriter.WriteLine("{0}{0}{0}<ProductionSymbol SymbolIndex=\"{1}\"/>", (object)str, (object)production.Handle()[index3].TableIndex);
                        checked { ++index3; }
                    }
                    textWriter.WriteLine("{0}{0}</Production>", (object)str);
                    checked { ++index4; }
                }
                textWriter.WriteLine("{0}</m_Production>", (object)str);
                textWriter.WriteLine("{0}<m_CharSet Count=\"{1}\">", (object)str, (object)this.m_CharSet.Count);
                int num5 = checked(this.m_CharSet.Count - 1);
                int index5 = 0;
                while (index5 <= num5)
                {
                    CharacterSet characterSet = this.m_CharSet[index5];
                    textWriter.WriteLine("{0}{0}<CharSet Index=\"{1}\" Count=\"{2}\">", (object)str, (object)index5, (object)characterSet.Count());
                    int num3 = checked(characterSet.Count() - 1);
                    int index3 = 0;
                    while (index3 <= num3)
                    {
                        textWriter.WriteLine("{0}{0}{0}<Char UnicodeIndex=\"{1}\"/>", (object)str, (object)characterSet[index3]);
                        checked { ++index3; }
                    }
                    textWriter.WriteLine("{0}{0}</CharSet>", (object)str);
                    checked { ++index5; }
                }
                textWriter.WriteLine("{0}</m_CharSet>", (object)str);
                textWriter.WriteLine("{0}<DFATable Count=\"{1}\" InitialState=\"{2}\">", (object)str, (object)this.m_DFA.Count, (object)this.m_DFA.InitialState);
                int num6 = checked(this.DFA.Count - 1);
                int index6 = 0;
                while (index6 <= num6)
                {
                    FAState faState = this.DFA[index6];
                    textWriter.WriteLine("{0}{0}<DFAState Index=\"{1}\" EdgeCount=\"{2}\" AcceptSymbol=\"{3}\">", (object)str, (object)index6, (object)faState.Edges().Count(), (object)faState.AcceptIndex());
                    int num3 = checked(faState.Edges().Count() - 1);
                    int index3 = 0;
                    while (index3 <= num3)
                    {
                        textWriter.WriteLine("{0}{0}{0}<DFAEdge CharSetIndex=\"{1}\" Target=\"{2}\"/>", (object)str, (object)faState.Edges()[index3].Characters.TableIndex, (object)faState.Edges()[index3].Target);
                        checked { ++index3; }
                    }
                    textWriter.WriteLine("{0}{0}</DFAState>", (object)str);
                    checked { ++index6; }
                }
                textWriter.WriteLine("{0}</DFATable>", (object)str);
                textWriter.WriteLine("{0}<LALRTable Count=\"{1}\" InitialState=\"{2}\">", (object)str, (object)this.m_LALR.Count, (object)this.m_LALR.InitialState);
                int num7 = checked(this.m_LALR.Count - 1);
                int index7 = 0;
                while (index7 <= num7)
                {
                    LRState lrState = this.m_LALR[index7];
                    textWriter.WriteLine("{0}{0}<LALRState Index=\"{1}\" ActionCount=\"{2}\">", (object)str, (object)index7, (object)lrState.Count);
                    int num3 = checked(lrState.Count - 1);
                    int num8 = 0;
                    while (num8 <= num3)
                    {
                        textWriter.WriteLine("{0}{0}{0}<LALRAction SymbolIndex=\"{1}\" Action=\"{2}\" Value=\"{3}\"/>", (object)str, (object)lrState[checked((short)num8)].SymbolIndex(), (object)Conversion.Int((int)lrState[checked((short)num8)].Type()), (object)lrState[checked((short)num8)].Value());
                        checked { ++num8; }
                    }
                    textWriter.WriteLine("{0}{0}</LALRState>", (object)str);
                    checked { ++index7; }
                }
                textWriter.WriteLine("{0}</LALRTable>", (object)str);
                textWriter.WriteLine("</Tables>");
                textWriter.Close();
                flag = true;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                flag = false;
                Information.Err().Clear();
                ProjectData.ClearProjectError();
            }
            return flag;
        }


        protected enum EGTRecord : byte
        {
            CharSetLiteral = 67, // 0x43
            DFAState = 68, // 0x44
            InitialStates = 73, // 0x49
            LRState = 76, // 0x4C
            ParamRecord = 80, // 0x50
            Production = 82, // 0x52
            Symbol = 83, // 0x53
            Counts_1 = 84, // 0x54
            CharRanges = 99, // 0x63
            Group = 103, // 0x67
            Property = 112, // 0x70
            TableCounts = 116, // 0x74
        }


        protected enum EGTProperty
        {
            Name,
            Version,
            Author,
            About,
            CharacterSet,
            CharacterMapping,
            GeneratedBy,
            GeneratedDate,
        }
    }
}