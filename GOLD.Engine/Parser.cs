using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace GOLD.Engine
{
    public class Parser
    {
        private const string version = "5.0";
        private FAStateList dfa;
        private CharacterSetList charSetTable;
        private string lookaheadBuffer;
        private LRStateList lrStates;
        private int currentLALR;
        public Stack<Token> stack;
        private bool haveReduction;
        private Queue<Token> inputTokens;
        private TextReader source;
        private Position sysPosition;
        private Stack<Token> groupStack;
        private GroupList groupTable;

        [Description("Determines if reductions will be trimmed in cases where a production contains a single element.")]
        public bool TrimReductions { get; set; }
        [Description("Returns information about the current grammar.")]
        public GrammarProperties Grammar { get; private set; }
        [Description("Current line and column being read from the source.")]
        public Position CurrentPosition { get; }
        [Description("Returns a list of Symbols recognized by the grammar.")]
        public SymbolList SymbolTable { get; private set; }
        [Description("Returns a list of Productions recognized by the grammar.")]
        public ProductionList ProductionTable { get; private set; }
        [Description("If the Parse() method returns a SyntaxError, this method will contain a list of the symbols the grammar expected to see.")]
        public SymbolList ExpectedSymbols { get; }
        [Description("Returns true if parse tables were loaded.")]
        public bool IsTablesLoaded { get; private set; }
        [Description("When the Parse() method returns a Reduce, this method will contain the current Reduction.")]
        public object CurrentReduction
        {
            get
            {
                return !haveReduction ? null : stack.Peek().Data;
            }
            set
            {
                if (haveReduction)
                    stack.Peek().Data = (string)value;
            }
        }
        [Description("If the Parse() function returns TokenRead, this method will return that last read token.")]
        public Token CurrentToken
        {
            get
            {
                return inputTokens.Peek();
            }
        }

        public Parser()
        {
            SymbolTable = new SymbolList();
            dfa = new FAStateList();
            charSetTable = new CharacterSetList();
            ProductionTable = new ProductionList();
            lrStates = new LRStateList();
            stack = new Stack<Token>();
            ExpectedSymbols = new SymbolList();
            inputTokens = new Queue<Token>();
            sysPosition = new Position();
            CurrentPosition = new Position();
            Grammar = new GrammarProperties();
            groupStack = new Stack<Token>();
            groupTable = new GroupList();
            Restart();
            IsTablesLoaded = false;
            TrimReductions = false;
        }

        [Description("Opens a string for parsing.")]
        public bool Open(ref string text)
        {
            return Open(new StringReader(text));
        }

        [Description("Opens a text stream for parsing.")]
        public bool Open(TextReader reader)
        {
            Token token = new Token();
            Restart();
            source = reader;
            token.State = lrStates.InitialState;
            stack.Push(token);
            return true;
        }

        [Description("Removes the next token from the input queue.")]
        public Token DiscardCurrentToken()
        {
            return inputTokens.Dequeue();
        }

        [Description("Added a token onto the end of the input queue.")]
        public void EnqueueInput(Token token)
        {
            inputTokens.Enqueue(token);
        }

        [Description("Pushes the token onto the top of the input queue. This token will be analyzed next.")]
        public void PushInput(Token token)
        {
            var l = inputTokens.ToList();
            l.Insert(0, token);
            inputTokens = new Queue<Token>(l);
        }

        // Return Count characters from the lookahead buffer. DO NOT CONSUME
        // This is used to create the text stored in a token. It is disgarded
        // separately. Because of the design of the DFA algorithm, count should
        // never exceed the buffer length. The If-Statement below is fault-tolerate
        // programming, but not necessary.
        private string LookaheadBuffer(int count)
        {
            if (count > lookaheadBuffer.Length)
                count = Convert.ToInt32(lookaheadBuffer);

            return lookaheadBuffer.Substring(0, count);
        }

        // Return single char at the index. This function will also increase 
        // buffer if the specified character is not present. It is used 
        // by the DFA algorithm.
        private string Lookahead(int charIndex)
        {
            // Check if we must read characters from the Stream
            if (charIndex > lookaheadBuffer.Length)
            {
                int readCount = charIndex - lookaheadBuffer.Length;
                for (int i = 0; i < readCount; i++)
                    lookaheadBuffer += (char)source.Read();
            }

            // If the buffer is still smaller than the index, we have reached
            // the end of the text. In this case, return a null string - the DFA
            // code will understand.
            if (charIndex <= lookaheadBuffer.Length)
                return lookaheadBuffer[charIndex-1].ToString();

            return string.Empty;
        }

        [Description("Library name and version.")]
        public string About()
        {
            return "GOLD Parser Engine; Version " + version;
        }

        internal void Clear()
        {
            SymbolTable.Clear();
            ProductionTable.Clear();
            charSetTable.Clear();
            dfa.Clear();
            lrStates.Clear();
            stack.Clear();
            inputTokens.Clear();
            Grammar = new GrammarProperties();
            groupStack.Clear();
            groupTable.Clear();
            Restart();
        }

        [Description("Loads parse tables from the specified filename. Only EGT (version 5.0) is supported.")]
        public bool LoadTables(string path)
        {
            return LoadTables(new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)));
        }

        [Description("Loads parse tables from the specified BinaryReader. Only EGT (version 5.0) is supported.")]
        public bool LoadTables(BinaryReader reader)
        {
            EGTReader egt = new EGTReader();
            bool success;
            try
            {
                egt.Open(reader);
                Restart();
                success = true;
                while (!egt.IsEndOfFile || !success)
                {
                    egt.GetNextRecord();
                    EGTRecord recType = (EGTRecord)egt.RetrieveByte();

                    switch (recType)
                    {
                        case EGTRecord.DFAState:
                            int index = egt.RetrieveInt16();
                            bool accept = egt.RetrieveBoolean();
                            int acceptIndex = egt.RetrieveInt16();
                            egt.RetrieveEntry(); // Reserved
                            dfa[index] = !accept ? new FAState() : new FAState(SymbolTable[acceptIndex]);
                            while (!egt.IsReadComplete)
                            {
                                int setIndex = egt.RetrieveInt16();
                                int target = egt.RetrieveInt16();
                                egt.RetrieveEntry(); // Reserved
                                dfa[index].Edges.Add(new FAEdge(this.charSetTable[setIndex], target));
                            }
                            break;

                        case EGTRecord.InitialStates:
                            dfa.InitialState = egt.RetrieveInt16();
                            lrStates.InitialState = egt.RetrieveInt16();
                            break;

                        case EGTRecord.LRState:
                            index = egt.RetrieveInt16();
                            egt.RetrieveEntry();
                            lrStates[index] = new LRState();
                            while (!egt.IsReadComplete)
                            {
                                int symIndex = egt.RetrieveInt16();
                                int action = egt.RetrieveInt16();
                                int target = egt.RetrieveInt16();
                                egt.RetrieveEntry();
                                lrStates[index].Add(new LRAction(SymbolTable[symIndex], (LRActionType)action, (short)target));
                            }
                            break;

                        case EGTRecord.Production:
                            index = egt.RetrieveInt16();
                            int headIndex = egt.RetrieveInt16();
                            egt.RetrieveEntry();
                            ProductionTable[index] = new Production(SymbolTable[headIndex], (short)index);
                            while (!egt.IsReadComplete)
                            {
                                int symIndex = egt.RetrieveInt16();
                                ProductionTable[index].Handle.Add(SymbolTable[symIndex]);
                            }
                            break;

                        case EGTRecord.Symbol:
                            index = egt.RetrieveInt16();
                            string name = egt.RetrieveString();
                            SymbolType type = (SymbolType)egt.RetrieveInt16();
                            SymbolTable[index] = new Symbol(name, type, (short)index);
                            break;

                        case EGTRecord.CharRanges:
                            index = egt.RetrieveInt16();
                            egt.RetrieveInt16(); // Codepage
                            int total = egt.RetrieveInt16();
                            egt.RetrieveEntry(); // Reserveds
                            charSetTable[index] = new CharacterSet();
                            while (!egt.IsReadComplete)
                            {
                                charSetTable[index].Add(new CharacterRange((ushort)egt.RetrieveInt16(), (ushort)egt.RetrieveInt16()));
                            }
                            break;

                        case EGTRecord.Group:
                            Group g = new Group();
                            index = egt.RetrieveInt16();
                            g.Name = egt.RetrieveString();
                            g.Container = SymbolTable[egt.RetrieveInt16()];
                            g.Start = SymbolTable[egt.RetrieveInt16()];
                            g.End = SymbolTable[egt.RetrieveInt16()];
                            g.Advance = (Group.AdvanceMode)egt.RetrieveInt16();
                            g.Ending = (Group.EndingMode)egt.RetrieveInt16();
                            egt.RetrieveEntry();

                            int count = egt.RetrieveInt16();
                            for (int i = 0; i < count; i++)
                                g.Nesting.Add(egt.RetrieveInt16());

                            g.Container.Group = g;
                            g.Start.Group = g;
                            g.End.Group = g;
                            groupTable[index] = g;
                            break;

                        case EGTRecord.Property:
                            index = egt.RetrieveInt16();
                            name = egt.RetrieveString(); // Just discard
                            Grammar.SetValue(index, egt.RetrieveString());
                            break;

                        case EGTRecord.TableCounts:
                            SymbolTable = new SymbolList(egt.RetrieveInt16());
                            charSetTable = new CharacterSetList(egt.RetrieveInt16());
                            ProductionTable = new ProductionList(egt.RetrieveInt16());
                            dfa = new FAStateList(egt.RetrieveInt16());
                            lrStates = new LRStateList(egt.RetrieveInt16());
                            groupTable = new GroupList(egt.RetrieveInt16());
                            break;

                        default:
                            throw new ParserException("File Error. A record of type '" + (char)recType + "' was read. This is not a valid code.");
                    }
                }

                egt.Dispose();
            }
            catch (Exception ex)
            {
                throw new ParserException(ex.Message, ex, nameof(LoadTables));
            }
            IsTablesLoaded = success;
            return success;
        }


        // This function analyzes a token and either:
        // 1. Makes a SINGLE reduction and pushes a complete Reduction object on the m_Stack
        // 2. Accepts the token and shifts
        // 3. Errors and places the expected symbol indexes in the Tokens list
        // The Token is assumed to be valid and WILL be checked
        // If an action is performed that requires controlt to be returned to the user, the function returns true.
        // The Message parameter is then set to the type of action.
        private ParseResult ParseLALR(ref Token token)
        {
            LRAction parseAction = lrStates[currentLALR][token.Parent];
            ParseResult parseResult = 0;
            if (parseAction != null)
            {
                haveReduction = false;
                switch (parseAction.Type)
                {
                    case LRActionType.Shift:
                        currentLALR = parseAction.Value;
                        token.State =(short)currentLALR;
                        stack.Push(token);
                        parseResult = ParseResult.Shift;
                        break;

                    case LRActionType.Reduce:
                        Production production = ProductionTable[parseAction.Value];
                        Token head;

                        // The current rule only consists of a single nonterminal and can be trimmed from the
                        // parse tree. Usually we create a new Reduction, assign it to the Data property
                        // of Head and push it on the m_Stack. However, in this case, the Data property of the
                        // Head will be assigned the Data property of the reduced token (i.e. the only one
                        // on the m_Stack).
                        // In this case, to save code, the value popped of the m_Stack is changed into the head.
                        if (TrimReductions && production.ContainsOneNonTerminal())
                        {
                            head = stack.Pop();
                            head.Parent = production.Head;
                            parseResult = ParseResult.ReduceEliminated;
                        }
                        else
                        {
                            haveReduction = true;
                            Reduction newReduction = new Reduction(production.Handle.Count())
                            {
                                Parent = production
                            };
                            short n = (short)(production.Handle.Count() - 1);
                            while (n >= 0)
                            {
                                newReduction[n] = stack.Pop();
                                n--;
                            }
                            head = new Token(production.Head, newReduction);
                            parseResult = ParseResult.ReduceNormal;
                        }

                        short index = stack.Peek().State;
                        short n2 = lrStates[index].IndexOf(production.Head);
                        if (n2 != -1)
                        {
                            currentLALR = lrStates[index][n2].Value;
                            head.State = (short)currentLALR;
                            stack.Push(head);
                            break;
                        }
                        parseResult = ParseResult.InternalError;
                        break;

                    case LRActionType.Accept:
                        haveReduction = true;
                        parseResult = ParseResult.Accept;
                        break;
                }
            }
            else
            {
                ExpectedSymbols.Clear();
                foreach (LRAction LRAction in lrStates[currentLALR])
                {
                    switch (LRAction.Symbol.Type)
                    {
                        case SymbolType.Content:
                        case SymbolType.End:
                        case SymbolType.GroupStart:
                        case SymbolType.GroupEnd:
                            ExpectedSymbols.Add(LRAction.Symbol);
                            break;
                    }
                }
                parseResult = ParseResult.SyntaxError;
            }
            return parseResult;
        }

        [Description("Restarts the parser. Loaded tables are retained.")]
        public void Restart()
        {
            currentLALR = lrStates.InitialState;
            sysPosition.Column = 0;
            sysPosition.Line = 0;
            CurrentPosition.Line = 0;
            CurrentPosition.Column = 0;
            haveReduction = false;
            ExpectedSymbols.Clear();
            inputTokens.Clear();
            stack.Clear();
            lookaheadBuffer = "";
            groupStack.Clear();
        }

        // This function implements the DFA for th parser's lexer.
        // It generates a token which is used by the LALR state
        // machine.
        private Token LookaheadDFA()
        {
            Token result = new Token();
            bool done = false;
            int currentDFA = dfa.InitialState;
            int currentPosition = 1;
            int lastAcceptState = -1;
            int lastAcceptPosition = -1;
            string ch = Lookahead(1);

            if (!string.IsNullOrEmpty(ch) && char.ConvertToUtf32(ch, 0) != ushort.MaxValue)
            {
                // This code searches all the branches of the current DFA state
                // for the next character in the input Stream. If found the
                // target state is returned.
                while (!done)
                {
                    string ch2 = Lookahead(currentPosition);
                    bool found;
                    int target = 0;
                    if (string.IsNullOrEmpty(ch2))
                    {
                        found = false;
                    }
                    else
                    {
                        found = false;
                        for (int i = 0; i < dfa[currentDFA].Edges.Count; i++)
                        {
                            FAEdge edge = dfa[currentDFA].Edges[i];
                            if (edge.Characters.Contains(char.ConvertToUtf32(ch2, 0)))
                            {
                                found = true;
                                target = edge.Target;
                                break;
                            }
                        }
                    }

                    // This block-if statement checks whether an edge was found from the current state. If so, the state and current
                    // position advance. Otherwise it is time to exit the main loop and report the token found (if there was one). 
                    // If the LastAcceptState is -1, then we never found a match and the Error Token is created. Otherwise, a new 
                    // token is created using the Symbol in the Accept State and all the characters that comprise it.
                    if (found)
                    {
                        // This code checks whether the target state accepts a token.
                        // If so, it sets the appropiate variables so when the
                        // algorithm in done, it can return the proper token and
                        // number of characters.
                        if (dfa[target].Accept != null)
                        {
                            lastAcceptState = target;
                            lastAcceptPosition = currentPosition;
                        }
                        currentDFA = target;
                        currentPosition++;
                    }
                    else
                    {
                        done = true;
                        if (lastAcceptState == -1)
                        {
                            result.Parent = SymbolTable.GetFirstOrDefaultType(SymbolType.Error);
                            result.Data = LookaheadBuffer(1);
                        }
                        else
                        {
                            result.Parent = dfa[lastAcceptState].Accept;
                            result.Data = LookaheadBuffer(lastAcceptPosition);
                        }
                    }
                }
            }
            else
            {
                result.Data = "";
                result.Parent = SymbolTable.GetFirstOrDefaultType(SymbolType.End);
            }
            result.Position.Copy(sysPosition);
            return result;
        }

        // Consume/Remove the characters from the front of the buffer. 
        private void ConsumeBuffer(int charCount)
        {
            if (charCount > lookaheadBuffer.Length)
                return;

            // Count Carriage Returns and increment the internal column and line
            // numbers. This is done for the Developer and is not necessary for the
            // DFA algorithm.
            for (int i = 0; i < charCount; i++)
            {
                switch (lookaheadBuffer[i])
                {
                    case '\n':
                        sysPosition.Line++;
                        sysPosition.Column = 0;
                        break;
                    case '\r':
                        break;
                    default:
                        sysPosition.Column++;
                        break;
                }
            }
            lookaheadBuffer = lookaheadBuffer.Remove(0, charCount);
        }

        // This function creates a token and also takes into account the current
        // lexing mode of the parser. In particular, it contains the group logic. 
        // A stack is used to track the current "group". This replaces the comment
        // level counter. Also, text is appended to the token on the top of the 
        // stack. This allows the group text to returned in one chunk.
        private Token ProduceToken()
        {
            bool done = false;
            Token result = null;
            while (!done)
            {
                Token read = LookaheadDFA();

                // The logic - to determine if a group should be nested - requires that the top of the stack 
                // and the symbol's linked group need to be looked at. Both of these can be unset. So, this section
                // sets a Boolean and avoids errors. We will use this boolean in the logic chain below. 
                if (read.Type == SymbolType.GroupStart && (groupStack.Count == 0 || groupStack.Peek().Group.Nesting.Contains(read.Group.TableIndex)))
                {
                    ConsumeBuffer(((string)read.Data).Length);
                    groupStack.Push(read);
                }
                else if (groupStack.Count == 0) // The token is ready to be analyzed.  
                {
                    ConsumeBuffer(((string)read.Data).Length);
                    result = read;
                    done = true;
                }
                else if (groupStack.Peek().Group.End == read.Parent)
                {
                    // End the current group
                    Token pop = groupStack.Pop();
                    if (pop.Group.Ending == Group.EndingMode.Closed)
                    {
                        pop.Data = (string)pop.Data + (string)read.Data; // Append text
                        ConsumeBuffer(((string)read.Data).Length); // Consume token
                    }
                    if (groupStack.Count == 0) // We are out of the group. Return pop'd token (which contains all the group text)
                    {
                        pop.Parent = pop.Group.Container; // Change symbol to parent
                        result = pop;
                        done = true;
                    }
                    else
                    {
                        groupStack.Peek().Data = (string)groupStack.Peek().Data + (string)pop.Data; // Append group text to parent
                    }
                }
                else if (read.Type == SymbolType.End)
                {
                    // EOF always stops the loop. The caller function (Parse) can flag a runaway group error.
                    result = read;
                    done = true;
                }
                else
                {
                    Token top = groupStack.Peek();
                    if (top.Group.Advance == Group.AdvanceMode.Token)
                    {
                        top.Data = (string)top.Data + (string)read.Data; // Append all text
                        ConsumeBuffer(((string)read.Data).Length);
                    }
                    else
                    {
                        top.Data = (string)top.Data + ((string)read.Data)[0]; //  Append one character
                        ConsumeBuffer(1);
                    }
                }
            }
            return result;
        }

        [Description("Performs a parse action on the input. This method is typically used in a loop until either grammar is accepted or an error occurs.")]
        public ParseMessage Parse(bool debug = false)
        {
            if (!IsTablesLoaded)
            {
                if(debug)
                    Console.WriteLine("\tThis error occurs if the EGT was not loaded.");

                return ParseMessage.NotLoadedError;
            }

            bool done = false;
            ParseMessage result = 0;
            while (!done)
            {
                if (inputTokens.Count == 0)
                {
                    PushInput(ProduceToken());

                    if (debug)
                        Console.WriteLine("\tSymbol read: {0};\tToken created: {1};\tOn stack: {2}", (string)CurrentToken.Data, CurrentToken.Parent.Name, GetStackState());

                    result = ParseMessage.TokenRead;
                    done = true;
                }
                else
                {
                    Token read = inputTokens.Peek();
                    CurrentPosition.Copy(read.Position); // Update current position
                    if (groupStack.Count != 0) // Runaway group
                    {
                        if(debug)
                            Console.WriteLine("\tGROUP ERROR! Unexpected end of file.");

                        result = ParseMessage.GroupError;
                        done = true;
                    }
                    else if (read.Type == SymbolType.Noise) // Just discard. These were already reported to the user.
                    {
                        if (debug)
                            Console.WriteLine("\tNoise left.");

                        inputTokens.Dequeue();
                    }
                    else if (read.Type == SymbolType.Error)
                    {
                        if (debug)
                            Console.WriteLine("\tCannot recognize token: " + CurrentToken.Data);

                        result = ParseMessage.LexicalError;
                        done = true;
                    }
                    else // Finally, we can parse the token.
                    {
                        switch (ParseLALR(ref read))
                        {
                            case ParseResult.Accept:
                                if(debug)
                                    Console.WriteLine("\tAccepted!");

                                result = ParseMessage.Accept;
                                done = true;
                                break;
                            case ParseResult.Shift:
                                // ParseToken() shifted the token on the front of the Token-Queue. 
                                // It now exists on the Token-Stack and must be eliminated from the queue.
                                if(debug)
                                    Console.WriteLine("\tShift token: {0};\tOn stack: {1}", CurrentToken.Parent.Name, GetStackState());

                                inputTokens.Dequeue();
                                break;
                            case ParseResult.ReduceNormal:

                                if (debug)
                                    Console.WriteLine("\tLookahead: '{0}';\tReduce: '{1}';\tOn stack: {2}", CurrentToken.Parent.Name, ((Reduction)CurrentReduction).Parent.Head.Name, GetStackState());

                                result = ParseMessage.Reduction;
                                done = true;
                                break;
                            case ParseResult.SyntaxError:
                                if(debug)
                                    Console.WriteLine("\tExpecting a different token");

                                result = ParseMessage.SyntaxError;
                                done = true;
                                break;
                            case ParseResult.InternalError:
                                if(debug)
                                    Console.WriteLine("\tINTERNAL ERROR! Something is horribly wrong.");

                                result = ParseMessage.InternalError;
                                done = true;
                                break;
                        }
                    }
                }
            }
            return result;
        }

        private string GetStackState()
        {
            var x = stack.ToList();
            x.RemoveAt(x.Count - 1);
            x.Reverse();
            string onStack = string.Empty;
            for (int i = 0; i < x.Count; i++)
            {
                onStack += "'" + x[i].Parent.Name + "' ";
            }

            return onStack;
        }

        private enum ParseResult
        {
            Accept = 1,
            Shift = 2,
            ReduceNormal = 3,
            ReduceEliminated = 4,   // Trim
            SyntaxError = 5,
            InternalError = 6,
        }
    }
}
