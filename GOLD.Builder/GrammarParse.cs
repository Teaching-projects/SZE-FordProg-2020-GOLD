using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace GOLD.Builder
{
    internal sealed class GrammarParse
    {
        private static Parser Scanner = new Parser();
        public static bool Accepted;

        public static void Setup()
        {
            string Title = "";
            string str = FileUtility.AppPath();
            bool flag = false;
            if (GrammarParse.Scanner == null)
            {
                flag = true;
                Title = "The scanner object is nothing";
            }
            else if (!GrammarParse.Scanner.LoadTables(str + "\\gp.dat"))
            {
                flag = true;
                Title = "The file 'gp.dat' cannot be loaded.";
            }
            else
            {
                string Left = Strings.Trim(GrammarParse.Scanner.Tables.Properties["Version"].Value);
                if (Operators.CompareString(Left, "", true) == 0)
                {
                    flag = true;
                    Title = "The file 'gp.dat' is invalid: ";
                }
                else if (Operators.CompareString(Left, "5.0.1", true) != 0)
                {
                    flag = true;
                    Title = "The file 'gp.dat' is the incorrect version: '" + Left + "'";
                }
            }
            if (!flag)
                return;
            BuilderApp.Log.Add(SysLogSection.Internal, SysLogAlert.Critical, Title);
        }

        public static bool Parse(TextReader Reader)
        {
            string str1 = "";
            string str2 = "";
            Grammar.Clear();
            GrammarParse.Accepted = false;
            Parser scanner = GrammarParse.Scanner;
            scanner.Open(Reader);
            scanner.TrimReductions = false;
            bool flag = false;
            while (!flag)
            {
                switch (scanner.Parse())
                {
                    case ParseMessage.TokenRead:
                        str2 = Conversions.ToString(scanner.CurrentToken().Data);
                        break;
                    case ParseMessage.Reduction:
                        Parser parser1 = scanner;
                        Parser parser2 = scanner;
                        Reduction currentReduction = (Reduction)parser2.CurrentReduction;
                        object newObject = GrammarParse.CreateNewObject(ref currentReduction);
                        parser2.CurrentReduction = (object)currentReduction;
                        object objectValue1 = RuntimeHelpers.GetObjectValue(newObject);
                        parser1.CurrentReduction = objectValue1;
                        break;
                    case ParseMessage.Accept:
                        flag = true;
                        break;
                    case ParseMessage.NotLoadedError:
                        BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "INTERNAL ERROR", "The scanner was unable to be initialized. Please report this bug.", "");
                        flag = true;
                        break;
                    case ParseMessage.LexicalError:
                        BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Lexical Error", "Cannot recognize the token after: " + str2, Conversions.ToString(scanner.CurrentPosition().Line));
                        flag = true;
                        break;
                    case ParseMessage.SyntaxError:
                        if (scanner.ExpectedSymbols().Count() >= 1)
                        {
                            str1 = GrammarParse.FriendlyTerminalName(scanner.ExpectedSymbols()[0]);
                            short num1 = checked((short)(scanner.ExpectedSymbols().Count() - 1));
                            short num2 = 1;
                            while ((int)num2 <= (int)num1)
                            {
                                str1 = str1 + ", " + GrammarParse.FriendlyTerminalName(scanner.ExpectedSymbols()[(int)num2]);
                                checked { ++num2; }
                            }
                        }
                        SysLog log = BuilderApp.Log;
                        Type Type = typeof(BuilderUtility);
                        object[] objArray1 = new object[2];
                        object[] objArray2 = objArray1;
                        Token token = scanner.CurrentToken();
                        object objectValue2 = RuntimeHelpers.GetObjectValue(token.Data);
                        objArray2[0] = objectValue2;
                        objArray1[1] = (object)false;
                        object[] objArray3 = objArray1;
                        object[] Arguments = objArray3;
                        bool[] flagArray = new bool[2] { true, false };
                        bool[] CopyBack = flagArray;
                        object Right = NewLateBinding.LateGet((object)null, Type, "DisplayText", Arguments, (string[])null, (Type[])null, CopyBack);
                        if (flagArray[0])
                            token.Data = RuntimeHelpers.GetObjectValue(objArray3[0]);
                        string Description = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject((object)"Read: ", Right), (object)"\r\n"), (object)"Expecting: "), (object)str1));
                        string Index = Conversions.ToString(scanner.CurrentPosition().Line);
                        log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Syntax Error", Description, Index);
                        flag = true;
                        break;
                    case ParseMessage.GroupError:
                        BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Runaway Comment", "You have a unterminated block comment.", "");
                        flag = true;
                        break;
                    case ParseMessage.InternalError:
                        BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "INTERNAL ERROR", "The scanner had an internal error. Please report this bug.", "");
                        flag = true;
                        break;
                }
                Notify.Counter = scanner.CurrentPosition().Line;
            }
            Notify.Counter = scanner.CurrentPosition().Line;
            return GrammarParse.Accepted;
        }

        private static object CreateNewObject(ref Reduction TheReduction)
        {
            object Left = (object)null;
            Reduction reduction1 = TheReduction;
            switch (reduction1.Parent.TableIndex)
            {
                case 13:
                    Left = (object)Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)));
                    break;
                case 14:
                    Left = (object)Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)));
                    break;
                case 15:
                    GrammarValueList grammarValueList1 = (GrammarValueList)reduction1.get_Data(0);
                    GrammarValueList grammarValueList2 = grammarValueList1;
                    object[] objArray1 = new object[1];
                    object[] objArray2 = objArray1;
                    Reduction reduction2 = reduction1;
                    Reduction reduction3 = reduction2;
                    int Index1 = 3;
                    int Index2 = Index1;
                    object objectValue1 = RuntimeHelpers.GetObjectValue(reduction3.get_Data(Index2));
                    objArray2[0] = objectValue1;
                    object[] objArray3 = objArray1;
                    object[] Arguments1 = objArray3;
                    bool[] flagArray1 = new bool[1] { true };
                    bool[] CopyBack1 = flagArray1;
                    NewLateBinding.LateCall((object)grammarValueList2, (Type)null, "Add", Arguments1, (string[])null, (Type[])null, CopyBack1, true);
                    if (flagArray1[0])
                        reduction2.set_Data(Index1, RuntimeHelpers.GetObjectValue(objArray3[0]));
                    Left = (object)grammarValueList1;
                    break;
                case 16:
                    GrammarValueList grammarValueList3 = new GrammarValueList();
                    GrammarValueList grammarValueList4 = grammarValueList3;
                    object[] objArray4 = new object[1];
                    object[] objArray5 = objArray4;
                    Reduction reduction4 = reduction1;
                    Reduction reduction5 = reduction4;
                    int Index3 = 0;
                    int Index4 = Index3;
                    object objectValue2 = RuntimeHelpers.GetObjectValue(reduction5.get_Data(Index4));
                    objArray5[0] = objectValue2;
                    object[] objArray6 = objArray4;
                    object[] Arguments2 = objArray6;
                    bool[] flagArray2 = new bool[1] { true };
                    bool[] CopyBack2 = flagArray2;
                    NewLateBinding.LateCall((object)grammarValueList4, (Type)null, "Add", Arguments2, (string[])null, (Type[])null, CopyBack2, true);
                    if (flagArray2[0])
                        reduction4.set_Data(Index3, RuntimeHelpers.GetObjectValue(objArray6[0]));
                    Left = (object)grammarValueList3;
                    break;
                case 17:
                    Left = (object)(Conversions.ToString(reduction1.get_Data(0)) + " " + reduction1.get_Data(1).ToString());
                    break;
                case 18:
                    Left = RuntimeHelpers.GetObjectValue(reduction1.get_Data(0));
                    break;
                case 19:
                    Left = (object)Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)));
                    break;
                case 20:
                    Left = (object)Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)));
                    break;
                case 21:
                    Left = (object)Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)));
                    break;
                case 22:
                    Grammar.AddProperty(Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0))), ((GrammarValueList)reduction1.get_Data(3)).ToString(), reduction1[0].Position.Line);
                    break;
                case 23:
                    GrammarValueList grammarValueList5 = (GrammarValueList)reduction1.get_Data(0);
                    GrammarValueList List = (GrammarValueList)reduction1.get_Data(3);
                    grammarValueList5.Add(List);
                    Left = (object)grammarValueList5;
                    break;
                case 24:
                    Left = RuntimeHelpers.GetObjectValue(reduction1.get_Data(0));
                    break;
                case 25:
                    Grammar.AddSymbolAttrib(new GrammarAttribAssign()
                    {
                        Name = Conversions.ToString(reduction1.get_Data(0)),
                        Values = (GrammarAttribList)reduction1.get_Data(4),
                        Line = reduction1[2].Position.Line
                    });
                    break;
                case 26:
                    Grammar.AddGroupAttrib(new GrammarAttribAssign()
                    {
                        Name = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(reduction1.get_Data(0), (object)" "), reduction1.get_Data(1))),
                        Values = (GrammarAttribList)reduction1.get_Data(5),
                        Line = reduction1[3].Position.Line
                    });
                    break;
                case 27:
                    GrammarAttribList grammarAttribList = (GrammarAttribList)reduction1.get_Data(0);
                    grammarAttribList.Add((GrammarAttrib)reduction1.get_Data(3));
                    Left = (object)grammarAttribList;
                    break;
                case 28:
                    Left = (object)new GrammarAttribList()
        {
          (GrammarAttrib) reduction1.get_Data(0)
        };
                    break;
                case 29:
                    GrammarAttrib grammarAttrib = new GrammarAttrib();
                    grammarAttrib.Name = Conversions.ToString(reduction1.get_Data(0));
                    GrammarValueList list = grammarAttrib.List;
                    object[] objArray7 = new object[1];
                    object[] objArray8 = objArray7;
                    Reduction reduction6 = reduction1;
                    Reduction reduction7 = reduction6;
                    int Index5 = 2;
                    int Index6 = Index5;
                    object objectValue3 = RuntimeHelpers.GetObjectValue(reduction7.get_Data(Index6));
                    objArray8[0] = objectValue3;
                    object[] objArray9 = objArray7;
                    object[] Arguments3 = objArray9;
                    bool[] flagArray3 = new bool[1] { true };
                    bool[] CopyBack3 = flagArray3;
                    NewLateBinding.LateCall((object)list, (Type)null, "Add", Arguments3, (string[])null, (Type[])null, CopyBack3, true);
                    if (flagArray3[0])
                        reduction6.set_Data(Index5, RuntimeHelpers.GetObjectValue(objArray9[0]));
                    grammarAttrib.IsSet = false;
                    Left = (object)grammarAttrib;
                    break;
                case 30:
                    Left = (object)new GrammarAttrib()
                    {
                        Name = Conversions.ToString(reduction1.get_Data(0)),
                        List = (GrammarValueList)reduction1.get_Data(3),
                        IsSet = true
                    };
                    break;
                case 31:
                    Grammar.GrammarSet CharSet = new Grammar.GrammarSet();
                    CharSet.Name = Conversions.ToString(reduction1.get_Data(1));
                    CharSet.Exp = (ISetExpression)reduction1.get_Data(5);
                    CharSet.Line = reduction1[0].Position.Line;
                    Grammar.AddUserSet(CharSet);
                    break;
                case 32:
                    Left = (object)new SetExp((ISetExpression)reduction1.get_Data(0), '+', (ISetExpression)reduction1.get_Data(3));
                    break;
                case 33:
                    Left = (object)new SetExp((ISetExpression)reduction1.get_Data(0), '-', (ISetExpression)reduction1.get_Data(3));
                    break;
                case 34:
                    Left = RuntimeHelpers.GetObjectValue(reduction1.get_Data(0));
                    break;
                case 35:
                    Reduction reduction8 = reduction1;
                    Reduction reduction9 = reduction8;
                    int Index7 = 0;
                    int Index8 = Index7;
                    string FullText = Conversions.ToString(reduction9.get_Data(Index8));
                    Grammar.WarnRegexSetLiteral(ref FullText, reduction1[0].Position.Line);
                    reduction8.set_Data(Index7, (object)FullText);
                    Left = (object)new SetItem(new CharacterSetBuild(Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)))));
                    break;
                case 36:
                    Grammar.AddUsedSetName(Conversions.ToString(reduction1.get_Data(1)), reduction1[0].Position.Line);
                    Left = (object)new SetItem(SetItem.SetType.Name, Conversions.ToString(reduction1.get_Data(1)));
                    break;
                case 37:
                    Left = (object)new SetItem((CharacterSetBuild)reduction1.get_Data(1));
                    break;
                case 38:
                    Left = Operators.ConcatenateObject(Operators.ConcatenateObject(reduction1.get_Data(0), (object)" "), reduction1.get_Data(1));
                    break;
                case 39:
                    Left = RuntimeHelpers.GetObjectValue(reduction1.get_Data(0));
                    break;
                case 40:
                    CharacterSetBuild characterSetBuild1 = (CharacterSetBuild)reduction1.get_Data(0);
                    CharacterSetRange characterSetRange1 = (CharacterSetRange)reduction1.get_Data(3);
                    characterSetBuild1.AddRange(characterSetRange1.First, characterSetRange1.Last);
                    Left = (object)characterSetBuild1;
                    break;
                case 41:
                    CharacterSetBuild characterSetBuild2 = new CharacterSetBuild();
                    CharacterSetRange characterSetRange2 = (CharacterSetRange)reduction1.get_Data(0);
                    characterSetBuild2.AddRange(characterSetRange2.First, characterSetRange2.Last);
                    Left = (object)characterSetBuild2;
                    break;
                case 42:
                    Left = (object)new CharacterSetRange(Conversions.ToInteger(reduction1.get_Data(0)), Conversions.ToInteger(reduction1.get_Data(0)));
                    break;
                case 43:
                    Left = (object)new CharacterSetRange(Conversions.ToInteger(reduction1.get_Data(0)), Conversions.ToInteger(reduction1.get_Data(2)));
                    break;
                case 44:
                    Left = (object)Grammar.GetHexValue(Conversions.ToString(reduction1.get_Data(0)));
                    if (Conversions.ToBoolean(Operators.OrObject(Operators.CompareObjectLess(Left, (object)0, true), Operators.CompareObjectGreater(Left, (object)(int)ushort.MaxValue, true))))
                    {
                        BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid set constant value", Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject((object)"The value '", reduction1.get_Data(0)), (object)"' is not valid.")), Conversions.ToString(reduction1[0].Position.Line));
                        break;
                    }
                    break;
                case 45:
                    Left = (object)Grammar.GetDecValue(Conversions.ToString(reduction1.get_Data(0)));
                    if (Conversions.ToBoolean(Operators.OrObject(Operators.CompareObjectLess(Left, (object)0, true), Operators.CompareObjectGreater(Left, (object)(int)ushort.MaxValue, true))))
                    {
                        BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid set constant value", Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject((object)"The value '", reduction1.get_Data(0)), (object)"' is not valid.")), Conversions.ToString(reduction1[0].Position.Line));
                        break;
                    }
                    break;
                case 46:
                    string str = Conversions.ToString(reduction1.get_Data(1));
                    string upper = str.ToUpper();
                    if (Operators.CompareString(upper, "LINE", true) == 0 || Operators.CompareString(upper, "START", true) == 0 || Operators.CompareString(upper, "END", true) == 0)
                    {
                        Grammar.AddGroup(new Grammar.GrammarGroup(Conversions.ToString(reduction1.get_Data(0)), Conversions.ToString(reduction1.get_Data(1)), Conversions.ToString(reduction1.get_Data(4)), reduction1[1].Position.Line));
                        break;
                    }
                    BuilderApp.Log.Add(SysLogSection.Grammar, SysLogAlert.Critical, "Invalid group usage value", "The usage value '" + str + "' is not valid. It can be either Start, End or Line.", Conversions.ToString(reduction1[1].Position.Line));
                    break;
                case 47:
                    Left = (object)Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)));
                    break;
                case 48:
                    Left = (object)Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)));
                    break;
                case 49:
                    Grammar.GrammarSymbol Sym1 = new Grammar.GrammarSymbol();
                    Sym1.Name = Conversions.ToString(reduction1.get_Data(0));
                    Sym1.Exp = (RegExp)reduction1.get_Data(3);
                    Sym1.Line = reduction1[2].Position.Line;
                    Sym1.Type = SymbolType.Content;
                    Grammar.AddTerminalHead(ref Sym1);
                    break;
                case 50:
                    RegExp regExp1 = (RegExp)reduction1.get_Data(0);
                    RegExpSeq regExpSeq1 = (RegExpSeq)reduction1.get_Data(3);
                    regExpSeq1.Priority = (short)-1;
                    regExp1.Add(ref regExpSeq1);
                    Left = (object)regExp1;
                    break;
                case 51:
                    RegExp regExp2 = new RegExp();
                    RegExpSeq regExpSeq2 = (RegExpSeq)reduction1.get_Data(0);
                    regExpSeq2.Priority = (short)-1;
                    regExp2.Add(ref regExpSeq2);
                    Left = (object)regExp2;
                    break;
                case 52:
                    RegExpSeq regExpSeq3 = (RegExpSeq)reduction1.get_Data(0);
                    RegExpSeq regExpSeq4 = regExpSeq3;
                    Reduction reduction10 = reduction1;
                    Reduction reduction11 = reduction10;
                    int Index9 = 1;
                    int Index10 = Index9;
                    RegExpItem regExpItem1 = (RegExpItem)reduction11.get_Data(Index10);
                    ref RegExpItem local1 = ref regExpItem1;
                    regExpSeq4.Add(ref local1);
                    reduction10.set_Data(Index9, (object)regExpItem1);
                    Left = (object)regExpSeq3;
                    break;
                case 53:
                    RegExpSeq regExpSeq5 = new RegExpSeq();
                    RegExpSeq regExpSeq6 = regExpSeq5;
                    Reduction reduction12 = reduction1;
                    Reduction reduction13 = reduction12;
                    int Index11 = 0;
                    int Index12 = Index11;
                    RegExpItem regExpItem2 = (RegExpItem)reduction13.get_Data(Index12);
                    ref RegExpItem local2 = ref regExpItem2;
                    regExpSeq6.Add(ref local2);
                    reduction12.set_Data(Index11, (object)regExpItem2);
                    Left = (object)regExpSeq5;
                    break;
                case 54:
                    Left = (object)new RegExpItem((object)(SetItem)reduction1.get_Data(0), Conversions.ToString(reduction1.get_Data(1)));
                    break;
                case 55:
                    Left = (object)new RegExpItem((object)new SetItem(SetItem.SetType.Sequence, Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)))), Conversions.ToString(reduction1.get_Data(1)));
                    break;
                case 56:
                    Left = (object)new RegExpItem((object)new SetItem(SetItem.SetType.Sequence, Conversions.ToString(reduction1.get_Data(0))), Conversions.ToString(reduction1.get_Data(1)));
                    break;
                case 57:
                    Left = (object)new RegExpItem(RuntimeHelpers.GetObjectValue(reduction1.get_Data(1)), Conversions.ToString(reduction1.get_Data(3)));
                    break;
                case 58:
                    RegExp regExp3 = (RegExp)reduction1.get_Data(0);
                    RegExp regExp4 = regExp3;
                    Reduction reduction14 = reduction1;
                    Reduction reduction15 = reduction14;
                    int Index13 = 2;
                    int Index14 = Index13;
                    RegExpSeq regExpSeq7 = (RegExpSeq)reduction15.get_Data(Index14);
                    ref RegExpSeq local3 = ref regExpSeq7;
                    regExp4.Add(ref local3);
                    reduction14.set_Data(Index13, (object)regExpSeq7);
                    Left = (object)regExp3;
                    break;
                case 59:
                    RegExp regExp5 = new RegExp();
                    RegExp regExp6 = regExp5;
                    Reduction reduction16 = reduction1;
                    Reduction reduction17 = reduction16;
                    int Index15 = 0;
                    int Index16 = Index15;
                    RegExpSeq regExpSeq8 = (RegExpSeq)reduction17.get_Data(Index16);
                    ref RegExpSeq local4 = ref regExpSeq8;
                    regExp6.Add(ref local4);
                    reduction16.set_Data(Index15, (object)regExpSeq8);
                    Left = (object)regExp5;
                    break;
                case 60:
                    Left = (object)"+";
                    break;
                case 61:
                    Left = (object)"?";
                    break;
                case 62:
                    Left = (object)"*";
                    break;
                case 63:
                    Left = (object)"";
                    break;
                case 64:
                    Grammar.GrammarProductionList grammarProductionList1 = (Grammar.GrammarProductionList)reduction1.get_Data(3);
                    Grammar.GrammarSymbol grammarSymbol = new Grammar.GrammarSymbol();
                    grammarSymbol.Name = Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0)));
                    grammarSymbol.Type = SymbolType.Nonterminal;
                    grammarSymbol.Line = reduction1[2].Position.Line;
                    try
                    {
                        foreach (Grammar.GrammarProduction Prod in (ArrayList)grammarProductionList1)
                        {
                            Prod.Head = grammarSymbol;
                            Grammar.AddProduction(Prod);
                        }
                        break;
                    }
                    finally
                    {
                        IEnumerator enumerator;
                        if (enumerator is IDisposable)
                            (enumerator as IDisposable).Dispose();
                    }
                case 65:
                    Grammar.GrammarProductionList grammarProductionList2 = (Grammar.GrammarProductionList)reduction1.get_Data(0);
                    grammarProductionList2.Add(new Grammar.GrammarProduction()
                    {
                        Handle = (Grammar.GrammarSymbolList)reduction1.get_Data(3),
                        Line = reduction1[2].Position.Line
                    });
                    Left = (object)grammarProductionList2;
                    break;
                case 66:
                    Left = (object)new Grammar.GrammarProductionList()
        {
          new Grammar.GrammarProduction()
          {
            Handle = (Grammar.GrammarSymbolList) reduction1.get_Data(0)
          }
        };
                    break;
                case 67:
                    Left = (object)(Grammar.GrammarSymbolList)reduction1.get_Data(0);
                    break;
                case 68:
                    Left = (object)new Grammar.GrammarSymbolList();
                    break;
                case 69:
                    Grammar.GrammarSymbolList grammarSymbolList = (Grammar.GrammarSymbolList)reduction1.get_Data(0);
                    grammarSymbolList.Add((Grammar.GrammarSymbol)reduction1.get_Data(1));
                    Left = (object)grammarSymbolList;
                    break;
                case 70:
                    Left = (object)new Grammar.GrammarSymbolList();
                    break;
                case 71:
                    Grammar.GrammarSymbol Sym2 = new Grammar.GrammarSymbol(Conversions.ToString(reduction1.get_Data(0)), SymbolType.Content, reduction1[0].Position.Line);
                    Grammar.AddHandleSymbol(Sym2);
                    Left = (object)Sym2;
                    break;
                case 72:
                    Grammar.GrammarSymbol Sym3 = new Grammar.GrammarSymbol(Grammar.TokenText(Conversions.ToString(reduction1.get_Data(0))), SymbolType.Nonterminal, reduction1[0].Position.Line);
                    Grammar.AddHandleSymbol(Sym3);
                    Left = (object)Sym3;
                    break;
            }
            return Left;
        }

        private static string FriendlyTerminalName(Symbol Sym)
        {
            string str;
            switch ((short)((int)Sym.TableIndex - 22))
            {
                case 0:
                    str = "Decimal Number";
                    break;
                case 1:
                    str = "Hexadecimal Number";
                    break;
                case 2:
                    str = "Identifier";
                    break;
                case 3:
                    str = "Literal";
                    break;
                case 4:
                    str = "New Line";
                    break;
                case 5:
                    str = "Nonterminal";
                    break;
                case 6:
                    str = "Parameter Name";
                    break;
                case 7:
                    str = "Set Literal";
                    break;
                default:
                    str = Sym.Name;
                    break;
            }
            return str;
        }


        private enum SymbolIndex
        {
            Eof,
            Error,
            Comment,
            Whitespace,
            Exclam,
            Exclamtimes,
            Timesexclam,
            Minus,
            Lparan,
            Rparan,
            Times,
            Comma,
            Dotdot,
            Coloncoloneq,
            Question,
            Ateq,
            Lbrace,
            Pipe,
            Rbrace,
            Plus,
            Ltgt,
            Eq,
            Decnumber,
            Hexnumber,
            Identifier,
            Literal,
            Newline,
            Nonterminal,
            Parametername,
            Setliteral,
            Attributedecl,
            Attributeitem,
            Attributelist,
            Charcodeitem,
            Charcodelist,
            Charcodevalue,
            Content,
            Definition,
            Grammar,
            Groupdecl,
            Groupitem,
            Handle,
            Handles,
            Idseries,
            Kleeneopt,
            Nl,
            Nlo,
            Param,
            Parambody,
            Regexpitem,
            Regexpseq,
            Ruledecl,
            Setdecl,
            Setexp,
            Setitem,
            Subregexp,
            Symbol,
            Symbols,
            Terminalbody,
            Terminaldecl,
            Terminalname,
            Valueitem,
            Valueitems,
            Valuelist,
        }


        private enum ProductionIndex
        {
            Grammar,
            Content,
            Content2,
            Definition,
            Definition2,
            Definition3,
            Definition4,
            Definition5,
            Definition6,
            Nlo_Newline,
            Nlo,
            Nl_Newline,
            Nl_Newline2,
            Terminalname_Identifier,
            Terminalname_Literal,
            Valuelist_Comma,
            Valuelist,
            Valueitems,
            Valueitems2,
            Valueitem_Identifier,
            Valueitem_Nonterminal,
            Valueitem_Literal,
            Param_Parametername_Eq,
            Parambody_Pipe,
            Parambody,
            Attributedecl_Ateq_Lbrace_Rbrace,
            Attributedecl_Identifier_Ateq_Lbrace_Rbrace,
            Attributelist_Comma,
            Attributelist,
            Attributeitem_Identifier_Eq_Identifier,
            Attributeitem_Identifier_Eq_Lbrace_Rbrace,
            Setdecl_Lbrace_Rbrace_Eq,
            Setexp_Plus,
            Setexp_Minus,
            Setexp,
            Setitem_Setliteral,
            Setitem_Lbrace_Rbrace,
            Setitem_Lbrace_Rbrace2,
            Idseries_Identifier,
            Idseries_Identifier2,
            Charcodelist_Comma,
            Charcodelist,
            Charcodeitem,
            Charcodeitem_Dotdot,
            Charcodevalue_Hexnumber,
            Charcodevalue_Decnumber,
            Groupdecl_Identifier_Eq,
            Groupitem_Identifier,
            Groupitem_Literal,
            Terminaldecl_Eq,
            Terminalbody_Pipe,
            Terminalbody,
            Regexpseq,
            Regexpseq2,
            Regexpitem,
            Regexpitem_Literal,
            Regexpitem_Identifier,
            Regexpitem_Lparan_Rparan,
            Subregexp_Pipe,
            Subregexp,
            Kleeneopt_Plus,
            Kleeneopt_Question,
            Kleeneopt_Times,
            Kleeneopt,
            Ruledecl_Nonterminal_Coloncoloneq,
            Handles_Pipe,
            Handles,
            Handle,
            Handle_Ltgt,
            Symbols,
            Symbols2,
            Symbol,
            Symbol_Nonterminal,
        }
    }
}