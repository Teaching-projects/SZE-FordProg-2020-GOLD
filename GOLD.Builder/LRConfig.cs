using System;
namespace GOLD.Builder
{

    internal class LRConfig : IComparable, DictionarySet.IMember
    {
        public short Position;
        public LookaheadSymbolSet LookaheadSet;
        public bool Modified;
        public bool InheritLookahead;
        public ProductionBuild Parent;
        public LRStatus Status;

        public LRConfig(ProductionBuild Parent, int Position, LookaheadSymbolSet InitSet)
        {
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Parent = Parent;
            this.Position = checked((short)Position);
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Modified = true;
            this.InheritLookahead = false;
            int num = checked(InitSet.Count() - 1);
            int index = 0;
            while (index <= num)
            {
                this.LookaheadSet.Add(new LookaheadSymbol(InitSet[index]));
                checked { ++index; }
            }
        }

        public LRConfig(LRConfig Config)
        {
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Parent = Config.Parent;
            this.Position = Config.Position;
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Modified = Config.Modified;
            this.InheritLookahead = Config.InheritLookahead;
            this.LookaheadSet.Copy((DictionarySet)Config.LookaheadSet);
        }

        public LRConfig(ProductionBuild Rule)
        {
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Parent = Rule;
            this.Position = (short)0;
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Modified = true;
            this.InheritLookahead = false;
        }

        public LRConfig()
        {
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Parent = (ProductionBuild)null;
            this.Position = (short)0;
            this.LookaheadSet = new LookaheadSymbolSet();
            this.Modified = true;
            this.InheritLookahead = false;
        }

        public bool IsComplete()
        {
            return (int)this.Position > checked(this.Parent.Handle().Count() - 1);
        }

        public LRActionType NextAction()
        {
            return (int)this.Position <= checked(this.Parent.Handle().Count() - 1) ? (this.NextSymbol(0).Category() != SymbolCategory.Terminal ? LRActionType.Goto : LRActionType.Shift) : LRActionType.Reduce;
        }

        public int TableIndex()
        {
            return (int)this.Parent.TableIndex;
        }

        public bool IsEqualTo(ref LRConfig Config)
        {
            return (int)this.Parent.TableIndex == Config.TableIndex() & (int)this.Position == (int)Config.Position;
        }

        public bool IsLessThan(ref LRConfig ConfigB)
        {
            return (int)this.Position == (int)ConfigB.Position ? (int)this.Parent.TableIndex != ConfigB.TableIndex() && (int)this.Parent.TableIndex < ConfigB.TableIndex() : (int)this.Position > (int)ConfigB.Position;
        }

        public bool IsGreaterThan(ref LRConfig ConfigB)
        {
            LRConfig lrConfig1 = ConfigB;
            LRConfig lrConfig2 = this;
            ref LRConfig local = ref lrConfig2;
            return lrConfig1.IsLessThan(ref local);
        }

        public short CheckaheadCount()
        {
            return checked((short)(this.Parent.Handle().Count() - (int)this.Position - 1));
        }

        public LRConfigCompare CompareCore(ref LRConfig ConfigB)
        {
            LRConfigCompare lrConfigCompare = 0;
            if ((int)this.Parent.TableIndex == ConfigB.TableIndex() & (int)this.Position == (int)ConfigB.Position)
            {
                switch (this.LookaheadSet.CompareTo((DictionarySet)ConfigB.LookaheadSet))
                {
                    case DictionarySet.Compare.Equal:
                        lrConfigCompare = LRConfigCompare.EqualFull;
                        break;
                    case DictionarySet.Compare.UnEqual:
                        lrConfigCompare = LRConfigCompare.EqualCore;
                        break;
                    case DictionarySet.Compare.Subset:
                        lrConfigCompare = LRConfigCompare.ProperSubset;
                        break;
                }
            }
            else
                lrConfigCompare = LRConfigCompare.UnEqual;
            return lrConfigCompare;
        }

        public bool HasEqualCore(ref LRConfig Config)
        {
            return this.IsEqualTo(ref Config);
        }

        public SymbolBuild NextSymbol(int Offset = 0)
        {
            return this.Parent.Handle()[checked((int)this.Position + Offset)];
        }

        public SymbolBuild Checkahead(short Offset = 0)
        {
            if ((int)this.Position <= checked(this.Parent.Handle().Count() - 1 - (int)Offset))
                return this.Parent.Handle()[checked((int)this.Position + 1 + (int)Offset)];
            return (SymbolBuild)null;
        }

        public string Text(string Marker = "^")
        {
            string str = "<" + this.Parent.Head.Name + "> ::=";
            short num1 = checked((short)(this.Parent.Handle().Count() - 1));
            short num2 = 0;
            while ((int)num2 <= (int)num1)
            {
                if ((int)num2 == (int)this.Position)
                    str = str + " " + Marker;
                str = str + " " + this.Parent.Handle()[(int)num2].Text(false);
                checked { ++num2; }
            }
            if ((int)this.Position > checked(this.Parent.Handle().Count() - 1))
                str = str + " " + Marker;
            return str;
        }

        public int CompareTo(object obj)
        {
            LRConfig lrConfig = (LRConfig)obj;
            if (this.IsEqualTo(ref lrConfig))
                return 0;
            return this.IsLessThan(ref lrConfig) ? -1 : 1;
        }

        public DictionarySet.MemberResult Difference(DictionarySet.IMember NewObject)
        {
            return (DictionarySet.MemberResult)null;
        }

        public DictionarySet.MemberResult Intersect(DictionarySet.IMember NewObject)
        {
            return (DictionarySet.MemberResult)null;
        }

        public IComparable Key()
        {
            return (IComparable)this;
        }

        DictionarySet.MemberResult DictionarySet.IMember.Union(
          DictionarySet.IMember NewObject)
        {
            LRConfig lrConfig1 = (LRConfig)NewObject;
            LRConfig lrConfig2 = new LRConfig(this);
            bool SetChanged;
            if (lrConfig2.LookaheadSet.UnionWith(lrConfig1.LookaheadSet))
            {
                SetChanged = true;
                lrConfig2.Modified = true;
            }
            else
            {
                lrConfig2.Modified = this.Modified | lrConfig1.Modified;
                SetChanged = false;
            }
            lrConfig2.InheritLookahead = this.InheritLookahead | lrConfig1.InheritLookahead;
            return new DictionarySet.MemberResult((DictionarySet.IMember)lrConfig2, SetChanged);
        }
    }
}