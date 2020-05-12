using System.ComponentModel;

namespace GOLD.Engine
{
    public class Token
    {
        [Description("Returns the line/column position where the token was read.")]
        public Position Position { get; }
        [Description("Returns/sets the object associated with the token.")]
        public object Data { get; set; }
        internal short State { get; set; }
        [Description("Returns the parent symbol of the token.")]
        public Symbol Parent { get; internal set; }
        [Description("Returns the symbol type associated with this token.")]
        public SymbolType Type
        {
            get
            {
                return Parent.Type;
            }
        }
        internal Group Group
        {
            get
            {
                return Parent.Group;
            }
        }

        internal Token()
        {
            Position = new Position();
            Parent = null;
            Data = null;
            State = 0;
        }

        public Token(Symbol parent, object data)
        {
            Position = new Position();
            Parent = parent;
            Data = data;
            State = 0;
        }
    }
}