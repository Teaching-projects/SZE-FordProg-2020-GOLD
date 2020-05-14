using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace GOLD.Builder
{
    internal class Parser
    {
        public ParseTables m_Tables;
        private string m_LookaheadBuffer;
        private int m_CurrentLALR;
        private TokenStack m_Stack;
        private SymbolList m_ExpectedSymbols;
        private bool m_HaveReduction;
        private bool m_TrimReductions;
        private TokenDeque m_InputTokens;
        private TextReader m_Source;
        private Position m_SysPosition;
        private Position m_CurrentPosition;
        private TokenStack m_GroupStack;

        public Parser()
        {
            this.m_Tables = new ParseTables();
            this.m_Stack = new TokenStack();
            this.m_ExpectedSymbols = new SymbolList();
            this.m_InputTokens = new TokenDeque();
            this.m_SysPosition = new Position();
            this.m_CurrentPosition = new Position();
            this.m_GroupStack = new TokenStack();
            this.Restart();
            this.m_Tables.Clear();
            this.m_TrimReductions = false;
        }

        public bool Open(ref string Text)
        {
            this.Restart();
            this.m_Source = (TextReader)new StringReader(Text);
            return true;
        }

        public bool Open(TextReader Reader)
        {
            this.Restart();
            this.m_Source = Reader;
            return true;
        }

        public object CurrentReduction
        {
            get
            {
                return !this.m_HaveReduction ? (object)null : RuntimeHelpers.GetObjectValue(this.m_Stack.Top().Data);
            }
            set
            {
                if (!this.m_HaveReduction)
                    return;
                this.m_Stack.Top().Data = RuntimeHelpers.GetObjectValue(value);
            }
        }

        public bool TrimReductions
        {
            get
            {
                return this.m_TrimReductions;
            }
            set
            {
                this.m_TrimReductions = value;
            }
        }

        internal short CurrentLALRState()
        {
            return checked((short)this.m_CurrentLALR);
        }

        public Position CurrentPosition()
        {
            return this.m_CurrentPosition;
        }

        public Token CurrentToken()
        {
            return this.m_InputTokens.Top();
        }

        public Token DiscardCurrentToken()
        {
            return this.m_InputTokens.Dequeue();
        }

        public void EnqueueInput(ref Token TheToken)
        {
            this.m_InputTokens.Enqueue(ref TheToken);
        }

        public void PushInput(ref Token TheToken)
        {
            this.m_InputTokens.Push(TheToken);
        }

        private string LookaheadBuffer(int CharCount)
        {
            string str = "";
            if (CharCount <= this.m_LookaheadBuffer.Length)
                str = this.m_LookaheadBuffer.Substring(0, CharCount);
            return str;
        }

        private string Lookahead(int CharIndex)
        {
            if (CharIndex > this.m_LookaheadBuffer.Length)
            {
                int num1 = checked(CharIndex - this.m_LookaheadBuffer.Length);
                int num2 = 1;
                while (num2 <= num1)
                {
                    this.m_LookaheadBuffer += Conversions.ToString(Strings.ChrW(this.m_Source.Read()));
                    checked { ++num2; }
                }
            }
            if (CharIndex <= this.m_LookaheadBuffer.Length)
                return Conversions.ToString(this.m_LookaheadBuffer[checked(CharIndex - 1)]);
            return "";
        }

        internal void Clear()
        {
            this.m_Tables.Clear();
            this.m_Stack.Clear();
            this.m_InputTokens.Clear();
            this.m_GroupStack.Clear();
            this.m_Tables.Group.Clear();
            this.Restart();
        }

        public bool LoadTables(string Path)
        {
            bool flag;
            try
            {
                flag = this.m_Tables.Load(ref Path);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Information.Err().Clear();
                flag = false;
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        internal ParseTables Tables
        {
            get
            {
                return this.m_Tables;
            }
            set
            {
                this.m_Tables = value;
            }
        }

        internal SymbolList ExpectedSymbols()
        {
            return this.m_ExpectedSymbols;
        }

        private Parser.ParseResult ParseLALR(ref Token NextToken)
        {
            LRAction lrAction1 = this.m_Tables.LALR[this.m_CurrentLALR][NextToken.Parent];
            Parser.ParseResult parseResult;
            if (lrAction1 != null)
            {
                this.m_HaveReduction = false;
                switch (lrAction1.Type())
                {
                    case LRActionType.Shift:
                        this.m_CurrentLALR = (int)lrAction1.Value();
                        NextToken.State = checked((short)this.m_CurrentLALR);
                        this.m_Stack.Push(ref NextToken);
                        parseResult = Parser.ParseResult.Shift;
                        break;
                    case LRActionType.Reduce:
                        Production production = this.m_Tables.Production[(int)lrAction1.Value()];
                        Token TheToken;
                        if (this.m_TrimReductions & production.ContainsOneNonTerminal())
                        {
                            TheToken = this.m_Stack.Pop();
                            TheToken.Parent = production.Head;
                            parseResult = Parser.ParseResult.ReduceEliminated;
                        }
                        else
                        {
                            this.m_HaveReduction = true;
                            Reduction reduction1 = new Reduction(production.Handle().Count());
                            Reduction reduction2 = reduction1;
                            reduction2.Parent = production;
                            short num = checked((short)(production.Handle().Count() - 1));
                            while (num >= (short)0)
                            {
                                reduction2[(int)num] = this.m_Stack.Pop();
                                checked { num += (short)-1; }
                            }
                            TheToken = new Token(production.Head, (object)reduction1);
                            parseResult = Parser.ParseResult.ReduceNormal;
                        }
                        short state = this.m_Stack.Top().State;
                        short index = this.m_Tables.LALR[(int)state].IndexOf(production.Head);
                        if (index != (short)-1)
                        {
                            this.m_CurrentLALR = (int)this.m_Tables.LALR[(int)state][index].Value();
                            TheToken.State = checked((short)this.m_CurrentLALR);
                            this.m_Stack.Push(ref TheToken);
                        }
                        else
                            parseResult = Parser.ParseResult.InternalError;
                        break;
                    case LRActionType.Accept:
                        this.m_HaveReduction = true;
                        parseResult = Parser.ParseResult.Accept;
                        break;
                }
            }
            else
            {
                this.m_ExpectedSymbols.Clear();
                try
                {
                    foreach (LRAction lrAction2 in (ArrayList)this.m_Tables.LALR[this.m_CurrentLALR])
                    {
                        if (lrAction2.Symbol.Type == SymbolType.Content | lrAction2.Symbol.Type == SymbolType.End)
                            this.m_ExpectedSymbols.Add(lrAction2.Symbol);
                    }
                }
                finally
                {
                    IEnumerator enumerator;
                    if (enumerator is IDisposable)
                        (enumerator as IDisposable).Dispose();
                }
                parseResult = Parser.ParseResult.SyntaxError;
            }
            return parseResult;
        }

        public void Restart()
        {
            this.m_SysPosition.Column = 0;
            this.m_SysPosition.Line = 0;
            this.m_CurrentPosition.Line = 0;
            this.m_CurrentPosition.Column = 0;
            this.m_InputTokens.Clear();
            this.m_LookaheadBuffer = "";
            this.m_GroupStack.Clear();
            this.m_CurrentLALR = (int)this.m_Tables.LALR.InitialState;
            this.m_Stack.Clear();
            this.m_Stack.Push(ref new Token()
            {
                State = this.m_Tables.LALR.InitialState
            });
            this.m_HaveReduction = false;
            this.m_ExpectedSymbols.Clear();
        }

        public bool TablesLoaded()
        {
            return this.m_Tables.IsLoaded();
        }

        private Token LookaheadDFA()
        {
        label_1:
            int num1;
            Token token1;
            int num2;
            try
            {
                int num3 = 1;
                Token token2 = new Token();
            label_2:
                ProjectData.ClearProjectError();
                num1 = -2;
            label_3:
                num3 = 3;
                bool flag1 = false;
            label_4:
                num3 = 4;
                int index1 = (int)this.m_Tables.DFA.InitialState;
            label_5:
                num3 = 5;
                int CharIndex1 = 1;
            label_6:
                num3 = 6;
                int index2 = -1;
            label_7:
                num3 = 7;
                int CharCount = -1;
            label_8:
                num3 = 8;
                string str = this.Lookahead(1);
            label_9:
                num3 = 9;
                if (Operators.CompareString(str, "", true) == 0 | Strings.AscW(str) == (int)ushort.MaxValue)
                    goto label_44;
                else
                    goto label_43;
                label_10:
                num3 = 12;
                str = this.Lookahead(CharIndex1);
            label_11:
                num3 = 13;
                if (Operators.CompareString(str, "", true) != 0)
                    goto label_13;
                label_12:
                num3 = 14;
                bool flag2 = false;
                goto label_24;
            label_13:
                num3 = 16;
            label_14:
                num3 = 17;
                int index3 = 0;
            label_15:
                num3 = 18;
                flag2 = false;
                goto label_23;
            label_16:
                num3 = 21;
                FAEdge edge = this.m_Tables.DFA[index1].Edges()[index3];
            label_17:
                num3 = 22;
                if (!edge.Characters.Contains(Strings.AscW(str)))
                    goto label_20;
                label_18:
                num3 = 23;
                flag2 = true;
            label_19:
                num3 = 24;
                int target = edge.Target;
            label_20:
            label_21:
                num3 = 26;
                checked { ++index3; }
            label_22:
            label_23:
                num3 = 20;
                if (index3 < this.m_Tables.DFA[index1].Edges().Count() & !flag2)
                    goto label_16;
                label_24:
            label_25:
                num3 = 29;
                if (!flag2)
                    goto label_32;
                label_26:
                num3 = 30;
                if (this.m_Tables.DFA[target].Accept == null)
                    goto label_29;
                label_27:
                num3 = 31;
                index2 = target;
            label_28:
                num3 = 32;
                CharCount = CharIndex1;
            label_29:
            label_30:
                num3 = 34;
                index1 = target;
            label_31:
                num3 = 35;
                checked { ++CharIndex1; }
                goto label_41;
            label_32:
                num3 = 37;
            label_33:
                num3 = 38;
                flag1 = true;
            label_34:
                num3 = 39;
                if (index2 != -1)
                    goto label_37;
                label_35:
                num3 = 40;
                token2.Parent = this.m_Tables.Symbol.GetFirstOfType(SymbolType.Error);
            label_36:
                num3 = 41;
                token2.Data = (object)this.LookaheadBuffer(1);
                goto label_40;
            label_37:
                num3 = 43;
            label_38:
                num3 = 44;
                token2.Parent = this.m_Tables.DFA[index2].Accept;
            label_39:
                num3 = 45;
                token2.Data = (object)this.LookaheadBuffer(CharCount);
            label_40:
            label_41:
            label_42:
            label_43:
                num3 = 11;
                if (!flag1)
                    goto label_10;
                else
                    goto label_47;
                label_44:
                num3 = 50;
            label_45:
                num3 = 51;
                token2.Data = (object)"";
            label_46:
                num3 = 52;
                token2.Parent = this.m_Tables.Symbol.GetFirstOfType(SymbolType.End);
            label_47:
            label_48:
                num3 = 54;
                if (token2.Type() != SymbolType.LEGACYCommentLine)
                    goto label_61;
                label_49:
                num3 = 55;
                token2.Parent = token2.Group().Container;
            label_50:
                num3 = 56;
                bool flag3 = false;
            label_51:
                num3 = 57;
                int CharIndex2 = checked(CharCount + 1);
                goto label_60;
            label_52:
                num3 = 60;
                str = this.Lookahead(CharIndex2);
            label_53:
                num3 = 61;
                string Left = str;
            label_54:
                num3 = 64;
                if (Operators.CompareString(Left, "\n", true) != 0 && Operators.CompareString(Left, "\r", true) != 0 && Operators.CompareString(Left, "", true) != 0)
                    goto label_56;
                label_55:
                num3 = 65;
                flag3 = true;
                goto label_58;
            label_56:
                num3 = 68;
                Token token3 = token2;
                token3.Data = Operators.ConcatenateObject(token3.Data, (object)str);
            label_57:
                num3 = 69;
                checked { ++CharIndex2; }
            label_58:
            label_59:
            label_60:
                num3 = 59;
                if (!(flag3 | this.m_Source.Peek() == -1))
                    goto label_52;
                label_61:
            label_62:
                num3 = 73;
                token2.Position.Copy(this.m_SysPosition);
            label_63:
                token1 = token2;
                goto label_70;
            label_65:
                num2 = num3;
                switch (num1 > -2 ? num1 : 1)
                {
                    case 1:
                        int num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                goto label_1;
                            case 2:
                                goto label_2;
                            case 3:
                                goto label_3;
                            case 4:
                                goto label_4;
                            case 5:
                                goto label_5;
                            case 6:
                                goto label_6;
                            case 7:
                                goto label_7;
                            case 8:
                                goto label_8;
                            case 9:
                                goto label_9;
                            case 10:
                            case 11:
                                goto label_43;
                            case 12:
                                goto label_10;
                            case 13:
                                goto label_11;
                            case 14:
                                goto label_12;
                            case 15:
                            case 28:
                                goto label_24;
                            case 16:
                                goto label_13;
                            case 17:
                                goto label_14;
                            case 18:
                                goto label_15;
                            case 19:
                            case 20:
                                goto label_23;
                            case 21:
                                goto label_16;
                            case 22:
                                goto label_17;
                            case 23:
                                goto label_18;
                            case 24:
                                goto label_19;
                            case 25:
                                goto label_20;
                            case 26:
                                goto label_21;
                            case 27:
                                goto label_22;
                            case 29:
                                goto label_25;
                            case 30:
                                goto label_26;
                            case 31:
                                goto label_27;
                            case 32:
                                goto label_28;
                            case 33:
                                goto label_29;
                            case 34:
                                goto label_30;
                            case 35:
                                goto label_31;
                            case 36:
                            case 47:
                                goto label_41;
                            case 37:
                                goto label_32;
                            case 38:
                                goto label_33;
                            case 39:
                                goto label_34;
                            case 40:
                                goto label_35;
                            case 41:
                                goto label_36;
                            case 42:
                            case 46:
                                goto label_40;
                            case 43:
                                goto label_37;
                            case 44:
                                goto label_38;
                            case 45:
                                goto label_39;
                            case 48:
                                goto label_42;
                            case 49:
                            case 53:
                                goto label_47;
                            case 50:
                                goto label_44;
                            case 51:
                                goto label_45;
                            case 52:
                                goto label_46;
                            case 54:
                                goto label_48;
                            case 55:
                                goto label_49;
                            case 56:
                                goto label_50;
                            case 57:
                                goto label_51;
                            case 58:
                            case 59:
                                goto label_60;
                            case 60:
                                goto label_52;
                            case 61:
                                goto label_53;
                            case 62:
                            case 66:
                            case 70:
                                goto label_58;
                            case 63:
                            case 64:
                                goto label_54;
                            case 65:
                                goto label_55;
                            case 67:
                            case 68:
                                goto label_56;
                            case 69:
                                goto label_57;
                            case 71:
                                goto label_59;
                            case 72:
                                goto label_61;
                            case 73:
                                goto label_62;
                            case 74:
                                goto label_63;
                            case 75:
                                goto label_70;
                        }
                }
            }
            catch (Exception ex) when (ex is Exception & (uint)num1 > 0U & num2 == 0)
            {
                ProjectData.SetProjectError(ex);
                goto label_65;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        label_70:
            Token token4 = token1;
            if (num2 == 0)
                return token4;
            ProjectData.ClearProjectError();
            return token4;
        }

        private void ConsumeBuffer(int CharCount)
        {
            if (CharCount > this.m_LookaheadBuffer.Length)
                return;
            int num = checked(CharCount - 1);
            int index = 0;
            while (index <= num)
            {
                switch ((char)((int)this.m_LookaheadBuffer[index] - 10))
                {
                    case char.MinValue:
                        checked { ++this.m_SysPosition.Line; }
                        this.m_SysPosition.Column = 0;
                        goto case '\x0003';
                    case '\x0003':
                        checked { ++index; }
                        continue;
                    default:
                        checked { ++this.m_SysPosition.Column; }
                        goto case '\x0003';
                }
            }
            this.m_LookaheadBuffer = this.m_LookaheadBuffer.Remove(0, CharCount);
        }

        private Token ProduceToken()
        {
            bool flag = false;
            Token token1 = (Token)null;
            while (!flag)
            {
                Token TheToken = this.LookaheadDFA();
                if (TheToken.Type() == SymbolType.GroupStart && (this.m_GroupStack.Count == 0 || this.m_GroupStack.Top().Group().Nesting.Contains((int)TheToken.Group().TableIndex)))
                {
                    this.ConsumeBuffer(Conversions.ToInteger(NewLateBinding.LateGet(TheToken.Data, (Type)null, "Length", new object[0], (string[])null, (Type[])null, (bool[])null)));
                    this.m_GroupStack.Push(ref TheToken);
                }
                else if (this.m_GroupStack.Count == 0)
                {
                    this.ConsumeBuffer(Conversions.ToInteger(NewLateBinding.LateGet(TheToken.Data, (Type)null, "Length", new object[0], (string[])null, (Type[])null, (bool[])null)));
                    token1 = TheToken;
                    flag = true;
                }
                else if (this.m_GroupStack.Top().Group().End == TheToken.Parent)
                {
                    Token token2 = this.m_GroupStack.Pop();
                    if (token2.Group().Ending == EndingMode.Closed)
                    {
                        Token token3 = token2;
                        token3.Data = Operators.ConcatenateObject(token3.Data, TheToken.Data);
                        this.ConsumeBuffer(Conversions.ToInteger(NewLateBinding.LateGet(TheToken.Data, (Type)null, "Length", new object[0], (string[])null, (Type[])null, (bool[])null)));
                    }
                    if (this.m_GroupStack.Count == 0)
                    {
                        token2.Parent = token2.Group().Container;
                        token1 = token2;
                        flag = true;
                    }
                    else
                    {
                        Token token3 = this.m_GroupStack.Top();
                        token3.Data = Operators.ConcatenateObject(token3.Data, token2.Data);
                    }
                }
                else if (TheToken.Type() == SymbolType.End)
                {
                    token1 = TheToken;
                    flag = true;
                }
                else
                {
                    Token token2 = this.m_GroupStack.Top();
                    if (token2.Group().Advance == AdvanceMode.Token)
                    {
                        Token token3 = token2;
                        token3.Data = Operators.ConcatenateObject(token3.Data, TheToken.Data);
                        this.ConsumeBuffer(Conversions.ToInteger(NewLateBinding.LateGet(TheToken.Data, (Type)null, "Length", new object[0], (string[])null, (Type[])null, (bool[])null)));
                    }
                    else
                    {
                        Token token3 = token2;
                        token3.Data = Operators.ConcatenateObject(token3.Data, NewLateBinding.LateGet(TheToken.Data, (Type)null, "Chars", new object[1]
                        {
            (object) 0
                        }, (string[])null, (Type[])null, (bool[])null));
                        this.ConsumeBuffer(1);
                    }
                }
            }
            return token1;
        }

        public ParseMessage Parse()
        {
            if (!this.m_Tables.IsLoaded())
                return ParseMessage.NotLoadedError;
            bool flag = false;
            ParseMessage parseMessage;
            while (!flag)
            {
                if (this.m_InputTokens.Count == 0)
                {
                    this.m_InputTokens.Push(this.ProduceToken());
                    parseMessage = ParseMessage.TokenRead;
                    flag = true;
                }
                else
                {
                    Token NextToken = this.m_InputTokens.Top();
                    this.m_CurrentPosition.Copy(NextToken.Position);
                    if (this.m_GroupStack.Count != 0)
                    {
                        parseMessage = ParseMessage.GroupError;
                        flag = true;
                    }
                    else if (NextToken.Type() == SymbolType.Noise)
                        this.m_InputTokens.Pop();
                    else if (NextToken.Type() == SymbolType.Error)
                    {
                        parseMessage = ParseMessage.LexicalError;
                        flag = true;
                    }
                    else
                    {
                        switch (this.ParseLALR(ref NextToken))
                        {
                            case Parser.ParseResult.Accept:
                                parseMessage = ParseMessage.Accept;
                                flag = true;
                                break;
                            case Parser.ParseResult.Shift:
                                this.m_InputTokens.Dequeue();
                                parseMessage = ParseMessage.Shift;
                                flag = true;
                                break;
                            case Parser.ParseResult.ReduceNormal:
                                parseMessage = ParseMessage.Reduction;
                                flag = true;
                                break;
                            case Parser.ParseResult.SyntaxError:
                                parseMessage = ParseMessage.SyntaxError;
                                flag = true;
                                break;
                            case Parser.ParseResult.InternalError:
                                parseMessage = ParseMessage.InternalError;
                                flag = true;
                                break;
                        }
                    }
                }
            }
            return parseMessage;
        }


        private enum ParseResult
        {
            Accept = 1,
            Shift = 2,
            ReduceNormal = 3,
            ReduceEliminated = 4,
            SyntaxError = 5,
            InternalError = 6,
        }
    }
}