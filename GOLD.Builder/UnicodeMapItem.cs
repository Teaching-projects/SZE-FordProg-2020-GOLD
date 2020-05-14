using Microsoft.VisualBasic.CompilerServices;
using System;

namespace GOLD.Builder
{
    internal class UnicodeMapItem : DictionarySet.IMember
    {
        public int Code;
        public int Map;

        public UnicodeMapItem()
        {
            this.Code = -1;
            this.Map = -1;
        }

        public UnicodeMapItem(int Code, int Map)
        {
            this.Code = Code;
            this.Map = Map;
        }

        public UnicodeMapItem(int Code)
        {
            this.Code = Code;
            this.Map = -1;
        }

        public IComparable Key()
        {
            return (IComparable)this.Code;
        }

        public DictionarySet.MemberResult Union(DictionarySet.IMember NewObject)
        {
            return new DictionarySet.MemberResult((DictionarySet.IMember)this);
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