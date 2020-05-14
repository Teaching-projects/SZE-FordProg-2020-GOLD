
using Microsoft.VisualBasic.CompilerServices;
using System.Diagnostics;

namespace GOLD.Builder
{
    internal class FAEdgeBuildList : FAEdgeList
    {
        [DebuggerNonUserCode]
        public FAEdgeBuildList()
        {
        }

        public FAEdgeBuild this[int Index]
        {
            get
            {
                return (FAEdgeBuild)base[Index];
            }
            set
            {
                this[Index] = value;
            }
        }

        public int Add(FAEdgeBuild Item)
        {
            return this.Add((FAEdge)Item);
        }
    }
}