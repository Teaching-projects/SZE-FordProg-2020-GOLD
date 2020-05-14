namespace GOLD.Builder
{
    internal class LRConfigSet : DictionarySet
    {
        public bool Add(LRConfig Item)
        {
            return Add((DictionarySet.IMember)Item);
        }

        public LRConfig this[int Index]
        {
            get
            {
                return (LRConfig)base[Index];
            }
        }

        public bool UnionWith(LRConfigSet SetB)
        {
            return this.UnionWith((DictionarySet)SetB);
        }

        public LRConfigSet Union(LRConfigSet SetB)
        {
            return this.Union(SetB);
        }

        public bool EqualCore(ref LRConfigSet ConfigSetB)
        {
            return this.IsEqualTo(ref ConfigSetB);
        }

        public bool IsEqualTo(ref LRConfigSet ConfigSetB)
        {
            bool flag;
            if (this.Count() == ConfigSetB.Count())
            {
                short num = 0;
                flag = false;
                while ((int)num < this.Count() & !flag)
                {
                    LRConfig lrConfig = (LRConfig)base[(int)num];
                    LRConfig Config = ConfigSetB[(int)num];
                    if (!lrConfig.IsEqualTo(ref Config))
                        flag = true;
                    checked { ++num; }
                }
            }
            else
                flag = true;
            return !flag;
        }

        public bool IsLessThan(ref LRConfigSet ConfigSetB)
        {
            bool flag1 = false;
            short num1 = 0;
            bool flag2 = false;
            if (this.Count() < ConfigSetB.Count())
                flag2 = true;
            else if (this.Count() > ConfigSetB.Count())
            {
                flag2 = false;
            }
            else
            {
                while ((int)num1 < this.Count() & !flag1)
                {
                    LRConfig lrConfig1 = (LRConfig)base[(int)num1];
                    LRConfig lrConfig2 = ConfigSetB[(int)num1];
                    int num2 = lrConfig1.TableIndex();
                    int num3 = lrConfig2.TableIndex();
                    short position1 = lrConfig1.Position;
                    short position2 = lrConfig2.Position;
                    if (num2 != num3)
                    {
                        flag2 = num2 < num3;
                        flag1 = true;
                    }
                    else if ((int)position1 != (int)position2)
                    {
                        flag2 = (int)position1 < (int)position2;
                        flag1 = true;
                    }
                    checked { ++num1; }
                }
            }
            return flag2;
        }

        public bool IsGreaterThan(ref LRConfigSet ConfigSetB)
        {
            LRConfigSet lrConfigSet1 = ConfigSetB;
            LRConfigSet lrConfigSet2 = this;
            ref LRConfigSet local = ref lrConfigSet2;
            return lrConfigSet1.IsLessThan(ref local);
        }

        public LRConfigCompare CompareCore(ref LRConfigSet ConfigB)
        {
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = false;
            if (this.Count() == ConfigB.Count())
            {
                flag1 = false;
                flag2 = false;
                flag3 = false;
                short num = 0;
                while ((int)num < this.Count() & !flag3)
                {
                    LRConfig lrConfig1 = this[(int)num];
                    LRConfig lrConfig2 = ConfigB[(int)num];
                    ref LRConfig local = ref lrConfig2;
                    switch (lrConfig1.CompareCore(ref local))
                    {
                        case LRConfigCompare.ProperSubset:
                            flag2 = true;
                            break;
                        case LRConfigCompare.EqualCore:
                            flag1 = true;
                            break;
                        case LRConfigCompare.UnEqual:
                            flag3 = true;
                            break;
                    }
                    checked { ++num; }
                }
            }
            else
                flag3 = true;
            return !flag3 ? (!flag1 ? (!flag2 ? LRConfigCompare.EqualFull : LRConfigCompare.ProperSubset) : LRConfigCompare.EqualCore) : LRConfigCompare.UnEqual;
        }
    }
}