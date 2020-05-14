namespace GOLD.Builder
{
    internal class ConfigTrackSet : DictionarySet
    {
        public ConfigTrackSet()
        {
        }

        public ConfigTrackSet(ConfigTrackSet A, ConfigTrackSet B)
          : base((DictionarySet)A, (DictionarySet)B)
        {
        }

        public bool Add(ConfigTrack Item)
        {
            return Add((DictionarySet.IMember)Item);
        }

        public ConfigTrack this[int Index]
        {
            get
            {
                return (ConfigTrack)base[Index];
            }
        }

        public bool UnionWith(ConfigTrackSet SetB)
        {
            return this.UnionWith((DictionarySet)SetB);
        }
    }
}