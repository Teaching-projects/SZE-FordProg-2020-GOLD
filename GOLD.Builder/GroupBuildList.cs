using Microsoft.VisualBasic.CompilerServices;

namespace GOLD.Builder
{
    internal class GroupBuildList : GroupList
    {
        public GroupBuildList()
        {
        }

        internal GroupBuildList(int Size)
          : base(Size)
        {
        }

        public GroupBuild this[int Index]
        {
            get
            {
                return (GroupBuild)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(GroupBuild Item)
        {
            return this.Add((Group)Item);
        }

        public GroupBuild AddUnique(string Name)
        {
            int index = this.ItemIndex(Name);
            if (index != -1)
                return (GroupBuild)base[index];
            GroupBuild groupBuild = new GroupBuild();
            groupBuild.Name = Name;
            this.Add((Group)groupBuild);
            return groupBuild;
        }

        public GroupBuild AddUnique(GroupBuild Grp)
        {
            int index = this.ItemIndex(Grp.Name);
            if (index != -1)
                return (GroupBuild)base[index];
            this.Add((Group)Grp);
            return Grp;
        }

        public int ItemIndex(string Name)
        {
            int num = -1;
            int index = 0;
            while (index < this.Count & num == -1)
            {
                if (Operators.CompareString(base[index].Name.ToUpper(), Name.ToUpper(), true) == 0)
                    num = index;
                checked { ++index; }
            }
            return num;
        }

        public override string ToString()
        {
            string str = "";
            if (this.Count >= 1)
            {
                str = this[0].Name;
                int num = checked(this.Count - 1);
                int index = 1;
                while (index <= num)
                {
                    str = str + ", " + this[index].Name;
                    checked { ++index; }
                }
            }
            return str;
        }
    }
}