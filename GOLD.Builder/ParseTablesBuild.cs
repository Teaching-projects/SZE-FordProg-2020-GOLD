using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;

namespace GOLD.Builder
{

    internal class ParseTablesBuild : ParseTables
    {
        public ParseTablesBuild()
        {
            this.m_Properties = new VariableList();
            this.m_Symbol = (SymbolList)new SymbolBuildList();
            this.m_CharSet = (CharacterSetList)new CharacterSetBuildList();
            this.m_Group = (GroupList)new GroupBuildList();
            this.m_DFA = (FAStateList)new FAStateBuildList();
            this.m_Production = (ProductionList)new ProductionBuildList();
            this.m_LALR = (LRStateList)new LRStateBuildList();
        }

        public void ComputeCGTMetadata()
        {
            int num1 = checked(this.m_Symbol.Count() - 1);
            int index1 = 0;
            while (index1 <= num1)
            {
                this.Symbol[index1].UsesDFA = false;
                checked { ++index1; }
            }
            int num2 = checked(this.DFA.Count - 1);
            int index2 = 0;
            while (index2 <= num2)
            {
                if (this.DFA[index2].Accept != null)
                    this.Symbol[(int)this.DFA[index2].Accept.TableIndex].UsesDFA = true;
                checked { ++index2; }
            }
        }

        public new VariableList Properties
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

        public SymbolBuildList Symbol
        {
            get
            {
                return (SymbolBuildList)this.m_Symbol;
            }
            set
            {
                this.m_Symbol = (SymbolList)value;
            }
        }

        public CharacterSetBuildList CharSet
        {
            get
            {
                return (CharacterSetBuildList)this.m_CharSet;
            }
            set
            {
                this.m_CharSet = (CharacterSetList)value;
            }
        }

        public ProductionBuildList Production
        {
            get
            {
                return (ProductionBuildList)this.m_Production;
            }
            set
            {
                this.m_Production = (ProductionList)value;
            }
        }

        public FAStateBuildList DFA
        {
            get
            {
                return (FAStateBuildList)this.m_DFA;
            }
            set
            {
                this.m_DFA = (FAStateList)value;
            }
        }

        public LRStateBuildList LALR
        {
            get
            {
                return (LRStateBuildList)this.m_LALR;
            }
            set
            {
                this.m_LALR = (LRStateList)value;
            }
        }

        public GroupBuildList Group
        {
            get
            {
                return (GroupBuildList)this.m_Group;
            }
            set
            {
                this.m_Group = (GroupList)value;
            }
        }

        public new bool Load(ref string Path)
        {
            SimpleDB.Reader reader = new SimpleDB.Reader();
            this.Clear();
            reader.Open(Path);
            string Left = reader.Header();
            bool flag = Operators.CompareString(Left, "GOLD Parser Tables/v1.0", true) != 0 ? Operators.CompareString(Left, "GOLD Parser Tables/v5.0", true) == 0 && this.LoadVer5(reader) : this.LoadVer1(reader);
            this.ComputeCGTMetadata();
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
                            if (flag2)
                                this.m_DFA[index1] = (FAState)new FAStateBuild((SymbolBuild)this.m_Symbol[index2]);
                            else
                                this.m_DFA[index1] = (FAState)new FAStateBuild();
                            while (!EGT.RecordComplete())
                            {
                                int index3 = EGT.RetrieveInt16();
                                int Target = EGT.RetrieveInt16();
                                EGT.RetrieveEntry();
                                this.m_DFA[index1].Edges().Add((FAEdge)new FAEdgeBuild((CharacterSetBuild)this.m_CharSet[index3], Target));
                            }
                            break;
                        case ParseTables.EGTRecord.InitialStates:
                            this.m_DFA.InitialState = checked((short)EGT.RetrieveInt16());
                            this.m_LALR.InitialState = checked((short)EGT.RetrieveInt16());
                            break;
                        case ParseTables.EGTRecord.LRState:
                            int index4 = EGT.RetrieveInt16();
                            EGT.RetrieveEntry();
                            this.m_LALR[index4] = (LRState)new LRStateBuild();
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
                            this.m_Production[index5] = (Production)new ProductionBuild((SymbolBuild)this.m_Symbol[index6], checked((short)index5));
                            while (!EGT.RecordComplete())
                            {
                                int index3 = EGT.RetrieveInt16();
                                this.m_Production[index5].Handle().Add(this.m_Symbol[index3]);
                            }
                            break;
                        case ParseTables.EGTRecord.Symbol:
                            int TableIndex = EGT.RetrieveInt16();
                            string Name = EGT.RetrieveString();
                            SymbolType Type = (SymbolType)EGT.RetrieveInt16();
                            this.m_Symbol[TableIndex] = (Symbol)new SymbolBuild(Name, Type, TableIndex);
                            break;
                        case ParseTables.EGTRecord.CharRanges:
                            CharacterSetBuild characterSetBuild = new CharacterSetBuild();
                            int index7 = EGT.RetrieveInt16();
                            EGT.RetrieveInt16();
                            EGT.RetrieveInt16();
                            EGT.RetrieveEntry();
                            while (!EGT.RecordComplete())
                                characterSetBuild.AddRange(EGT.RetrieveInt16(), EGT.RetrieveInt16());
                            characterSetBuild.TableIndex = index7;
                            this.m_CharSet[index7] = (CharacterSet)characterSetBuild;
                            break;
                        case ParseTables.EGTRecord.Group:
                            GroupBuild groupBuild1 = new GroupBuild();
                            GroupBuild groupBuild2 = groupBuild1;
                            int index8 = EGT.RetrieveInt16();
                            groupBuild2.Name = EGT.RetrieveString();
                            groupBuild2.Container = this.m_Symbol[EGT.RetrieveInt16()];
                            groupBuild2.Start = this.m_Symbol[EGT.RetrieveInt16()];
                            groupBuild2.End = this.m_Symbol[EGT.RetrieveInt16()];
                            groupBuild2.Advance = (AdvanceMode)EGT.RetrieveInt16();
                            groupBuild2.Ending = (EndingMode)EGT.RetrieveInt16();
                            EGT.RetrieveEntry();
                            int num3 = EGT.RetrieveInt16();
                            int num4 = 1;
                            while (num4 <= num3)
                            {
                                groupBuild2.Nesting.Add(EGT.RetrieveInt16());
                                checked { ++num4; }
                            }
                            groupBuild1.Container.Group = (Group)groupBuild1;
                            groupBuild1.Start.Group = (Group)groupBuild1;
                            groupBuild1.End.Group = (Group)groupBuild1;
                            this.m_Group[index8] = (Group)groupBuild1;
                            break;
                        case ParseTables.EGTRecord.Property:
                            EGT.RetrieveInt16();
                            this.m_Properties.Add(EGT.RetrieveString(), (object)EGT.RetrieveString());
                            break;
                        case ParseTables.EGTRecord.TableCounts:
                            this.m_Symbol = (SymbolList)new SymbolBuildList(EGT.RetrieveInt16());
                            this.m_CharSet = (CharacterSetList)new CharacterSetBuildList(EGT.RetrieveInt16());
                            this.m_Production = (ProductionList)new ProductionBuildList(EGT.RetrieveInt16());
                            this.m_DFA = (FAStateList)new FAStateBuildList(EGT.RetrieveInt16());
                            this.m_LALR = (LRStateList)new LRStateBuildList(EGT.RetrieveInt16());
                            this.m_Group = (GroupList)new GroupBuildList(EGT.RetrieveInt16());
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
                throw new Exception("LoadTables: " + ex.Message);
            }
            return flag1;
        }

        private bool LoadVer1(SimpleDB.Reader CGT)
        {
            bool flag1 = true;
            int index1 = 0;
            while (!(CGT.EndOfFile() | !flag1))
            {
                CGT.GetNextRecord();
                switch (CGT.RetrieveByte())
                {
                    case 67:
                        int index2 = CGT.RetrieveInt16();
                        CharacterSetBuild characterSetBuild = new CharacterSetBuild(CGT.RetrieveString());
                        characterSetBuild.TableIndex = index2;
                        this.m_CharSet[index2] = (CharacterSet)characterSetBuild;
                        break;
                    case 68:
                        FAState faState = new FAState();
                        int index3 = CGT.RetrieveInt16();
                        bool flag2 = CGT.RetrieveBoolean();
                        int index4 = CGT.RetrieveInt16();
                        CGT.RetrieveEntry();
                        if (flag2)
                            this.m_DFA[index3] = (FAState)new FAStateBuild((SymbolBuild)this.m_Symbol[index4]);
                        else
                            this.m_DFA[index3] = (FAState)new FAStateBuild();
                        while (!CGT.RecordComplete())
                        {
                            int index5 = CGT.RetrieveInt16();
                            int Target = CGT.RetrieveInt16();
                            CGT.RetrieveEntry();
                            this.m_DFA[index3].AddEdge((FAEdge)new FAEdgeBuild((CharacterSetBuild)this.m_CharSet[index5], Target));
                        }
                        break;
                    case 73:
                        this.m_DFA.InitialState = checked((short)CGT.RetrieveInt16());
                        this.m_LALR.InitialState = checked((short)CGT.RetrieveInt16());
                        break;
                    case 76:
                        int index6 = CGT.RetrieveInt16();
                        CGT.RetrieveEntry();
                        this.m_LALR[index6] = (LRState)new LRStateBuild();
                        while (!CGT.RecordComplete())
                        {
                            int index5 = CGT.RetrieveInt16();
                            int num1 = CGT.RetrieveInt16();
                            int num2 = CGT.RetrieveInt16();
                            CGT.RetrieveEntry();
                            this.m_LALR[index6].Add(new LRAction(this.m_Symbol[index5], (LRActionType)num1, checked((short)num2)));
                        }
                        break;
                    case 80:
                        this.m_Properties["Name"].Value = CGT.RetrieveString();
                        this.m_Properties["Version"].Value = CGT.RetrieveString();
                        this.m_Properties["Author"].Value = CGT.RetrieveString();
                        this.m_Properties["About"].Value = CGT.RetrieveString();
                        this.m_Properties["Case Sensitive"].Value = Conversions.ToString(CGT.RetrieveBoolean());
                        index1 = CGT.RetrieveInt16();
                        break;
                    case 82:
                        int index7 = CGT.RetrieveInt16();
                        int index8 = CGT.RetrieveInt16();
                        CGT.RetrieveEntry();
                        this.m_Production[index7] = (Production)new ProductionBuild((SymbolBuild)this.m_Symbol[index8], checked((short)index7));
                        while (!CGT.RecordComplete())
                        {
                            int index5 = CGT.RetrieveInt16();
                            this.m_Production[index7].Handle().Add(this.m_Symbol[index5]);
                        }
                        break;
                    case 83:
                        int TableIndex = CGT.RetrieveInt16();
                        string Name = CGT.RetrieveString();
                        SymbolType Type = (SymbolType)CGT.RetrieveInt16();
                        this.m_Symbol[TableIndex] = (Symbol)new SymbolBuild(Name, Type, TableIndex);
                        break;
                    case 84:
                        this.m_Symbol = (SymbolList)new SymbolBuildList(CGT.RetrieveInt16());
                        this.m_CharSet = (CharacterSetList)new CharacterSetBuildList(CGT.RetrieveInt16());
                        this.m_Production = (ProductionList)new ProductionBuildList(CGT.RetrieveInt16());
                        this.m_DFA = (FAStateList)new FAStateBuildList(CGT.RetrieveInt16());
                        this.m_LALR = (LRStateList)new LRStateBuildList(CGT.RetrieveInt16());
                        break;
                    default:
                        flag1 = false;
                        break;
                }
            }
            this.StartSymbol = this.m_Symbol[index1];
            SymbolBuild symbolBuild1 = (SymbolBuild)null;
            SymbolBuild symbolBuild2 = (SymbolBuild)null;
            SymbolBuild symbolBuild3 = (SymbolBuild)null;
            int num = checked(this.m_Symbol.Count() - 1);
            int index9 = 0;
            while (index9 <= num)
            {
                SymbolBuild symbolBuild4 = (SymbolBuild)this.m_Symbol[index9];
                switch (symbolBuild4.Type)
                {
                    case SymbolType.Noise:
                        if (symbolBuild3 == null)
                            symbolBuild3 = symbolBuild4;
                        break;
                    case SymbolType.GroupStart:
                        symbolBuild1 = symbolBuild4;
                        break;
                    case SymbolType.GroupEnd:
                        symbolBuild2 = symbolBuild4;
                        break;
                }
                checked { ++index9; }
            }
            if (symbolBuild1 != null)
            {
                GroupBuild groupBuild = new GroupBuild();
                this.m_Group.Add((Group)groupBuild);
                groupBuild.TableIndex = (short)0;
                groupBuild.Name = "Comment Block";
                groupBuild.Container = (Symbol)symbolBuild3;
                groupBuild.Nesting.Add((int)groupBuild.TableIndex);
                groupBuild.Advance = AdvanceMode.Token;
                groupBuild.Ending = EndingMode.Closed;
                groupBuild.Start = (Symbol)symbolBuild1;
                groupBuild.End = (Symbol)symbolBuild2;
                groupBuild.Start.Group = (Group)groupBuild;
                groupBuild.End.Group = (Group)groupBuild;
                groupBuild.NestingNames = "All";
            }
            return flag1;
        }
    }
}