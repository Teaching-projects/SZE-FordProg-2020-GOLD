using System;

namespace GOLD.Builder
{
    internal class LookaheadSymbol : DictionarySet.IMember
    {
        public SymbolBuild Parent;
        public ConfigTrackSet Configs;

        public LookaheadSymbol(SymbolBuild Sym)
        {
            this.Parent = Sym;
            this.Configs = new ConfigTrackSet();
        }

        public LookaheadSymbol(LookaheadSymbol Look)
        {
            this.Parent = Look.Parent;
            this.Configs = new ConfigTrackSet();
            this.Configs.Copy((DictionarySet)Look.Configs);
        }

        public LookaheadSymbol()
        {
            this.Configs = new ConfigTrackSet();
        }

        IComparable DictionarySet.IMember.Key()
        {
            return (IComparable)this.Parent.TableIndex;
        }

        DictionarySet.MemberResult DictionarySet.IMember.Union(
          DictionarySet.IMember Obj)
        {
            LookaheadSymbol lookaheadSymbol = (LookaheadSymbol)Obj;
            return new DictionarySet.MemberResult((DictionarySet.IMember)new LookaheadSymbol()
            {
                Parent = this.Parent,
                Configs = new ConfigTrackSet(this.Configs, lookaheadSymbol.Configs)
            });
        }

        public DictionarySet.MemberResult Difference(DictionarySet.IMember NewObject)
        {
            return (DictionarySet.MemberResult)null;
        }

        public DictionarySet.MemberResult Intersect(DictionarySet.IMember NewObject)
        {
            return new DictionarySet.MemberResult((DictionarySet.IMember)this);
        }
    }
}