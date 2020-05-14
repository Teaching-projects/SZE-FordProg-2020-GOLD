namespace GOLD.Builder
{

    internal class Variable
    {
        public string Name;
        public string Value;

        public Variable()
        {
            this.Name = "";
            this.Value = "";
        }

        public Variable(string Name)
        {
            this.Name = Name;
            this.Value = "";
        }
    }
}