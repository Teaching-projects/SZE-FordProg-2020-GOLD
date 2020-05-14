namespace GOLD.Builder
{
    internal class SetItem : ISetExpression
    {
        public string m_Text;
        public CharacterSetBuild m_Characters;
        public SetItem.SetType m_Type;

        public SetItem(CharacterSetBuild CharSet)
        {
            this.m_Type = SetItem.SetType.Chars;
            this.m_Characters = CharSet;
        }

        public SetItem(SetItem.SetType Type, string Text)
        {
            this.m_Type = Type;
            this.m_Text = Text;
        }

        public SetItem()
        {
            this.m_Type = SetItem.SetType.Name;
        }

        public SetItem.SetType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }

        public string Text
        {
            get
            {
                return this.m_Text;
            }
            set
            {
                this.m_Text = value;
            }
        }

        public CharacterSet Characters
        {
            get
            {
                return (CharacterSet)this.m_Characters;
            }
            set
            {
                this.m_Characters = (CharacterSetBuild)value;
            }
        }


        public enum SetType
        {
            Chars,
            Name,
            Sequence
        }
    }
}