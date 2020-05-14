using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;

namespace GOLD.Builder
{
    internal class ConfigTrack : DictionarySet.IMember
    {
        public LRConfig Parent;
        public bool FromConfig;
        public bool FromFirst;

        public ConfigTrack(ConfigTrack Track)
        {
            this.Parent = Track.Parent;
            this.FromConfig = Track.FromConfig;
            this.FromFirst = Track.FromFirst;
        }

        public ConfigTrack(LRConfig Config, ConfigTrackSource Source)
        {
            this.Parent = Config;
            this.FromConfig = Source == ConfigTrackSource.Config;
            this.FromFirst = Source == ConfigTrackSource.First;
        }

        IComparable DictionarySet.IMember.Key()
        {
            return (IComparable)this.Parent.TableIndex();
        }

        DictionarySet.MemberResult DictionarySet.IMember.Union(
          DictionarySet.IMember Obj)
        {
            ConfigTrack configTrack = (ConfigTrack)Obj;
            return new DictionarySet.MemberResult((DictionarySet.IMember)new ConfigTrack(this)
            {
                FromConfig = (this.FromConfig | configTrack.FromConfig),
                FromFirst = (this.FromFirst | configTrack.FromFirst)
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

        public string Text()
        {
            object Left = Operators.ConcatenateObject(Operators.ConcatenateObject(Interaction.IIf(this.FromFirst, (object)"F", (object)"-"), Interaction.IIf(this.FromConfig, (object)"C", (object)"-")), (object)" : ");
            LRConfig parent = this.Parent;
            string str1 = "^";
            ref string local = ref str1;
            string str2 = parent.Text(local);
            return Conversions.ToString(Operators.ConcatenateObject(Left, (object)str2));
        }
    }
}