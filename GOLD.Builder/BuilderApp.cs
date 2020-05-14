using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace GOLD.Builder
{
    internal sealed class BuilderApp
    {
        public static DefinedCharacterSetList UserDefinedSets = new DefinedCharacterSetList();
        internal static DefinedCharacterSetList PredefinedSets = new DefinedCharacterSetList();
        public static ParseTablesBuild BuildTables = new ParseTablesBuild();
        public static SysLog Log = new SysLog();
        public static SysLog BOOTLOG = new SysLog();
        public static SysLog SaveCGTWarnings = new SysLog();
        public const string APP_VERSION_FULL = "5.2.0.";
        public const string APP_VERSION_TITLE = "5.2";
        public const bool IN_PROGRESS = false;
        public static bool CancelTableBuild;
        public static ProgramMode Mode;
        public const short RANK_FA_UNRANKED = -1;
        public const short RANK_FA_FIXED_LENGTH = 0;
        public const short RANK_FA_VARIABLE_LENGTH = 10001;
        public const short RANK_LR_UNRANKED = -1;
        public const short RANK_LR_SHIFT = 10001;
        public const short RANK_LR_REDUCE = 10002;

        public static CharMappingMode GetParamCharMapping()
        {
            string Left = Microsoft.VisualBasic.Strings.UCase(BuildTables.Properties["Character Mapping"].Value);
            if (Operators.CompareString(Left, "WINDOWS-1252", true) == 0 || Operators.CompareString(Left, "ANSI", true) == 0)
                return CharMappingMode.Windows1252;
            return Operators.CompareString(Left, "NONE", true) == 0 ? CharMappingMode.None : CharMappingMode.Invalid;
        }

        public static void CreateLRPriorStateLists()
        {
            int num1 = checked(BuildTables.LALR.Count - 1);
            int index1 = 0;
            while (index1 <= num1)
            {
                BuildTables.LALR[index1].PriorStates.Clear();
                checked { ++index1; }
            }
            int num2 = checked(BuildTables.LALR.Count - 1);
            int index2 = 0;
            while (index2 <= num2)
            {
                LRStateBuild lrStateBuild = BuildTables.LALR[index2];
                int num3 = checked(lrStateBuild.Count - 1);
                int num4 = 0;
                while (num4 <= num3)
                {
                    LRAction lrAction = lrStateBuild[checked((short)num4)];
                    switch (lrAction.Type())
                    {
                        case LRActionType.Shift:
                        case LRActionType.Goto:
                            BuildTables.LALR[(int)lrAction.Value()].PriorStates.Add(index2);
                            break;
                    }
                    checked { ++num4; }
                }
                checked { ++index2; }
            }
        }

        public static void CreateDFAPriorStateLists()
        {
            int num1 = checked(BuildTables.DFA.Count - 1);
            int index1 = 0;
            while (index1 <= num1)
            {
                BuildTables.DFA[index1].PriorStates.Clear();
                checked { ++index1; }
            }
            int num2 = checked(BuildTables.DFA.Count - 1);
            int index2 = 0;
            while (index2 <= num2)
            {
                FAStateBuild faStateBuild = BuildTables.DFA[index2];
                int num3 = checked(faStateBuild.Edges().Count() - 1);
                int index3 = 0;
                while (index3 <= num3)
                {
                    FAEdge edge = (FAEdge)faStateBuild.Edges()[index3];
                    BuildTables.DFA[edge.Target].PriorStates.Add(index2);
                    checked { ++index3; }
                }
                checked { ++index2; }
            }
        }

        public static void LogActionTotals()
        {
            short[] numArray1 = new short[checked(BuildTables.Symbol.Count() - 1 + 1)];
            int num1 = checked(BuildTables.LALR.Count - 1);
            int index1 = 0;
            int num2=0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            while (index1 <= num1)
            {
                LRStateBuild lrStateBuild = BuildTables.LALR[index1];
                short num6 = checked((short)(lrStateBuild.Count - 1));
                short index2 = 0;
                while ((int)index2 <= (int)num6)
                {
                    LRAction lrAction = lrStateBuild[index2];
                    switch (lrAction.Type())
                    {
                        case LRActionType.Shift:
                            checked { ++num2; }
                            break;
                        case LRActionType.Reduce:
                            checked { ++num3; }
                            short[] numArray2 = numArray1;
                            short[] numArray3 = numArray2;
                            int index3 = (int)lrAction.SymbolIndex();
                            int index4 = index3;
                            int num7 = (int)checked((short)((int)numArray2[index3] + 1));
                            numArray3[index4] = (short)num7;
                            break;
                        case LRActionType.Goto:
                            checked { ++num4; }
                            break;
                        case LRActionType.Accept:
                            checked { ++num5; }
                            break;
                    }
                    checked { ++index2; }
                }
                checked { ++index1; }
            }
            string Title = "Total actions: " + Conversions.ToString(num2) + " Shifts, " + Conversions.ToString(num3) + " Reduces, " + Conversions.ToString(num4) + " Gotos, " + Conversions.ToString(num5) + " Accepts.";
            Log.Add(SysLogSection.LALR, SysLogAlert.Detail, Title);
        }

        public static string AppNameDesc()
        {
            return "Grammar Oriented Language Developer";
        }

        public static string AppNameVersionTitle()
        {
            return "GOLD Parser Builder 5.2";
        }

        public static string AppNameVersionFull()
        {
            return "GOLD Parser Builder 5.2.0.";
        }

        public static string AppName()
        {
            return "GOLD Parser Builder";
        }

        private static void CheckIfUsed(ref bool[] Checked, int NonTerminalIndex)
        {
            short num1 = checked((short)(BuildTables.Production.Count() - 1));
            short num2 = 0;
            while ((int)num2 <= (int)num1)
            {
                ProductionBuild productionBuild = BuildTables.Production[(int)num2];
                if ((int)productionBuild.Head.TableIndex == NonTerminalIndex)
                {
                    Checked[NonTerminalIndex] = true;
                    short num3 = checked((short)(productionBuild.Handle().Count() - 1));
                    short num4 = 0;
                    while ((int)num4 <= (int)num3)
                    {
                        SymbolBuild symbolBuild = productionBuild.Handle()[(int)num4];
                        switch (symbolBuild.Type)
                        {
                            case SymbolType.Nonterminal:
                                if (!Checked[(int)symbolBuild.TableIndex])
                                {
                                    CheckIfUsed(ref Checked, (int)symbolBuild.TableIndex);
                                    break;
                                }
                                break;
                            case SymbolType.Content:
                                Checked[(int)symbolBuild.TableIndex] = true;
                                break;
                        }
                        checked { ++num4; }
                    }
                }
                checked { ++num2; }
            }
        }

        private static void CheckIllegalSymbols()
        {
            short num1 = checked((short)(BuildTables.Production.Count() - 1));
            short num2 = 0;
            while ((int)num2 <= (int)num1)
            {
                ProductionBuild productionBuild = BuildTables.Production[(int)num2];
                short num3 = checked((short)(productionBuild.Handle().Count() - 1));
                short num4 = 0;
                while ((int)num4 <= (int)num3)
                {
                    SymbolBuild symbolBuild = productionBuild.Handle()[(int)num4];
                    switch (symbolBuild.Type)
                    {
                        case SymbolType.Nonterminal:
                        case SymbolType.Content:
                        case SymbolType.GroupEnd:
                            checked { ++num4; }
                            continue;
                        case SymbolType.Noise:
                            if (symbolBuild.Reclassified)
                            {
                                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Noise terminal used in a production.", "The symbol '" + symbolBuild.Name + "' is was changed by 'Noise' by the system. This is done for the terminal's named 'Whitespace' and 'Comment'. It was used in the production " + productionBuild.Text(), "");
                                goto case SymbolType.Nonterminal;
                            }
                            else
                            {
                                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Noise terminal used in a production.", "The symbol '" + symbolBuild.Name + "' is declared as Noise. This means it is ignored by the parser. It was used in the production " + productionBuild.Text(), "");
                                goto case SymbolType.Nonterminal;
                            }
                        case SymbolType.GroupStart:
                            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Cannot use a group start symbol in a production.", "The symbol '" + symbolBuild.Name + "' is the start of a lexical group. The lexer will use this symbol, and the matching end, to create a single container token. It was used in the production " + productionBuild.Text(), "");
                            goto case SymbolType.Nonterminal;
                        default:
                            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Illegal symbol", "The symbol '" + symbolBuild.Name + "' is not allowed  in the production " + productionBuild.Text(), "");
                            goto case SymbolType.Nonterminal;
                    }
                }
                checked { ++num2; }
            }
        }

        private static void CheckRuleRecursion()
        {
            int num = checked(BuildTables.Production.Count() - 1);
            int index = 0;
            while (index <= num)
            {
                ProductionBuild productionBuild = BuildTables.Production[index];
                if (productionBuild.Handle().Count() == 1)
                {
                    if ((int)productionBuild.Head.TableIndex == (int)productionBuild.Handle()[0].TableIndex)
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "The rule " + productionBuild.Name() + " is defined as itself", "This rule is defined using the form <A> ::= <A>. This will eventually lead to a shift-reduce error.", "");
                }
                else if (productionBuild.Handle().Count() >= 2)
                {
                    short tableIndex1 = productionBuild.Head.TableIndex;
                    short tableIndex2 = productionBuild.Handle()[0].TableIndex;
                    short tableIndex3 = productionBuild.Handle()[checked(productionBuild.Handle().Count() - 1)].TableIndex;
                    if ((int)tableIndex1 == (int)tableIndex2 & (int)tableIndex1 == (int)tableIndex3)
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "The rule " + productionBuild.Name() + " is both left and right recursive.", "This rule is defined using the form <A> ::= <A> .. <A>. This will eventually lead to a shift-reduce error.", "");
                }
                checked { ++index; }
            }
        }

        private static void CheckDuplicateGroupStart()
        {
            GroupBuildList[] groupBuildListArray = new GroupBuildList[checked(BuildTables.Symbol.Count() + 1)];
            short num1 = checked((short)(BuildTables.Symbol.Count() - 1));
            short num2 = 0;
            while ((int)num2 <= (int)num1)
            {
                groupBuildListArray[(int)num2] = new GroupBuildList();
                checked { ++num2; }
            }
            short num3 = checked((short)(BuildTables.Group.Count - 1));
            short num4 = 0;
            while ((int)num4 <= (int)num3)
            {
                GroupBuild groupBuild = BuildTables.Group[(int)num4];
                groupBuildListArray[(int)groupBuild.Start.TableIndex].Add(groupBuild);
                checked { ++num4; }
            }
            short num5 = checked((short)(BuildTables.Symbol.Count() - 1));
            short num6 = 0;
            while ((int)num6 <= (int)num5)
            {
                if (groupBuildListArray[(int)num6].Count >= 2)
                {
                    SymbolBuild symbolBuild = BuildTables.Symbol[(int)num6];
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Symbol used to start multiple groups", "The symbol '" + symbolBuild.Name + "' is used in the following groups: " + groupBuildListArray[(int)num6].ToString(), "");
                }
                checked { ++num6; }
            }
        }

        private static void CheckUnusedSymbols()
        {
            bool[] Checked = new bool[checked(BuildTables.Symbol.Count() - 1 + 1)];
            short num1 = checked((short)(BuildTables.Symbol.Count() - 1));
            short num2 = 0;
            while ((int)num2 <= (int)num1)
            {
                Checked[(int)num2] = false;
                checked { ++num2; }
            }
            CheckIfUsed(ref Checked, (int)BuildTables.StartSymbol.TableIndex);
            short num3 = checked((short)(BuildTables.Symbol.Count() - 1));
            short num4 = 0;
            while ((int)num4 <= (int)num3)
            {
                Symbol symbol = (Symbol)BuildTables.Symbol[(int)num4];
                if (!Checked[(int)num4] & symbol.Type == SymbolType.Nonterminal)
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Unreachable rule: <" + symbol.Name + ">", "The rule <" + symbol.Name + "> cannot be reached from the \"Start Symbol\".", "");
                checked { ++num4; }
            }
            if (LoggedCriticalError())
                return;
            short num5 = checked((short)(BuildTables.Symbol.Count() - 1));
            short num6 = 0;
            while ((int)num6 <= (int)num5)
            {
                Symbol symbol = (Symbol)BuildTables.Symbol[(int)num6];
                if (!Checked[(int)num6] & symbol.Type == SymbolType.Content)
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Unused terminal: " + symbol.Name, "The terminal " + symbol.Name + " is defined but not used.", "");
                checked { ++num6; }
            }
        }

        public static void PopulateSaveCGTWarningTable()
        {
            SaveCGTWarnings.Clear();
            try
            {
                int num1 = checked(BuildTables.Group.Count - 1);
                int index1 = 0;
                while (index1 <= num1)
                {
                    GroupBuild groupBuild = BuildTables.Group[index1];
                    if (Operators.CompareString(groupBuild.Name.ToUpper(), "COMMENT BLOCK", true) != 0 & Operators.CompareString(groupBuild.Name.ToUpper(), "COMMENT LINE", true) != 0)
                        SaveCGTWarnings.Add("The group '" + groupBuild.Name + "' will not be saved", "Version 1.0 only supports one group: Comment. The start/end symbols will be saved as regular terminals.");
                    checked { ++index1; }
                }
                int index2 = BuildTables.Group.ItemIndex("COMMENT BLOCK");
                if (index2 != -1)
                {
                    GroupBuild groupBuild = BuildTables.Group[index2];
                    if (groupBuild.Nesting.Count != 2)
                        SaveCGTWarnings.Add("Comment Block attribute will change", "Version 1.0 only supports 'all' nested block comments. When the file is saved, it will use this attribute.");
                    if (groupBuild.Advance != AdvanceMode.Token)
                        SaveCGTWarnings.Add("Comment Block attribute will change", "Version 1.0 only supports 'token' advancing in block comments. When the file is saved, it will use this attribute.");
                    if (groupBuild.Ending != EndingMode.Closed)
                        SaveCGTWarnings.Add("Comment Block attribute will change", "Version 1.0 only supports 'closed' block comments. When the file is saved, it will use this attribute.");
                }
                if (BuildTables.CharSet.Count > 0)
                {
                    long num2 = 0;
                    long num3 = 0;
                    int num4 = checked(BuildTables.CharSet.Count - 1);
                    int index3 = 0;
                    while (index3 <= num4)
                    {
                        checked { num2 += (long)(5 + BuildTables.CharSet[index3].Count() * 2 + 2); }
                        checked { num3 += (long)(12 + 6 * BuildTables.CharSet[index3].RangeList().Count); }
                        checked { ++index3; }
                    }
                    if ((double)num2 / (double)num3 >= 2.0)
                        SaveCGTWarnings.Add(SysLogSection.Grammar, SysLogAlert.Info, "Version 1.0 will require " + Conversions.ToString(num2) + " bytes to store character set data. The new format will require " + Conversions.ToString(num3) + " bytes.");
                }
                bool flag = false;
                int index4 = 0;
                while (index4 < BuildTables.CharSet.Count & !flag)
                {
                    if (BuildTables.CharSet[index4].Contains(0))
                        flag = true;
                    checked { ++index4; }
                }
                if (!flag)
                    return;
                SaveCGTWarnings.Add("Character &00 cannot be stored", "Due to how character sets are stored in Version 1.0, null characters (&00) will not stored.");
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Information.Err().Clear();
                ProjectData.ClearProjectError();
            }
        }

        public static bool LoggedCriticalError()
        {
            return Log.AlertCount(SysLogAlert.Critical) != 0;
        }

        public static bool LoggedInternalError()
        {
            return Log.SectionCount(SysLogSection.Internal) != 0;
        }

        public static bool LoggedWarning()
        {
            return Log.AlertCount(SysLogAlert.Warning) != 0;
        }

        public static string About()
        {
            return "" + "Admittedly, this is not a particularly clever acronym, but it does (in part) represent the history of the greater Sacramento Area." + "\r\n\r\n" + "This application is shareware. You are completely FREE to use this application for your projects, but I hope that you will SHARE your feedback and suggestions with me!" + "\r\n\r\n" + "Only with your input will this become a respected and useful programmer's tool. Happy programming!";
        }

        public static string AppVersionFull()
        {
            return "5.2.0.";
        }

        public static void Setup()
        {
            LoadPredefinedSets();
            GrammarParse.Setup();
            UnicodeTable.Setup();
        }

        public static void ComputeDFA()
        {
            if (!LoggedCriticalError())
            {
                CancelTableBuild = false;
                DateTime now1 = DateAndTime.Now;
                BuildDFA.Build();
                DateTime now2 = DateAndTime.Now;
                Log.Add(SysLogSection.DFA, SysLogAlert.Detail, "DFA computation took: " + BuilderUtility.TimeDiffString(now1, now2));
            }
            CreateDFAPriorStateLists();
        }

        public static bool LoadTables(string Path)
        {
            bool flag = BuildTables.Load(ref Path);
            if (flag)
            {
                CreateDFAPriorStateLists();
                CreateLRPriorStateLists();
            }
            return flag;
        }

        public static void ComputeComplete()
        {
            if (!BuildTables.Properties.Contains("Generated By"))
                BuildTables.Properties.Add("Generated By", (object)AppNameVersionFull());
            if (!BuildTables.Properties.Contains("Generated Date"))
                BuildTables.Properties.Add("Generated Date", (object)Microsoft.VisualBasic.Strings.Format((object)DateAndTime.Now, "yyyy-MM-dd HH:mm"));
            PopulateSaveCGTWarningTable();
        }

        public static short AddCharacterSet(ref CharacterSetBuild Chars)
        {
            short num1 = -1;
            short num2 = 0;
            while ((int)num2 < BuildTables.CharSet.Count & num1 == (short)-1)
            {
                if (BuildTables.CharSet[(int)num2].IsEqualSet((NumberSet)Chars))
                    num1 = num2;
                checked { ++num2; }
            }
            if (num1 == (short)-1)
            {
                num1 = checked((short)BuildTables.CharSet.Add(ref Chars));
                Chars.TableIndex = (int)num1;
            }
            return num1;
        }

        public static void ComputeLALR()
        {
            CancelTableBuild = false;
            DateTime now1 = DateAndTime.Now;
            BuildLR.Build();
            DateTime now2 = DateAndTime.Now;
            Log.Add(SysLogSection.LALR, SysLogAlert.Detail, "LALR computation took: " + BuilderUtility.TimeDiffString(now1, now2));
            if (!LoggedCriticalError())
            {
                Log.Add(SysLogSection.LALR, SysLogAlert.Success, "LALR Table was succesfully created", "The table contains a total of " + Conversions.ToString(BuildTables.LALR.Count) + " states", "");
                LogActionTotals();
            }
            CreateLRPriorStateLists();
        }

        public static void EnterGrammar(ref TextReader MetaGrammar)
        {
            Restart();
            Mode = ProgramMode.Input;
            Notify.Started("Analyzing Grammar");
            Grammar.Build(MetaGrammar);
            Populate();
            AssignTableIndexes();
            LinkGroupSymbolsToGroup();
            AssignPriorities();
            string Left = Microsoft.VisualBasic.Strings.UCase(BuildTables.Properties["Case Sensitive"].Value);
            if (Operators.CompareString(Left, "TRUE", true) != 0 && Operators.CompareString(Left, "FALSE", true) != 0)
                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid Property Value", "The \"Case Sensitive\" parameter must be either True or False.", "");
            if (GetParamCharMapping() == CharMappingMode.Invalid)
                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid Property Value", "The \"Character Mapping\" parameter must be either Unicode, Windows-1252, or ANSI (same as Windows-1252).", "");
            int num1 = 0;
            int num2 = checked(BuildTables.Symbol.Count() - 1);
            int index = 0;
            while (index <= num2)
            {
                if (BuildTables.Symbol[index].Type == SymbolType.Content)
                    checked { ++num1; }
                checked { ++index; }
            }
            Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, "The grammar contains a total of " + Conversions.ToString(num1) + " formal terminals.");
            if (!LoggedCriticalError())
                DoSemanticAnalysis();
            if (!LoggedCriticalError())
                Log.Add(SysLogSection.Grammar, SysLogAlert.Success, "The grammar was successfully analyzed");
            Notify.Completed("Grammar Completed");
            Mode = ProgramMode.Idle;
        }

        private static void LoadPredefinedSets()
        {
            SimpleDB.Reader reader = new SimpleDB.Reader();
            try
            {
                reader.Open(FileUtility.AppPath() + "\\sets.dat");
                if (Operators.CompareString(reader.Header(), "GOLD Character Sets", true) != 0)
                {
                    Log.Add(SysLogSection.Internal, SysLogAlert.Critical, "The file 'sets.dat' is invalid");
                }
                else
                {
                    while (!reader.EndOfFile())
                    {
                        reader.GetNextRecord();
                        string str1 = reader.RetrieveString();
                        string str2 = reader.RetrieveString();
                        string str3 = reader.RetrieveString();
                        reader.RetrieveInt16();
                        DefinedCharacterSet definedCharacterSet = new DefinedCharacterSet();
                        definedCharacterSet.Name = str1;
                        definedCharacterSet.Type = str2;
                        definedCharacterSet.Comment = str3;
                        while (!reader.RecordComplete())
                            definedCharacterSet.AddRange(reader.RetrieveInt16(), reader.RetrieveInt16());
                        PredefinedSets.Add(ref definedCharacterSet);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                Information.Err().Clear();
                Log.Add(SysLogSection.Internal, SysLogAlert.Critical, exception.Message);
                ProjectData.ClearProjectError();
            }
        }

        private static void DoSemanticAnalysis()
        {
            if (!LoggedCriticalError())
                CheckIllegalSymbols();
            if (!LoggedCriticalError())
                CheckUnusedSymbols();
            if (!LoggedCriticalError())
                CheckRuleRecursion();
            if (LoggedCriticalError())
                return;
            CheckDuplicateGroupStart();
        }

        public static bool AddProduction(ref ProductionBuild NewRule)
        {
            bool flag = false;
            short num = 0;
            while (!flag & (int)num < BuildTables.Production.Count())
            {
                if (BuildTables.Production[(int)num].Equals((object)NewRule))
                    flag = true;
                checked { ++num; }
            }
            if (!flag)
                BuildTables.Production.Add(ref NewRule);
            return !flag;
        }

        public static bool RuleTypeExists(ref SymbolBuild NonTerminal)
        {
            bool flag = false;
            short num = 0;
            while (!flag & (int)num < BuildTables.Production.Count())
            {
                if (BuildTables.Production[(int)num].Head.IsEqualTo((Symbol)NonTerminal))
                    flag = true;
                checked { ++num; }
            }
            return flag;
        }

        public static bool IsPredefinedSet(string Name)
        {
            return PredefinedSets.ItemIndex(Name) != -1;
        }

        public static bool IsUserDefinedSet(string Name)
        {
            return UserDefinedSets.ItemIndex(Name) != -1;
        }

        public static CharacterSet GetCharacterSet(string Name)
        {
            CharacterSet characterSet = (CharacterSet)null;
            int index1 = UserDefinedSets.ItemIndex(Name);
            if (index1 != -1)
            {
                characterSet = (CharacterSet)UserDefinedSets[index1];
            }
            else
            {
                int index2 = PredefinedSets.ItemIndex(Name);
                if (index2 != -1)
                    characterSet = (CharacterSet)PredefinedSets[index2];
            }
            return characterSet;
        }

        public static void Restart()
        {
            Clear();
        }

        private static void Clear()
        {
            UserDefinedSets.Clear();
            Log.Clear();
            Grammar.Clear();
            SaveCGTWarnings.Clear();
            BuildTables.Clear();
        }

        public static bool SaveLog(string FilePath)
        {
            bool flag;
            try
            {
                TextWriter textWriter = (TextWriter)new StreamWriter(FilePath, false, Encoding.UTF8);
                int num = checked(Log.Count() - 1);
                int index = 0;
                while (index <= num)
                {
                    SysLogItem sysLogItem = Log[index];
                    string str1 = sysLogItem.SectionName().PadRight(15) + sysLogItem.AlertName().PadRight(10);
                    string str2 = (sysLogItem.Index != null ? str1 + sysLogItem.Index.ToString().PadRight(8) : str1 + Microsoft.VisualBasic.Strings.Space(8)) + sysLogItem.Title + " : " + sysLogItem.Description;
                    textWriter.WriteLine(str2);
                    checked { ++index; }
                }
                textWriter.Close();
                flag = true;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                flag = false;
                ProjectData.ClearProjectError();
            }
            return true;
        }

        public static bool CanUseSymbolTable()
        {
            return BuildTables.Symbol.Count() >= 1;
        }

        public static bool CanUseRuleTable()
        {
            return BuildTables.Production.Count() >= 1;
        }

        public static bool CanUseDFA()
        {
            return BuildTables.DFA.Count >= 1;
        }

        public static bool CanUseLALR()
        {
            return BuildTables.LALR.Count >= 1;
        }

        public static bool CanUserDefinedSetTable()
        {
            return UserDefinedSets.Count >= 1;
        }

        private static void PopulateStartSymbol()
        {
            string str = Microsoft.VisualBasic.Strings.Trim(BuildTables.Properties["Start Symbol"].Value);
            if (str.StartsWith("<") & str.EndsWith(">"))
                str = str.Substring(1, checked(str.Length - 2));
            if (Operators.CompareString(str, "", true) == 0)
            {
                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "No specified start symbol", "Please assign the \"Start Symbol\" parameter to the start symbol's name", "");
            }
            else
            {
                int index = (int)BuildTables.Symbol.NonterminalIndex(str);
                if (index == -1)
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid start symbol", "Please assign the \"Start Symbol\" parameter to the start symbol's name", "");
                else
                    BuildTables.StartSymbol = (Symbol)BuildTables.Symbol[index];
            }
        }

        private static void PopulateProperties()
        {
            try
            {
                foreach (Variable property in (ArrayList)Grammar.Properties)
                    BuildTables.Properties.Add(property);
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
            if (!BuildTables.Properties.Contains("Name"))
                BuildTables.Properties.Add("Name", (object)"(Untitled)");
            if (!BuildTables.Properties.Contains("Author"))
                BuildTables.Properties.Add("Author", (object)"(Unknown)");
            if (!BuildTables.Properties.Contains("Version"))
                BuildTables.Properties.Add("Version", (object)"(Not Specified)");
            if (!BuildTables.Properties.Contains("Auto Whitespace"))
                BuildTables.Properties.Add("Auto Whitespace", (object)"True");
            if (!BuildTables.Properties.Contains("Case Sensitive"))
                BuildTables.Properties.Add("Case Sensitive", (object)"False");
            if (!BuildTables.Properties.Contains("Character Mapping"))
                BuildTables.Properties.Add("Character Mapping", (object)"Windows-1252");
            else if (BuildTables.Properties["Character Mapping"].Value.ToUpper().Contains("UNICODE"))
                BuildTables.Properties["Character Mapping"].Value = "None";
            BuildTables.Properties.Add("Character Set", (object)"Unicode");
        }

        public static void Populate()
        {
            BuildTables.Clear();
            BuildTables.Symbol.AddUnique(new SymbolBuild("EOF", SymbolType.End, false, CreatorType.Generated));
            BuildTables.Symbol.AddUnique(new SymbolBuild("Error", SymbolType.Error, false, CreatorType.Generated));
            PopulateProperties();
            PopulateSets();
            PopulateDefinedSymbols();
            PopulateGroupsAndWhitespace();
            PopulateProductionHeads();
            PopulateHandleSymbols();
            PopulateProductions();
            PopulateStartSymbol();
            ApplyGroupAttributes();
            ApplySymbolAttributes();
            ApplyVirtualProperty();
            CreateImplicitDeclarations();
        }

        internal static CharacterSetBuild SetExpEvaluate(ref ISetExpression Exp)
        {
            CharacterSetBuild characterSetBuild1 = (CharacterSetBuild)null;
            if (Exp is SetItem)
            {
                SetItem setItem = (SetItem)Exp;
                switch (setItem.Type)
                {
                    case SetItem.SetType.Chars:
                        characterSetBuild1 = (CharacterSetBuild)setItem.Characters;
                        break;
                    case SetItem.SetType.Name:
                        CharacterSetBuild characterSet = (CharacterSetBuild)GetCharacterSet(setItem.Text);
                        if (characterSet == null)
                        {
                            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Character set is not defined", "The character set {" + setItem.Text + "} was not defined in the grammar.", "");
                            characterSetBuild1 = new CharacterSetBuild();
                            break;
                        }
                        characterSetBuild1 = new CharacterSetBuild((CharacterSet)characterSet);
                        break;
                }
            }
            else if (Exp is SetExp)
            {
                SetExp setExp = (SetExp)Exp;
                CharacterSetBuild characterSetBuild2 = SetExpEvaluate(ref setExp.LeftOperand);
                CharacterSetBuild characterSetBuild3 = SetExpEvaluate(ref setExp.RightOperand);
                switch ((char)((int)setExp.Operator - 43))
                {
                    case char.MinValue:
                        characterSetBuild2.UnionWith((NumberSet)characterSetBuild3);
                        characterSetBuild1 = characterSetBuild2;
                        break;
                    case '\x0002':
                        characterSetBuild2.Remove((NumberSet)characterSetBuild3);
                        characterSetBuild1 = characterSetBuild2;
                        break;
                    default:
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Internal Error: Invalid set operator.", "Operator: '" + Conversions.ToString(setExp.Operator) + "'", "");
                        characterSetBuild1 = (CharacterSetBuild)new CharacterSet();
                        break;
                }
            }
            else
            {
                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Internal Error: Invalid object in set expression.", "Embedded object: " + Versioned.TypeName((object)Exp), "");
                characterSetBuild1 = new CharacterSetBuild();
            }
            return characterSetBuild1;
        }

        internal static NumberSet SetExpDefinedSets(ref ISetExpression Exp)
        {
            NumberSet numberSet1 = new NumberSet(new int[0]);
            if (Exp is SetItem)
            {
                SetItem setItem = (SetItem)Exp;
                if (setItem.Type == SetItem.SetType.Name)
                {
                    int num = UserDefinedSets.ItemIndex(setItem.Text);
                    if (num != -1)
                        numberSet1.Add(num);
                }
            }
            else if (Exp is SetExp)
            {
                SetExp setExp = (SetExp)Exp;
                NumberSet numberSet2 = SetExpDefinedSets(ref setExp.LeftOperand);
                NumberSet SetB = SetExpDefinedSets(ref setExp.RightOperand);
                numberSet1 = numberSet2.Union(ref SetB);
            }
            else
            {
                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Internal Error: Invalid object in set expression.", "Embedded object: " + Versioned.TypeName((object)Exp), "");
                numberSet1 = (NumberSet)new CharacterSetBuild();
            }
            return numberSet1;
        }

        private static void PopulateSets()
        {
            try
            {
                foreach (Grammar.GrammarSet userSet in (ArrayList)Grammar.UserSets)
                    UserDefinedSets.Add(ref new DefinedCharacterSet(userSet.Name)
                    {
                        Exp = userSet.Exp
                    });
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
            bool flag1 = true;
            try
            {
                foreach (Grammar.GrammarIdentifier usedSetName in (ArrayList)Grammar.UsedSetNames)
                {
                    if (UserDefinedSets.ItemIndex(usedSetName.Name) == -1)
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Undefined Set", "The set {" + usedSetName.Name + "} is used but not defined in the grammar.", Conversions.ToString(usedSetName.Line));
                        flag1 = false;
                    }
                }
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
            if (flag1)
            {
                int num1 = checked(UserDefinedSets.Count - 1);
                int index1 = 0;
                while (index1 <= num1)
                {
                    UserDefinedSets[index1].Dependacy = SetExpDefinedSets(ref UserDefinedSets[index1].Exp);
                    checked { ++index1; }
                }
                bool flag2 = true;
                while (flag2)
                {
                    flag2 = false;
                    try
                    {
                        foreach (DefinedCharacterSet userDefinedSet in (ArrayList)UserDefinedSets)
                        {
                            int num2 = checked(userDefinedSet.Dependacy.Count() - 1);
                            int index2 = 0;
                            while (index2 <= num2)
                            {
                                int index3 = userDefinedSet.Dependacy[index2];
                                if (userDefinedSet.Dependacy.UnionWith(UserDefinedSets[index3].Dependacy))
                                    flag2 = true;
                                checked { ++index2; }
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator enumerator;
                        if (enumerator is IDisposable)
                            (enumerator as IDisposable).Dispose();
                    }
                }
                int num3 = checked(UserDefinedSets.Count - 1);
                int Number = 0;
                while (Number <= num3)
                {
                    if (UserDefinedSets[Number].Dependacy.Contains(Number))
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Circular Set Definition", "The set {" + UserDefinedSets[Number].Name + "} is defined using itself. This can be either direct or through other sets.", "");
                        flag1 = false;
                    }
                    checked { ++Number; }
                }
            }
            if (!flag1)
                return;
            bool flag3 = true;
            while (flag3)
            {
                flag3 = false;
                try
                {
                    foreach (DefinedCharacterSet userDefinedSet in (ArrayList)UserDefinedSets)
                    {
                        CharacterSetBuild characterSetBuild = SetExpEvaluate(ref userDefinedSet.Exp);
                        if (!userDefinedSet.IsEqualSet((NumberSet)characterSetBuild))
                        {
                            userDefinedSet.Copy((NumberSet)characterSetBuild);
                            flag3 = true;
                        }
                    }
                }
                finally
                {
                    IEnumerator enumerator;
                    if (enumerator is IDisposable)
                        (enumerator as IDisposable).Dispose();
                }
            }
        }

        private static void PopulateDefinedSymbols()
        {
            try
            {
                foreach (Grammar.GrammarSymbol terminalDef in (ArrayList)Grammar.TerminalDefs)
                {
                    SymbolBuild symbolBuild = new SymbolBuild();
                    symbolBuild.Name = terminalDef.Name;
                    symbolBuild.Type = terminalDef.Type;
                    symbolBuild.UsesDFA = true;
                    symbolBuild.CreatedBy = CreatorType.Defined;
                    symbolBuild.RegularExp = terminalDef.Exp;
                    BuildTables.Symbol.Add(symbolBuild);
                }
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
        }

        private static void PopulateProductionHeads()
        {
            try
            {
                foreach (Grammar.GrammarProduction production in (ArrayList)Grammar.Productions)
                    BuildTables.Symbol.AddUnique(new SymbolBuild(production.Head.Name, SymbolType.Nonterminal));
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
        }

        private static void PopulateProductions()
        {
            try
            {
                foreach (Grammar.GrammarProduction production in (ArrayList)Grammar.Productions)
                {
                    ProductionBuild productionBuild = new ProductionBuild();
                    int index1 = (int)BuildTables.Symbol.ItemIndex(production.Head.Name, SymbolType.Nonterminal);
                    productionBuild.Head = BuildTables.Symbol[index1];
                    try
                    {
                        foreach (Grammar.GrammarSymbol grammarSymbol in (ArrayList)production.Handle)
                        {
                            int index2 = grammarSymbol.Type != SymbolType.Nonterminal ? (int)BuildTables.Symbol.ItemIndexCategory(grammarSymbol.Name, SymbolCategory.Terminal) : (int)BuildTables.Symbol.ItemIndexCategory(grammarSymbol.Name, SymbolCategory.Nonterminal);
                            productionBuild.Handle().Add(BuildTables.Symbol[index2]);
                        }
                    }
                    finally
                    {
                        IEnumerator enumerator;
                        if (enumerator is IDisposable)
                            (enumerator as IDisposable).Dispose();
                    }
                    BuildTables.Production.Add(ref productionBuild);
                }
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
        }

        private static void PopulateHandleSymbols()
        {
            try
            {
                foreach (Grammar.GrammarSymbol handleSymbol in (ArrayList)Grammar.HandleSymbols)
                {
                    if (handleSymbol.Type == SymbolType.Nonterminal)
                    {
                        if (BuildTables.Symbol.ItemIndexCategory(handleSymbol.Name, SymbolCategory.Nonterminal) == (short)-1)
                            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Undefined rule: <" + handleSymbol.Name + ">", "", Conversions.ToString(handleSymbol.Line));
                    }
                    else if (BuildTables.Symbol.ItemIndexCategory(handleSymbol.Name, SymbolCategory.Terminal) == (short)-1)
                    {
                        SymbolBuild symbolBuild = new SymbolBuild(handleSymbol.Name, SymbolType.Content, true, CreatorType.Implicit);
                        BuildTables.Symbol.Add(symbolBuild);
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, symbolBuild.Name + " was implicitly defined", "The terminal was implicitly declared as " + symbolBuild.Text(false), "");
                    }
                }
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
        }

        private static void CreateImplicitDeclarations()
        {
            int num = checked(BuildTables.Symbol.Count() - 1);
            int index = 0;
            while (index <= num)
            {
                SymbolBuild symbolBuild = BuildTables.Symbol[index];
                if (symbolBuild.Category() == SymbolCategory.Terminal & symbolBuild.UsesDFA & symbolBuild.RegularExp == null)
                {
                    symbolBuild.RegularExp = CreateBasicRegExp(symbolBuild.Name, SetItem.SetType.Sequence, char.MinValue);
                    symbolBuild.CreatedBy = CreatorType.Implicit;
                }
                checked { ++index; }
            }
        }

        private static void LinkGroupSymbolsToGroup()
        {
            int num = checked(BuildTables.Group.Count - 1);
            int index = 0;
            while (index <= num)
            {
                GroupBuild groupBuild = BuildTables.Group[index];
                groupBuild.Container.Group = (Group)groupBuild;
                groupBuild.Start.Group = (Group)groupBuild;
                groupBuild.End.Group = (Group)groupBuild;
                checked { ++index; }
            }
        }

        private static void PopulateGroupsAndWhitespace()
        {
            Grammar.GrammarGroupList grammarGroupList = new Grammar.GrammarGroupList();
            CharacterSetBuild CharSet = new CharacterSetBuild();
            SymbolBuild symbolBuild1 = new SymbolBuild();
            try
            {
                foreach (Grammar.GrammarGroup group in (ArrayList)Grammar.Groups)
                {
                    if (!group.IsBlock)
                        grammarGroupList.Add(group);
                }
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
            if (Grammar.GetParamCharSet() == CharSetMode.ANSI)
                CharSet.Copy((NumberSet)PredefinedSets["Whitespace"]);
            else
                CharSet.Copy((NumberSet)PredefinedSets["All Whitespace"]);
            if (grammarGroupList.Count >= 1)
            {
                RegExp Exp = new RegExp();
                int index = (int)BuildTables.Symbol.ItemIndex("NewLine");
                if (index == -1)
                {
                    CharSet.Remove(10, 13, 8232, 8233);
                    Exp.AddTextExp("{LF}|{CR}{LF}?|{LS}|{PS}");
                    symbolBuild1 = BuildTables.Symbol.AddUnique(new SymbolBuild("NewLine", SymbolType.Noise, Exp));
                    symbolBuild1.CreatedBy = CreatorType.Generated;
                }
                else
                    symbolBuild1 = BuildTables.Symbol[index];
            }
            try
            {
                foreach (Grammar.GrammarGroup group in (ArrayList)Grammar.Groups)
                {
                    SymbolBuild Container = BuildTables.Symbol.AddUnique(new SymbolBuild(group.Container, SymbolType.Content, false, CreatorType.Generated));
                    SymbolBuild Start = BuildTables.Symbol.AddUnique(new SymbolBuild(group.Start, SymbolType.GroupStart, true, CreatorType.Generated));
                    Start.Type = SymbolType.GroupStart;
                    EndingMode Mode;
                    SymbolBuild End;
                    if (group.IsBlock)
                    {
                        Mode = EndingMode.Closed;
                        End = BuildTables.Symbol.AddUnique(new SymbolBuild(group.End, SymbolType.GroupEnd, true, CreatorType.Generated));
                    }
                    else
                    {
                        Mode = EndingMode.Open;
                        End = symbolBuild1;
                    }
                    BuildTables.Group.AddUnique(new GroupBuild(group.Name, Container, Start, End, Mode));
                }
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
            if (BuildTables.Symbol.ItemIndex("Whitespace") == (short)-1 & Operators.CompareString(Microsoft.VisualBasic.Strings.UCase(BuildTables.Properties["Auto Whitespace"].Value), "TRUE", true) == 0)
            {
                RegExp basicRegExp = CreateBasicRegExp(CharSet, '+');
                SymbolBuild symbolBuild2 = new SymbolBuild();
                BuildTables.Symbol.Add(new SymbolBuild("Whitespace", SymbolType.Noise, basicRegExp)
                {
                    CreatedBy = CreatorType.Generated
                });
                Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, "Whitespace was implicitly defined", "The special terminal 'Whitespace' was implicitly defined as: {" + BuilderUtility.DisplayText((CharacterSet)CharSet, true, 1024, "", (short)-1) + "}+", "");
            }
            string str1 = "Whitespace";
            ref string local1 = ref str1;
            SymbolType symbolType = SymbolType.Noise;
            ref SymbolType local2 = ref symbolType;
            int num1 = (int)Reclassify(ref local1, ref local2);
            string str2 = "Comment";
            ref string local3 = ref str2;
            symbolType = SymbolType.Noise;
            ref SymbolType local4 = ref symbolType;
            int num2 = (int)Reclassify(ref local3, ref local4);
        }

        private static short Reclassify(ref string Name, ref SymbolType NewType)
        {
            short num = BuildTables.Symbol.TerminalIndex(Name);
            if (num != (short)-1)
            {
                BuildTables.Symbol[(int)num].Type = NewType;
                BuildTables.Symbol[(int)num].Reclassified = true;
            }
            return num;
        }

        private static RegExp CreateBasicRegExp(string Text, SetItem.SetType Type, char Kleene)
        {
            RegExp regExp = new RegExp();
            RegExpSeq regExpSeq = new RegExpSeq();
            regExpSeq.Add(ref new RegExpItem()
            {
                Data = (object)new SetItem()
                {
                    Text = Text,
                    Type = Type
                },
                Kleene = Conversions.ToString(Kleene)
            });
            regExp.Add(ref regExpSeq);
            return regExp;
        }

        private static RegExp CreateBasicRegExp(CharacterSetBuild CharSet, char Kleene)
        {
            RegExp regExp = new RegExp();
            RegExpSeq regExpSeq = new RegExpSeq();
            regExpSeq.Add(ref new RegExpItem()
            {
                Data = (object)new SetItem()
                {
                    Characters = (CharacterSet)CharSet,
                    Type = SetItem.SetType.Chars
                },
                Kleene = Conversions.ToString(Kleene)
            });
            regExp.Add(ref regExpSeq);
            return regExp;
        }

        private static void AssignTableIndexes()
        {
            short num1 = checked((short)(BuildTables.Symbol.Count() - 1 - 1));
            short num2 = 0;
            while ((int)num2 <= (int)num1)
            {
                int num3 = (int)num2;
                short num4 = checked((short)(BuildTables.Symbol.Count() - 1));
                short num5 = (short)num3;
                while ((int)num5 <= (int)num4)
                {
                    SymbolBuild symbolBuild1 = BuildTables.Symbol[(int)num5];
                    SymbolBuildList symbol1 = BuildTables.Symbol;
                    SymbolBuildList symbolBuildList = symbol1;
                    short num6 = num2;
                    int index = (int)num6;
                    SymbolBuild symbolBuild2 = symbolBuildList[index];
                    ref SymbolBuild local = ref symbolBuild2;
                    int num7 = symbolBuild1.IsLessThan(ref local) ? 1 : 0;
                    symbol1[(int)num6] = symbolBuild2;
                    if (num7 != 0)
                    {
                        Symbol symbol2 = (Symbol)BuildTables.Symbol[(int)num2];
                        BuildTables.Symbol[(int)num2] = BuildTables.Symbol[(int)num5];
                        BuildTables.Symbol[(int)num5] = (SymbolBuild)symbol2;
                    }
                    checked { ++num5; }
                }
                checked { ++num2; }
            }
            short num8 = checked((short)(BuildTables.Symbol.Count() - 1));
            short num9 = 0;
            while ((int)num9 <= (int)num8)
            {
                BuildTables.Symbol[(int)num9].SetTableIndex(num9);
                checked { ++num9; }
            }
            short num10 = checked((short)(BuildTables.Production.Count() - 1));
            short num11 = 0;
            while ((int)num11 <= (int)num10)
            {
                BuildTables.Production[(int)num11].SetTableIndex(num11);
                checked { ++num11; }
            }
            short num12 = checked((short)(BuildTables.Group.Count - 1));
            short num13 = 0;
            while ((int)num13 <= (int)num12)
            {
                BuildTables.Group[(int)num13].TableIndex = num13;
                checked { ++num13; }
            }
        }

        private static void AssignPriorities()
        {
            int num1 = checked(BuildTables.Symbol.Count() - 1);
            int index1 = 0;
            while (index1 <= num1)
            {
                SymbolBuild symbolBuild = BuildTables.Symbol[index1];
                if (symbolBuild.Category() == SymbolCategory.Terminal & symbolBuild.RegularExp != null)
                {
                    bool flag = false;
                    RegExp regularExp = symbolBuild.RegularExp;
                    int num2 = checked(regularExp.Count() - 1);
                    int index2 = 0;
                    while (index2 <= num2)
                    {
                        RegExpSeq regExpSeq = regularExp[index2];
                        if (regExpSeq.Priority == (short)-1)
                        {
                            if (regExpSeq.IsVariableLength())
                            {
                                regExpSeq.Priority = (short)10001;
                                flag = true;
                            }
                            else
                                regExpSeq.Priority = (short)0;
                        }
                        checked { ++index2; }
                    }
                    symbolBuild.VariableLength = flag;
                    if (flag)
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, symbolBuild.Name + " is variable length.");
                }
                checked { ++index1; }
            }
        }

        private static void ApplyGroupAttributes()
        {
            try
            {
                foreach (GrammarAttribAssign groupAttribute in (ArrayList)Grammar.GroupAttributes)
                {
                    int index1 = BuildTables.Group.ItemIndex(groupAttribute.Name);
                    if (index1 == -1)
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Attributes set for undefined terminal/group", "\"" + groupAttribute.Name + "\" was set, but it was not defined.", "");
                    }
                    else
                    {
                        GroupBuild groupBuild = BuildTables.Group[index1];
                        int num1 = checked(groupAttribute.Values.Count - 1);
                        int index2 = 0;
                        while (index2 <= num1)
                        {
                            GrammarAttrib grammarAttrib = groupAttribute.Values[index2];
                            bool flag = false;
                            string upper1 = grammarAttrib.Name.ToUpper();
                            if (Operators.CompareString(upper1, "ADVANCE", true) == 0)
                            {
                                string upper2 = grammarAttrib.Value(", ").ToUpper();
                                if (Operators.CompareString(upper2, "TOKEN", true) == 0)
                                    groupBuild.Advance = AdvanceMode.Token;
                                else if (Operators.CompareString(upper2, "CHARACTER", true) == 0)
                                    groupBuild.Advance = AdvanceMode.Character;
                                else
                                    flag = true;
                            }
                            else if (Operators.CompareString(upper1, "NESTING", true) == 0)
                            {
                                string upper2 = grammarAttrib.Value(", ").ToUpper();
                                if (Operators.CompareString(upper2, "ALL", true) == 0)
                                {
                                    groupBuild.NestingNames = "All";
                                    int num2 = checked(BuildTables.Group.Count - 1);
                                    int num3 = 0;
                                    while (num3 <= num2)
                                    {
                                        groupBuild.Nesting.Add(num3);
                                        checked { ++num3; }
                                    }
                                }
                                else if (Operators.CompareString(upper2, "SELF", true) == 0)
                                {
                                    groupBuild.Nesting.Add((int)groupBuild.TableIndex);
                                    groupBuild.NestingNames = "Self";
                                }
                                else if (Operators.CompareString(upper2, "NONE", true) == 0)
                                    groupBuild.NestingNames = "None";
                                else if (grammarAttrib.IsSet)
                                {
                                    int num2 = checked(grammarAttrib.List.Count - 1);
                                    int index3 = 0;
                                    while (index3 <= num2)
                                    {
                                        int num3 = BuildTables.Group.ItemIndex(grammarAttrib.List[index3]);
                                        if (num3 == -1)
                                            Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Undefined group", "Nesting attribute assignment for the group " + groupBuild.Name + " is invalid. The following was specified: " + grammarAttrib.List[index3], "");
                                        else
                                            groupBuild.Nesting.Add(num3);
                                        checked { ++index3; }
                                    }
                                    groupBuild.NestingNames = grammarAttrib.Value(", ");
                                }
                                else
                                    flag = true;
                            }
                            else if (Operators.CompareString(upper1, "ENDING", true) == 0)
                            {
                                string upper2 = grammarAttrib.Value(", ").ToUpper();
                                if (Operators.CompareString(upper2, "OPEN", true) == 0)
                                    groupBuild.Ending = EndingMode.Open;
                                else if (Operators.CompareString(upper2, "CLOSED", true) == 0)
                                    groupBuild.Ending = EndingMode.Closed;
                                else
                                    flag = true;
                            }
                            else
                                flag = true;
                            if (flag)
                                Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid attribute", "In the attribute assignment for '" + groupAttribute.Name + "', the following was specified: " + grammarAttrib.Name + " = " + grammarAttrib.Value(", "), Conversions.ToString(groupAttribute.Line));
                            checked { ++index2; }
                        }
                    }
                }
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
        }

        private static void ApplySymbolAttributes()
        {
            try
            {
                foreach (GrammarAttribAssign symbolAttribute in (ArrayList)Grammar.SymbolAttributes)
                {
                    int index1 = (int)BuildTables.Symbol.ItemIndex(symbolAttribute.Name);
                    if (index1 == -1)
                    {
                        Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Attributes set for undefined terminal/group", "\"" + symbolAttribute.Name + "\" was set, but it was not defined.", "");
                    }
                    else
                    {
                        SymbolBuild symbolBuild = BuildTables.Symbol[index1];
                        if (symbolBuild.Type == SymbolType.Content | symbolBuild.Type == SymbolType.Noise | symbolBuild.Type == SymbolType.GroupEnd)
                        {
                            int num = checked(symbolAttribute.Values.Count - 1);
                            int index2 = 0;
                            while (index2 <= num)
                            {
                                GrammarAttrib grammarAttrib = symbolAttribute.Values[index2];
                                bool flag = false;
                                string upper1 = grammarAttrib.Name.ToUpper();
                                if (Operators.CompareString(upper1, "TYPE", true) == 0)
                                {
                                    string upper2 = grammarAttrib.Value(", ").ToUpper();
                                    if (Operators.CompareString(upper2, "NOISE", true) == 0)
                                        symbolBuild.Type = SymbolType.Noise;
                                    else if (Operators.CompareString(upper2, "CONTENT", true) == 0)
                                        symbolBuild.Type = SymbolType.Content;
                                    else
                                        flag = true;
                                }
                                else if (Operators.CompareString(upper1, "SOURCE", true) == 0)
                                {
                                    string upper2 = grammarAttrib.Value(", ").ToUpper();
                                    if (Operators.CompareString(upper2, "VIRTUAL", true) == 0)
                                    {
                                        symbolBuild.UsesDFA = false;
                                        Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, symbolBuild.Name + " is a virtual terminal", "This terminal was entered into the symbol table, but not the Deterministic Finite Automata. As a result, tokens must be created at runtime by the developer or a specialized version of the Engine.", "");
                                    }
                                    else if (Operators.CompareString(upper2, "LEXER", true) == 0)
                                        symbolBuild.UsesDFA = true;
                                    else
                                        flag = true;
                                }
                                else
                                    flag = true;
                                if (flag)
                                    Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid attribute", "In the attribute assignment for '" + symbolAttribute.Name + "', the following was specified: " + grammarAttrib.Name + " = " + grammarAttrib.Value(", "), Conversions.ToString(symbolAttribute.Line));
                                checked { ++index2; }
                            }
                        }
                        else
                            Log.Add(SysLogSection.Grammar, SysLogAlert.Warning, "Cannot change attributes", "The attributes for '" + symbolAttribute.Name + "' cannot be changed.", "");
                    }
                }
            }
            finally
            {
                IEnumerator enumerator;
                if (enumerator is IDisposable)
                    (enumerator as IDisposable).Dispose();
            }
            ApplyVirtualProperty();
        }

        private static void ApplyVirtualProperty()
        {
            if (!BuildTables.Properties.Contains("Virtual Terminals"))
                return;
            string[] strArray = Microsoft.VisualBasic.Strings.Split(BuildTables.Properties["Virtual Terminals"].Value, " ", -1, CompareMethod.Text);
            int num = checked(((IEnumerable<string>)strArray).Count<string>() - 1);
            int index = 0;
            while (index <= num)
            {
                string str = strArray[index].Trim();
                if (Operators.CompareString(str, "", true) != 0)
                {
                    BuildTables.Symbol.AddUnique(new SymbolBuild(str, SymbolType.Content, false)).UsesDFA = false;
                    Log.Add(SysLogSection.Grammar, SysLogAlert.Detail, str + " is a virtual terminal", "This terminal was entered into the symbol table, but not the Deterministic Finite Automata. As a result, tokens must be created at runtime by the developer or a specialized version of the Engine.", "");
                }
                checked { ++index; }
            }
        }


        public enum ProgramMode
        {
            Startup,
            Idle,
            Input,
            NFACase,
            NFAClosure,
            BuildingNFA,
            BuildingDFA,
            BuildingFirstSets,
            BuildingLALRClosure,
            BuildingLALR,
        }
    }

}