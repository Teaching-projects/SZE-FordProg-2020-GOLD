namespace GOLD.Engine
{
    public class Position
    {
        public int Line { get; set; }
        public int Column { get; set; }

        internal Position()
        {
            Line = 0;
            Column = 0;
        }

        internal void Copy(Position pos)
        {
            Column = pos.Column;
            Line = pos.Line;
        }
    }
}