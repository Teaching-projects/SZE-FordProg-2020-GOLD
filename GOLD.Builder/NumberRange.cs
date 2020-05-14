namespace GOLD.Builder
{
    internal class NumberRange
    {
        public int First { get; }
        public int Last { get; }

        public NumberRange()
        {
            First = 0;
            Last = 0;
        }

        public NumberRange(int first, int last)
        {
            if (First <= Last)
            {
                First = first;
                Last = last;
            }
            else
            {
                First = last;
                Last = first;
            }
        }

        internal NumberRangeCompare Compare(NumberRange numberRange)
        {
            return Compare(numberRange.First, numberRange.Last);
        }

        internal NumberRangeCompare Compare(int first, int last)
        {
            NumberRangeCompare numberRangeCompare = 0;
            if (First < first & Last > last)
                numberRangeCompare = NumberRangeCompare.Superset;
            else if (first < First & last > Last)
                numberRangeCompare = NumberRangeCompare.Subset;
            else if (Last < first)
                numberRangeCompare = NumberRangeCompare.LessThanDisjoint;
            else if (First < first & Last < last)
                numberRangeCompare = NumberRangeCompare.LessThanOverlap;
            else if (last < First)
                numberRangeCompare = NumberRangeCompare.GreaterThanDisjoint;
            else if (first < First & last < Last)
                numberRangeCompare = NumberRangeCompare.GreaterThanOverlap;
            return numberRangeCompare;
        }

        internal NumberRangeRelation Relation(int first, int last)
        {
            return !(Last < first | First > last) ? (!(First >= first & Last <= last) ? (!(First < first & Last > last) ? NumberRangeRelation.Overlap : NumberRangeRelation.Superset) : NumberRangeRelation.Subset) : NumberRangeRelation.Disjoint;
        }
    }
}