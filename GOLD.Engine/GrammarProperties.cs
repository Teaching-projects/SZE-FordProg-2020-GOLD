namespace GOLD.Engine
{
    public class GrammarProperties
    {
        private const int propertyCount = 8;
        private string[] property;

        internal GrammarProperties()
        {
            property = new string[propertyCount];
            for (int i = 0; i < propertyCount; i++)
            {
                property[i] = "";
            }
        }

        internal bool SetValue(int index, string value)
        {
            if (index < 0 || index >= propertyCount)
                return false;

            property[index] = value;
            return true;
        }

        public string Name
        {
            get
            {
                return property[0];
            }
        }

        public string Version
        {
            get
            {
                return property[1];
            }
        }

        public string Author
        {
            get
            {
                return property[2];
            }
        }

        public string About
        {
            get
            {
                return property[3];
            }
        }

        public string CharacterSet
        {
            get
            {
                return property[4];
            }
        }

        public string CharacterMapping
        {
            get
            {
                return property[5];
            }
        }

        public string GeneratedBy
        {
            get
            {
                return property[6];
            }
        }

        public string GeneratedDate
        {
            get
            {
                return property[7];
            }
        }

        private enum PropertyIndex
        {
            Name,
            Version,
            Author,
            About,
            CharacterSet,
            CharacterMapping,
            GeneratedBy,
            GeneratedDate,
        }
    }
}