using System.Diagnostics;

namespace GOLD.Builder
{
    public class SysLogItem
    {
        public SysLogSection Section;
        public SysLogAlert Alert;
        public string Title;
        public string Description;
        public string Index;

        [DebuggerNonUserCode]
        public SysLogItem()
        {
        }

        public string SectionName()
        {
            string str;
            switch (this.Section)
            {
                case SysLogSection.Internal:
                    str = "Internal";
                    break;
                case SysLogSection.System:
                    str = "System";
                    break;
                case SysLogSection.Grammar:
                    str = "Grammar";
                    break;
                case SysLogSection.DFA:
                    str = "DFA States";
                    break;
                case SysLogSection.LALR:
                    str = "LALR States";
                    break;
                case SysLogSection.CommandLine:
                    str = "Input";
                    break;
                default:
                    str = "(Unspecified)";
                    break;
            }
            return str;
        }

        public string AlertName()
        {
            string str;
            switch (this.Alert)
            {
                case SysLogAlert.Success:
                    str = "Success";
                    break;
                case SysLogAlert.Warning:
                    str = "Warning";
                    break;
                case SysLogAlert.Critical:
                    str = "Error";
                    break;
                default:
                    str = "Details";
                    break;
            }
            return str;
        }
    }
}