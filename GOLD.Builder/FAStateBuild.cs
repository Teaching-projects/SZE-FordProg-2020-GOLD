namespace GOLD.Builder
{
    internal class FAStateBuild : FAState
    {
        public NumberSet NFAStates;
        public NumberSet NFAClosure;
        public FAAcceptList AcceptList;
        public NumberSet PriorStates;
        public short TableIndex;

        public FAStateBuild(SymbolBuild Accept)
          : base((Symbol)Accept)
        {
            this.NFAStates = new NumberSet(new int[0]);
            this.AcceptList = new FAAcceptList();
            this.PriorStates = new NumberSet(new int[0]);
            this.EdgeList = (FAEdgeList)new FAEdgeBuildList();
        }

        public FAStateBuild()
        {
            this.NFAStates = new NumberSet(new int[0]);
            this.AcceptList = new FAAcceptList();
            this.PriorStates = new NumberSet(new int[0]);
            this.EdgeList = (FAEdgeList)new FAEdgeBuildList();
        }

        public void CaseClosure()
        {
            int num1 = checked(base.Edges().Count() - 1);
            int index1 = 0;
            while (index1 <= num1)
            {
                NumberSet SetB = new NumberSet(new int[0]);
                CharacterSet characters = base.Edges()[index1].Characters;
                int num2 = checked(characters.Count() - 1);
                int index2 = 0;
                while (index2 <= num2)
                {
                    int CharCode = characters[index2];
                    int lowerCase = UnicodeTable.ToLowerCase(ref CharCode);
                    if (CharCode != lowerCase)
                        SetB.Add(lowerCase);
                    int upperCase = UnicodeTable.ToUpperCase(ref CharCode);
                    if (CharCode != upperCase)
                        SetB.Add(upperCase);
                    checked { ++index2; }
                }
                characters.UnionWith(SetB);
                checked { ++index1; }
            }
        }

        public void MappingClosure(CharMappingMode Mapping)
        {
            int num1 = checked(base.Edges().Count() - 1);
            int index1 = 0;
            while (index1 <= num1)
            {
                NumberSet SetB = new NumberSet(new int[0]);
                CharacterSet characters = base.Edges()[index1].Characters;
                int num2 = checked(characters.Count() - 1);
                int index2 = 0;
                while (index2 <= num2)
                {
                    int CharCode = characters[index2];
                    int win1252 = UnicodeTable.ToWin1252(ref CharCode);
                    if (CharCode != win1252)
                        SetB.Add(win1252);
                    checked { ++index2; }
                }
                characters.UnionWith(SetB);
                checked { ++index1; }
            }
        }

        public void AddLambdaEdge(int Target)
        {
            this.AddEdge((FAEdge)new FAEdgeBuild(new CharacterSetBuild(), Target));
        }

        public void AddEdge(FAEdgeBuild Edge)
        {
            this.AddEdge((FAEdge)Edge);
        }

        public void AddEdge(CharacterSetBuild CharSet, int Target)
        {
            this.AddEdge((FAEdge)new FAEdgeBuild(CharSet, Target));
        }

        public FAEdgeBuildList Edges()
        {
            return (FAEdgeBuildList)base.Edges();
        }
    }
}