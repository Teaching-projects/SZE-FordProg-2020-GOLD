using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GOLD.Builder
{

    internal sealed class BuilderUI
    {
        public static string GrammarText;


        public static string GrammarFileName = "";
        private static string m_StatusMessage = "";
        public static string TestFileName = "";
        public static WebScheme Colors = new WebScheme();
        public static WebScheme DefaultColors = new WebScheme();
        public static Settings ProgramSettings = new Settings();
        public static MRU FileMRU = new MRU(5);
        public static MRU TestMRU = new MRU(5);
        public const char PositionMarker = '•';
        public static bool GrammarModified;
        public static bool TablesNeedToBeSaved;
        public static WindowMain frmMain;
        public static WindowProperties frmParameters;
        public static WindowCharSet frmCharSets;
        public static WindowSymbols frmSymbols;
        public static WindowProductions frmRules;
        public static WindowDFA frmDFA;
        public static WindowLALR frmLALR;
        public static WindowLog frmLog;
        public static WindowTest frmTestGrammar;
        public static WindowFind frmFind;
        public static WindowReplace frmReplace;
        public static WindowPredefinedSets frmPredefinedTable;
        public static WindowNullableAndFirst frmNullableAndFirst;
        public const string kGrammarExtension = "grm";
        public const string kGrammarLoadFilter = "All Grammar Files|*.grm;*.cgt;*.egt|Grammars|*.grm|Enhanced Grammar Table file (v5) (*.egt)|*.egt|Compiled Grammar Table file (v1)(*.cgt)|*.cgt|Text Files|*.txt;*.text";
        public const string kGrammarSaveFilter = "Grammar|*.grm|Text File|*.txt";
        public const string kLogSaveFilter = "Log|*.log|Text File|*.txt;*.text";
        public const string TableSaveFilter = "Enhanced Grammar Tables (Version 5.0)|*.egt|Compiled Grammar Tables (Version 1.0)|*.cgt|XML (Version 5.0)|*.xml|XML (Version 1.0)|*.xml";
        public static States ApplicationState;
        private static int systemWorking;
        private static long m_ProgramCtrlNum;
        public static bool CriticalError;
        public static bool TestModified;
        public static bool TestPaused;
        public static bool TestParseIsActive;
        public static int TestCurrentReportIndex;
        public static string FindLast;
        public static string ReplaceLast;
        public const string LinkDocumentation = "http://www.GOLDparser.org/doc";
        public const string LinkAboutPicture = "http://www.DevinCook.com/Sacramento";
        public const string LinkHomepage = "http://www.GOLDparser.org";
        public const string LinkCheckUpdate = "http://www.GOLDparser.org/check";
        [AccessedThroughProperty("Printer")]
        private static TextPrinter _Printer;

        static BuilderUI()
        {
            Printer = new TextPrinter();
        }

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeleteMenu(IntPtr hMenu, int uPosition, int uFlags);

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        public static TextPrinter Printer
        {
            [DebuggerNonUserCode]
            get
            {
                return _Printer;
            }
            [DebuggerNonUserCode, MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _Printer = value;
            }
        }

        public static void LoadGrammarKeywords(ref KeywordSet Keywords)
        {
            KeywordSet keywordSet = Keywords;
            keywordSet.Clear();
            keywordSet.Add("![^\\n]*", GetColor("Grammar", "Comment"), "GrmComment");
            keywordSet.Add("'[^\\n]*'", GetColor("Grammar", "Literal"), "GrmLiteral");
            keywordSet.Add("\\'[^\\n]*\\'", GetColor("Grammar", "Property"), "GrmProperty");
            keywordSet.Add("\\[[^\\n]*]", "'[^']*'", GetColor("Grammar", "Set Literal"), "GrmSetLiteral");
            keywordSet.Add("{[a-zA-Z0-9 ]*}", GetColor("Grammar", "Set Name"), "GrmSetName");
            keywordSet.Add("{", GetColor("Grammar", "Set Name"), "GrmSetName");
            keywordSet.Add("}", GetColor("Grammar", "Set Name"), "GrmSetName");
            keywordSet.Add("<[^\\n>]*>", GetColor("Grammar", "Nonterminal"), "GrmNonterminal");
            keywordSet.Add("|", GetColor("Grammar", "Operator"), "GrmOperator");
            keywordSet.Add("::=", GetColor("Grammar", "Operator"), "GrmOperator");
            keywordSet.Add("=", GetColor("Grammar", "Operator"), "GrmOperator");
            keywordSet.Add("[+]", GetColor("Grammar", "Operator"), "GrmOperator");
            keywordSet.Add("-", GetColor("Grammar", "Operator"), "GrmOperator");
            keywordSet.Add("•", GetColor("Grammar", "Marker"));
            keywordSet.Add("#[0-9]+", GetColor("Grammar", "Number"), "GrmDecNumber");
            keywordSet.Add("&[0-9a-fA-F]+", GetColor("Grammar", "Number"), "GrmDecNumber");
            keywordSet.Add("[a-zA-Z][\\-_a-zA-Z0-9]*", GetColor("Grammar", "Terminal"), "GrmTerminal");
        }

        public static void UpdateMainWindowTitle()
        {
            frmMain.UpdateWindowTitle();
        }

        public static FileType GetFileType(string FileName)
        {
            string Left = Microsoft.VisualBasic.Strings.LCase(FileUtility.GetExtension(ref FileName));
            if (Operators.CompareString(Left, "cgt", true) == 0)
                return FileType.CGT;
            if (Operators.CompareString(Left, "egt", true) == 0)
                return FileType.EGT;
            return Operators.CompareString(Left, "grm", true) == 0 || Operators.CompareString(Left, "txt", true) == 0 || Operators.CompareString(Left, "text", true) == 0 ? FileType.Grammar : FileType.Unsupported;
        }

        public static bool IsSystemWorking()
        {
            return systemWorking >= 1;
        }

        public static bool CanEditGrammar()
        {
            if (IsSystemWorking())
                return false;
            switch (ApplicationState)
            {
                case States.GrammarWorking:
                case States.LALRWorking:
                case States.DFAWorking:
                case States.CGTLoaded:
                    return false;
                default:
                    return true;
            }
        }

        public static bool CanTest()
        {
            return BuilderApp.BuildTables.Symbol.Count() >= 1 & BuilderApp.BuildTables.CharSet.Count >= 1 & BuilderApp.BuildTables.DFA.Count >= 1 & BuilderApp.BuildTables.Production.Count() >= 1 & BuilderApp.BuildTables.LALR.Count >= 1;
        }

        public static void IncrementCtrlNum()
        {
            checked { ++m_ProgramCtrlNum; }
            if (m_ProgramCtrlNum <= 32000L)
                return;
            m_ProgramCtrlNum = 0L;
        }

        public static int ProgramCtrlNum()
        {
            return checked((int)m_ProgramCtrlNum);
        }

        public static void SetWorking(bool working)
        {
            if (working)
            {
                systemWorking++;
                if (systemWorking != 1)
                    return;
                AllRefreshControls();
            }
            else
            {
                systemWorking--;
                if (systemWorking <= 0)
                {
                    systemWorking = 0;
                    AllRefreshControls();
                }
            }
        }

        public static void ShowGrammar(int Line = -1)
        {
            Form frmGrammar1 = (Form)frmGrammar;
            int num = IsWindowVisible(ref frmGrammar1) ? 1 : 0;
            frmGrammar = (WindowGrammar)frmGrammar1;
            if (num == 0)
                frmGrammar = new WindowGrammar();
            WindowGrammar frmGrammar2 = frmGrammar;
            frmGrammar2.MdiParent = (Form)frmMain;
            frmGrammar2.Show();
            frmGrammar2.BringToFront();
            if (Line != -1)
                frmGrammar2.GotoLine(Line);
        }

        public static void ShowRules()
        {
            Form frmRules1 = (Form)frmRules;
            int num = IsWindowVisible(ref frmRules1) ? 1 : 0;
            frmRules = (WindowProductions)frmRules1;
            if (num == 0)
                frmRules = new WindowProductions();
            WindowProductions frmRules2 = frmRules;
            frmRules2.MdiParent = (Form)frmMain;
            frmRules2.Show();
            frmRules2.BringToFront();
        }

        public static void ShowParameters()
        {
            Form frmParameters1 = (Form)frmParameters;
            int num = IsWindowVisible(ref frmParameters1) ? 1 : 0;
            frmParameters = (WindowProperties)frmParameters1;
            if (num == 0)
                frmParameters = new WindowProperties();
            WindowProperties frmParameters2 = frmParameters;
            frmParameters2.MdiParent = (Form)frmMain;
            frmParameters2.Show();
            frmParameters2.BringToFront();
        }

        public static void ShowDFA(int Index = 0)
        {
            Form frmDfa1 = (Form)frmDFA;
            int num = IsWindowVisible(ref frmDfa1) ? 1 : 0;
            frmDFA = (WindowDFA)frmDfa1;
            if (num == 0)
                frmDFA = new WindowDFA();
            WindowDFA frmDfa2 = frmDFA;
            frmDfa2.MdiParent = (Form)frmMain;
            frmDfa2.CurrentState = (long)Index;
            frmDfa2.Show();
            frmDfa2.BringToFront();
        }

        public static void ShowLALR(int Index = 0)
        {
            Form frmLalr1 = (Form)frmLALR;
            int num = IsWindowVisible(ref frmLalr1) ? 1 : 0;
            frmLALR = (WindowLALR)frmLalr1;
            if (num == 0)
                frmLALR = new WindowLALR();
            WindowLALR frmLalr2 = frmLALR;
            frmLalr2.MdiParent = (Form)frmMain;
            frmLalr2.CurrentState = (long)Index;
            frmLalr2.Show();
            frmLalr2.BringToFront();
        }

        public static void ShowNullableAndFirst()
        {
            Form nullableAndFirst1 = (Form)frmNullableAndFirst;
            int num = IsWindowVisible(ref nullableAndFirst1) ? 1 : 0;
            frmNullableAndFirst = (WindowNullableAndFirst)nullableAndFirst1;
            if (num == 0)
                frmNullableAndFirst = new WindowNullableAndFirst();
            WindowNullableAndFirst nullableAndFirst2 = frmNullableAndFirst;
            nullableAndFirst2.MdiParent = (Form)frmMain;
            nullableAndFirst2.Show();
            nullableAndFirst2.BringToFront();
        }

        public static void ShowLog()
        {
            Form frmLog1 = (Form)frmLog;
            int num = IsWindowVisible(ref frmLog1) ? 1 : 0;
            frmLog = (WindowLog)frmLog1;
            if (num == 0)
                frmLog = new WindowLog();
            WindowLog frmLog2 = frmLog;
            frmLog2.MdiParent = (Form)frmMain;
            frmLog2.Show();
            frmLog2.BringToFront();
        }

        public static void ShowSymbols()
        {
            Form frmSymbols1 = (Form)frmSymbols;
            int num = IsWindowVisible(ref frmSymbols1) ? 1 : 0;
            frmSymbols = (WindowSymbols)frmSymbols1;
            if (num == 0)
                frmSymbols = new WindowSymbols();
            WindowSymbols frmSymbols2 = frmSymbols;
            frmSymbols2.MdiParent = (Form)frmMain;
            frmSymbols2.Show();
            frmSymbols2.BringToFront();
        }

        public static void ShowCharSets()
        {
            Form frmCharSets1 = (Form)frmCharSets;
            int num = IsWindowVisible(ref frmCharSets1) ? 1 : 0;
            frmCharSets = (WindowCharSet)frmCharSets1;
            if (num == 0)
                frmCharSets = new WindowCharSet();
            WindowCharSet frmCharSets2 = frmCharSets;
            frmCharSets2.MdiParent = (Form)frmMain;
            frmCharSets2.Show();
            frmCharSets2.BringToFront();
        }

        public static void ShowTest()
        {
            Form frmTestGrammar1 = (Form)frmTestGrammar;
            int num = IsWindowVisible(ref frmTestGrammar1) ? 1 : 0;
            frmTestGrammar = (WindowTest)frmTestGrammar1;
            if (num == 0)
                frmTestGrammar = new WindowTest();
            WindowTest frmTestGrammar2 = frmTestGrammar;
            frmTestGrammar2.MdiParent = (Form)frmMain;
            frmTestGrammar2.Show();
            frmTestGrammar2.BringToFront();
        }

        public static void ShowFind()
        {
            Form frmFind1 = (Form)frmFind;
            int num1 = IsWindowVisible(ref frmFind1) ? 1 : 0;
            frmFind = (WindowFind)frmFind1;
            if (num1 == 0)
                frmFind = new WindowFind();
            WindowFind frmFind2 = frmFind;
            int num2 = (int)frmFind2.ShowDialog((IWin32Window)frmMain);
            frmFind2.BringToFront();
        }

        public static void ShowReplace()
        {
            Form frmFind = (Form)frmFind;
            int num1 = IsWindowVisible(ref frmFind) ? 1 : 0;
            frmFind = (WindowFind)frmFind;
            if (num1 == 0)
                frmReplace = new WindowReplace();
            WindowReplace frmReplace = frmReplace;
            int num2 = (int)frmReplace.ShowDialog((IWin32Window)frmMain);
            frmReplace.BringToFront();
        }

        public static void ShowSymbolInfo(int SymbolIndex)
        {
            WindowSymbolInfo windowSymbolInfo = new WindowSymbolInfo();
            windowSymbolInfo.MdiParent = (Form)frmMain;
            windowSymbolInfo.SymbolIndex = SymbolIndex;
            windowSymbolInfo.Show();
            windowSymbolInfo.BringToFront();
        }

        public static void ShowGroupInfo(int GroupIndex)
        {
            WindowGroupInfo windowGroupInfo = new WindowGroupInfo();
            windowGroupInfo.MdiParent = (Form)frmMain;
            windowGroupInfo.GroupIndex = GroupIndex;
            windowGroupInfo.Show();
            windowGroupInfo.BringToFront();
        }

        public static bool IsWindowVisible(ref Form Window)
        {
            return Window != null && ((IEnumerable<Form>)frmMain.MdiChildren).Contains<Form>(Window);
        }

        public static void WindowPaste()
        {
            if (!CanActiveWindowUseClipboard())
                return;
            ((IClipboardForm)frmMain.ActiveMdiChild).Paste();
        }

        public static void WindowCut()
        {
            if (!CanActiveWindowUseClipboard())
                return;
            ((IClipboardForm)frmMain.ActiveMdiChild).Cut();
        }

        public static void WindowDelete()
        {
            if (!CanActiveWindowUseClipboard())
                return;
            ((IClipboardForm)frmMain.ActiveMdiChild).Delete();
        }

        public static void WindowCopy()
        {
            if (!CanActiveWindowUseClipboard())
                return;
            ((IClipboardForm)frmMain.ActiveMdiChild).Copy();
        }

        public static void WindowUndo()
        {
            if (!CanActiveWindowUseClipboard())
                return;
            ((IClipboardForm)frmMain.ActiveMdiChild).Undo();
        }

        public static void WindowRedo()
        {
            if (!CanActiveWindowUseClipboard())
                return;
            ((IClipboardForm)frmMain.ActiveMdiChild).Redo();
        }

        public static void WindowSelectAll()
        {
            if (!CanActiveWindowUseClipboard())
                return;
            ((IClipboardForm)frmMain.ActiveMdiChild).SelectAll();
        }

        public static void GrammarIndent()
        {
            if (frmMain.ActiveMdiChild != frmGrammar)
                return;
            frmGrammar.Indent();
        }

        public static void GrammarUnindent()
        {
            if (frmMain.ActiveMdiChild != frmGrammar)
                return;
            frmGrammar.Unindent();
        }

        public static void GrammarComment()
        {
            if (frmMain.ActiveMdiChild != frmGrammar)
                return;
            frmGrammar.Comment();
        }

        public static void GrammarUncomment()
        {
            if (frmMain.ActiveMdiChild != frmGrammar)
                return;
            frmGrammar.Uncomment();
        }

        public static void GrammarAutoAlign()
        {
            if (frmMain.ActiveMdiChild != frmGrammar)
                return;
            frmGrammar.AutoAlign();
        }

        public static void GrammarBookmarkToggle()
        {
            frmGrammar.BookmarkToggle();
        }

        public static void GrammarBookmarkPrevious()
        {
            frmGrammar.BookmarkPrevious();
        }

        public static void GrammarBookmarkNext()
        {
            frmGrammar.BookmarkNext();
        }

        public static void GrammarBookmarkClearAll()
        {
            if (!MessageBox("Bookmarks", "Clear all bookmarks?", WindowMessage.Style.Question, "Yes", "Cancel").Value)
                return;
            frmGrammar.BookmarkClearAll();
            SetStatusMessage("Bookmarks cleared", "");
        }

        public static void GrammarGoToLine()
        {
            string str = Interaction.InputBox("Please enter the line number.", "Go To Line", "", -1, -1);
            if (!(Operators.CompareString(str, "", true) != 0 & Versioned.IsNumeric((object)str)))
                return;
            frmGrammar.GotoLine(checked((int)Math.Round(Conversion.Val(str))));
        }

        public static bool CanActiveWindowUseClipboard()
        {
            if (frmMain.ActiveMdiChild is IClipboardForm)
                return ((IClipboardForm)frmMain.ActiveMdiChild).CanUseClipboard();
            return false;
        }

        public static void SetupDefaultColors()
        {
            WebScheme defaultColors = DefaultColors;
            defaultColors.Sections().Clear();
            defaultColors.Add("Page", "Background", "255, 255, 255");
            defaultColors.Add("Page", "Text", "0, 0, 0");
            defaultColors.Add("Page", "Lines", "160, 160, 160");
            defaultColors.Add("Grammar", "Comment", "0, 128, 0");
            defaultColors.Add("Grammar", "Literal", "160, 80, 0");
            defaultColors.Add("Grammar", "Property", "160, 0, 0");
            defaultColors.Add("Grammar", "Set Literal", "0, 128, 128");
            defaultColors.Add("Grammar", "Set Name", "128, 0, 128");
            defaultColors.Add("Grammar", "Terminal", "0, 0, 0");
            defaultColors.Add("Grammar", "Nonterminal", "0, 0, 224");
            defaultColors.Add("Grammar", "Operator", "0, 0, 0");
            defaultColors.Add("Grammar", "Marker", "255, 0, 0");
            defaultColors.Add("Grammar", "Number", "96, 0, 192");
            defaultColors.Add("Report", "Shift", "0, 0, 108");
            defaultColors.Add("Report", "Reduce", "96, 48, 16");
            defaultColors.Add("Report", "Accept", "0, 96, 0");
            defaultColors.Add("Report", "Goto", "96, 0, 96");
            defaultColors.Add("Report", "Token", "0, 0, 0");
            defaultColors.Add("Report", "Conflict", "192, 0, 0");
            defaultColors.Add("Report", "Warning", "72, 72, 0");
            defaultColors.Add("Report", "Detail", "32, 64, 128");
            defaultColors.Add("Report", "Success", "0, 96, 0");
            defaultColors.Add("Report", "Description", "0, 96, 48");
            defaultColors.Add("Select", "Text", "255, 255, 255");
            defaultColors.Add("Select", "Background", "64, 128, 255");
            defaultColors.Add("Select", "Bookmark", "0, 128, 128");
            defaultColors.Add("Division", "Background", "64, 80, 128");
            defaultColors.Add("Division", "Border", "32, 48, 64");
            defaultColors.Add("Division", "Text", "255, 255, 255");
            defaultColors.Add("Section", "Background", "64, 80, 128");
            defaultColors.Add("Section", "Border", "32, 48, 64");
            defaultColors.Add("Section", "Text", "255, 255, 255");
            defaultColors.Add("Subsection", "Background", "224, 224, 224");
            defaultColors.Add("Subsection", "Border", "192, 192, 192");
            defaultColors.Add("Subsection", "Text", "0, 0, 0");
        }

        public static WebScheme DefaultWebColors()
        {
            WebScheme webScheme1 = new WebScheme();
            WebScheme webScheme2 = webScheme1;
            webScheme2.Sections().Clear();
            webScheme2.Add("Page", "Background", "255, 255, 255");
            webScheme2.Add("Page", "Text", "0, 0, 0");
            webScheme2.Add("Page", "Lines", "160, 160, 160");
            webScheme2.Add("Grammar", "Comment", "0, 128, 0");
            webScheme2.Add("Grammar", "Literal", "160, 80, 0");
            webScheme2.Add("Grammar", "Property", "160, 0, 0");
            webScheme2.Add("Grammar", "Set Literal", "0, 128, 128");
            webScheme2.Add("Grammar", "Set Name", "128, 0, 128");
            webScheme2.Add("Grammar", "Terminal", "24, 48, 96");
            webScheme2.Add("Grammar", "Nonterminal", "40, 80, 160");
            webScheme2.Add("Grammar", "Operator", "64, 64, 64");
            webScheme2.Add("Grammar", "Marker", "255, 0, 0");
            webScheme2.Add("Grammar", "Number", "96, 0, 192");
            webScheme2.Add("Report", "Shift", "0, 0, 108");
            webScheme2.Add("Report", "Reduce", "96, 48, 16");
            webScheme2.Add("Report", "Accept", "0, 96, 0");
            webScheme2.Add("Report", "Goto", "96, 0, 96");
            webScheme2.Add("Report", "Token", "0, 0, 0");
            webScheme2.Add("Report", "Conflict", "192, 0, 0");
            webScheme2.Add("Report", "Warning", "72, 72, 0");
            webScheme2.Add("Report", "Detail", "32, 64, 128");
            webScheme2.Add("Report", "Success", "0, 96, 0");
            webScheme2.Add("Report", "Description", "0, 96, 48");
            webScheme2.Add("Select", "Text", "255, 255, 255");
            webScheme2.Add("Select", "Background", "64, 128, 255");
            webScheme2.Add("Select", "Bookmark", "0, 128, 128");
            webScheme2.Add("Division", "Background", "48, 96, 192");
            webScheme2.Add("Division", "Border", "16, 32, 64");
            webScheme2.Add("Division", "Text", "255, 255, 255");
            webScheme2.Add("Section", "Background", "40, 80, 160");
            webScheme2.Add("Section", "Border", "16, 32, 64");
            webScheme2.Add("Section", "Text", "255, 255, 255");
            webScheme2.Add("Subsection", "Background", "32, 64, 128");
            webScheme2.Add("Subsection", "Border", "16, 32, 64");
            webScheme2.Add("Subsection", "Text", "255, 255, 255");
            return webScheme1;
        }

        public static void Setup()
        {
            ApplicationState = States.GrammarReady;
            SetupDefaultColors();
            Colors.AddKeys((Settings)DefaultColors);
            Colors.SetAllScope(Settings.SettingScope.User);
            Colors.Name = "GOLD Parser Builder\\Colors";
            FindLast = "";
            ReplaceLast = "";
            Settings programSettings = ProgramSettings;
            programSettings.Name = "GOLD Parser Builder";
            programSettings.DefaultScope = Settings.SettingScope.User;
            programSettings.Add("Width", Conversions.ToString(800));
            programSettings.Add("Height", Conversions.ToString(600));
            programSettings.Add("Left", Conversions.ToString((double)checked(Screen.PrimaryScreen.WorkingArea.Width - 800) / 2.0));
            programSettings.Add("Top", Conversions.ToString((double)checked(Screen.PrimaryScreen.WorkingArea.Height - 600) / 2.0));
            programSettings.Add("Warn Edit Reset", Conversions.ToString(true));
            programSettings.Add("Allow Edit Color Files", "Change to FALSE to prevent the user from deleting/saving color schemes");
            programSettings.Add("Grammar File Encoding", "Auto");
            programSettings.Add("MRU", "File 0", "");
            programSettings.Add("MRU", "File 1", "");
            programSettings.Add("MRU", "File 2", "");
            programSettings.Add("MRU", "File 3", "");
            programSettings.Add("MRU", "File 4", "");
            programSettings.Add("Editor", "Font Name", "Courier New");
            programSettings.Add("Editor", "Font Size", Conversions.ToString(9));
            programSettings.Add("Editor", "Font Bold", Conversions.ToString(true));
            programSettings.Add("Editor", "Indent", Conversions.ToString(4));
            programSettings.Add("Editor", "Save Folder", "");
            programSettings.Add("Editor", "Load Folder", "");
            programSettings.Add("Editor", "Tab Behavior", "Tab");
            programSettings.Add("Test", "Font Name", "Courier New");
            programSettings.Add("Test", "Font Size", Conversions.ToString(9));
            programSettings.Add("Test", "Font Bold", Conversions.ToString(true));
            programSettings.Add("Test", "Save Folder", "");
            programSettings.Add("Test", "Load Folder", "");
            programSettings.Add("Test", "Load Pattern", "*.txt");
            programSettings.Add("Test", "Notify Test Done", Conversions.ToString(true));
            programSettings.Add("Test", "Notify Save Changes", Conversions.ToString(true));
            programSettings.Add("Test", "Report Whitespace", Conversions.ToString(false));
            programSettings.Add("Test", "Report Parser", Conversions.ToString(true));
            programSettings.Add("Test", "Auto Recover Limit", Conversions.ToString(0));
            programSettings.Add("Test", "Warn Edit Reset", Conversions.ToString(true));
            programSettings.Add("Test\\MRU", "File 0", "");
            programSettings.Add("Test\\MRU", "File 1", "");
            programSettings.Add("Test\\MRU", "File 2", "");
            programSettings.Add("Test\\MRU", "File 3", "");
            programSettings.Add("Test\\MRU", "File 4", "");
            programSettings.Add("Tables", "Font Name", "Courier New");
            programSettings.Add("Tables", "Font Size", Conversions.ToString(9));
            programSettings.Add("Tables", "Font Bold", Conversions.ToString(true));
            programSettings.Add("Page Setup", "Font Name", "Courier New");
            programSettings.Add("Page Setup", "Font Size", Conversions.ToString(9));
            programSettings.Add("Page Setup", "Font Bold", Conversions.ToString(true));
            programSettings.Add("Page Setup", "Scale", "in");
            programSettings.Add("Page Setup", "Margin Left", Conversions.ToString(0.75));
            programSettings.Add("Page Setup", "Margin Right", Conversions.ToString(0.75));
            programSettings.Add("Page Setup", "Margin Top", Conversions.ToString(0.75));
            programSettings.Add("Page Setup", "Margin Bottom", Conversions.ToString(0.75));
            programSettings.Add("Page Setup", "Line Numbers", Conversions.ToString(true));
            programSettings.Add("Templates", "Save Folder", "");
            programSettings.Add("Import", "Load Folder", "");
            programSettings.LoadRegistry();
        }

        private static Color RGB(byte Red, byte Green, byte Blue)
        {
            return Color.FromArgb((int)Red, (int)Green, (int)Blue);
        }

        public static void ProjectAnalyzeGrammar()
        {
            SetWorking(true);
            Reset();
            SetApplicationState(States.GrammarWorking);
            TextReader MetaGrammar = new StringReader(GrammarText);
            BuilderApp.EnterGrammar(ref MetaGrammar);
            if (BuilderApp.LoggedCriticalError())
            {
                SetApplicationState(States.GrammarFailed);
                ShowLog();
            }
            else if (BuilderApp.LoggedWarning())
            {
                SetApplicationState(States.LALRReady);
                ShowLog();
            }
            else
                SetApplicationState(States.LALRReady);
            AllUpdateData();
            SetWorking(false);
        }

        public static void ProjectCreateLALR()
        {
            SetWorking(true);
            SetApplicationState(States.LALRWorking);
            BuilderApp.ComputeLALR();
            IncrementCtrlNum();
            if (BuilderApp.CancelTableBuild)
            {
                SetApplicationState(States.LALRFailed);
                ShowLog();
            }
            else if (BuilderApp.LoggedCriticalError())
            {
                SetApplicationState(States.LALRFailed);
                ShowLog();
            }
            else if (BuilderApp.LoggedWarning())
            {
                SetApplicationState(States.DFAReady);
                ShowLog();
            }
            else
                SetApplicationState(States.DFAReady);
            AllUpdateData();
            SetWorking(false);
        }

        public static void ProjectCreateDFA()
        {
            SetWorking(true);
            SetApplicationState(States.DFAWorking);
            BuilderApp.ComputeDFA();
            IncrementCtrlNum();
            if (BuilderApp.CancelTableBuild)
            {
                SetApplicationState(States.DFAFailed);
                ShowLog();
            }
            else if (BuilderApp.LoggedCriticalError())
            {
                SetApplicationState(States.DFAFailed);
                ShowLog();
            }
            else if (BuilderApp.LoggedWarning())
            {
                SetApplicationState(States.TablesReady);
                TablesNeedToBeSaved = true;
                ShowLog();
            }
            else
            {
                BuilderApp.Log.Add(SysLogSection.System, SysLogAlert.Success, "The grammar was successfully analyzed and tables were created.");
                SetApplicationState(States.TablesReady);
                TablesNeedToBeSaved = true;
            }
            AllUpdateData();
            SetWorking(false);
        }

        public static void ProjectFinish()
        {
            BuilderApp.ComputeComplete();
            AllUpdateData();
            SetStatusMessage("Complete.", "");
        }

        public static void ProjectSaveTables()
        {
            SetWorking(true);
            if (SaveTablesDialog())
                SetApplicationState(States.TablesSaved);
            SetWorking(false);
        }

        public static bool CreateSkeletonProgram(string TemplatePath, string TargetPath)
        {
            ProgramTemplate.Tables = (ParseTables)BuilderApp.BuildTables;
            return ProgramTemplate.Create(TemplatePath, ref TargetPath);
        }

        public static void ProjectSkeletonProgram()
        {
            int num = (int)new WindowCreateSkeleton().ShowDialog((IWin32Window)frmMain);
        }

        public static void ShowTestMultipleFiles()
        {
            int num = (int)new WindowTestMultiple().ShowDialog((IWin32Window)frmMain);
        }

        public static void ShowPageSetup()
        {
            int num = (int)new WindowPageSetup().ShowDialog((IWin32Window)frmMain);
        }

        public static void ShowOptions()
        {
            int num = (int)new WindowOptions().ShowDialog((IWin32Window)frmMain);
        }

        public static void ShowTestOptions()
        {
            int num = (int)new WindowTestOptions().ShowDialog((IWin32Window)frmMain);
        }

        public static void ShowRevisions()
        {
            int num = (int)new WindowRevisions().ShowDialog((IWin32Window)frmMain);
        }

        public static void ShowExportText()
        {
            int num = (int)new WindowExportText().ShowDialog((IWin32Window)frmMain);
        }

        public static void ShowExportWebpage()
        {
            int num = (int)new WindowExportWebpage().ShowDialog((IWin32Window)frmMain);
        }

        public static void ShowPredefinedTable()
        {
            Form frmPredefinedTable1 = (Form)frmPredefinedTable;
            int num = IsWindowVisible(ref frmPredefinedTable1) ? 1 : 0;
            frmPredefinedTable = (WindowPredefinedSets)frmPredefinedTable1;
            if (num == 0)
                frmPredefinedTable = new WindowPredefinedSets();
            WindowPredefinedSets frmPredefinedTable2 = frmPredefinedTable;
            frmPredefinedTable2.MdiParent = (Form)frmMain;
            frmPredefinedTable2.Show();
            frmPredefinedTable2.BringToFront();
        }

        public static void ExportToXML()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            SaveFileDialog saveFileDialog2 = saveFileDialog1;
            saveFileDialog2.DefaultExt = "xml";
            saveFileDialog2.Filter = "XML File|*.xml|XML File (GOLD Version 1.0)|*.xml";
            saveFileDialog2.FileName = FileUtility.GetFileNameBase(ref GrammarFileName) + ".xml";
            saveFileDialog2.OverwritePrompt = true;
            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FilterIndex != 2 ? BuilderApp.BuildTables.SaveXML5(saveFileDialog2.FileName) : BuilderApp.BuildTables.SaveXML1(saveFileDialog2.FileName))
                {
                    SetStatusMessage("XML file was successfully created", "");
                }
                else
                {
                    MessageBox("Export error", "There was an error saving the exporting the XML file", WindowMessage.Style.Error);
                    SetStatusMessage("There was an error saving the exporting the XML file", "");
                }
            }
        }

        public static void ExportToYACC()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (!MessageBox("Export YACC/Bison", "This tool will only export the grammar's productions to the format used by the YACC/Bison compiler-compilers.The 'Lex' definitions must be done manually.\r\n\r\nDo you wish to continue?", WindowMessage.Style.Question, "Export", "Cancel").Value)
                return;
            SaveFileDialog saveFileDialog2 = saveFileDialog1;
            saveFileDialog2.DefaultExt = "y";
            saveFileDialog2.Filter = "YACC Grammar Files|*.y";
            saveFileDialog2.FileName = FileUtility.GetFileNameBase(ref GrammarFileName) + ".y";
            saveFileDialog2.OverwritePrompt = true;
            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                if (ExportText.SaveTablesYACC(saveFileDialog2.FileName))
                {
                    SetStatusMessage("YACC file was successfully created", "");
                }
                else
                {
                    MessageBox("Export error", "There was an error saving the exporting the YACC file", WindowMessage.Style.Error);
                    SetStatusMessage("There was an error saving the exporting the YACC file", "");
                }
            }
        }

        public static void PerformNextAction()
        {
            switch (ApplicationState)
            {
                case States.GrammarReady:
                    ProjectAnalyzeGrammar();
                    break;
                case States.LALRReady:
                    ProjectCreateLALR();
                    break;
                case States.DFAReady:
                    ProjectCreateDFA();
                    ProjectFinish();
                    break;
                case States.TablesReady:
                    ProjectSaveTables();
                    break;
            }
        }

        public static void SetApplicationState(States newState)
        {
            ApplicationState = newState;
            AllRefreshControls();
            AllUpdateData();
        }

        public static void Reset()
        {
            try
            {
                BuilderApp.Restart();
                IncrementCtrlNum();
                TablesNeedToBeSaved = false;
                SetStatusMessage("", "");
                SetApplicationState(States.GrammarReady);
                AllUpdateData();
                AllRefreshControls();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                ProjectData.ClearProjectError();
            }
        }

        public static void AllRefreshControls()
        {
            //frmMain.RefreshControls();
            //int num = checked(((IEnumerable<Form>)frmMain.MdiChildren).Count<Form>() - 1);
            //int index = 0;
            //while (index <= num)
            //{
            //    if (frmMain.MdiChildren[index] is IUpdateForm)
            //        ((IUpdateForm)frmMain.MdiChildren[index]).RefreshControls();
            //    checked { ++index; }
            //}
        }

        public static void AllRefreshStyles()
        {
            //int num = checked(((IEnumerable<Form>)frmMain.MdiChildren).Count<Form>() - 1);
            //int index = 0;
            //while (index <= num)
            //{
            //    if (frmMain.MdiChildren[index] is IStyleForm)
            //        ((IStyleForm)frmMain.MdiChildren[index]).RefreshStyles();
            //    checked { ++index; }
            //}
        }

        public static void AllUpdateData()
        {
            //Form[] mdiChildren = frmMain.MdiChildren;
            //int index = 0;
            //while (index < mdiChildren.Length)
            //{
            //    Form form = mdiChildren[index];
            //    if (form is IUpdateForm)
            //        ((IUpdateForm)form).UpdateData();
            //    checked { ++index; }
            //}
        }

        public static bool SaveTablesDialog()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            WindowCGTWarnings windowCgtWarnings = new WindowCGTWarnings();
            SaveFileDialog saveFileDialog2 = saveFileDialog1;
            saveFileDialog2.AddExtension = true;
            saveFileDialog2.DefaultExt = "egt";
            saveFileDialog2.Filter = "Enhanced Grammar Tables (Version 5.0)|*.egt|Compiled Grammar Tables (Version 1.0)|*.cgt|XML (Version 5.0)|*.xml|XML (Version 1.0)|*.xml";
            saveFileDialog2.FileName = FileUtility.GetFileNameBase(ref GrammarFileName);
            saveFileDialog2.Title = "Save table data";
            saveFileDialog2.OverwritePrompt = true;
            bool flag1;
            switch (saveFileDialog1.ShowDialog())
            {
                case DialogResult.OK:
                    int num1;
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 2:
                        case 4:
                            num1 = 1;
                            break;
                        default:
                            num1 = 5;
                            break;
                    }
                    bool flag2 = true;
                    try
                    {
                        if (num1 == 1 & BuilderApp.SaveCGTWarnings.Count() >= 1)
                            flag2 = windowCgtWarnings.ShowDialog() == DialogResult.OK;
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        Information.Err().Clear();
                        ProjectData.ClearProjectError();
                    }
                    if (flag2)
                    {
                        SetStatusMessage("Saving the tables...", "");
                        switch (saveFileDialog1.FilterIndex)
                        {
                            case 1:
                                ParseTablesBuild buildTables1 = BuilderApp.BuildTables;
                                SaveFileDialog saveFileDialog3 = saveFileDialog1;
                                string fileName1 = saveFileDialog3.FileName;
                                ref string local1 = ref fileName1;
                                int num2 = buildTables1.SaveVer5(ref local1) ? 1 : 0;
                                saveFileDialog3.FileName = fileName1;
                                flag1 = num2 != 0;
                                break;
                            case 2:
                                ParseTablesBuild buildTables2 = BuilderApp.BuildTables;
                                SaveFileDialog saveFileDialog4 = saveFileDialog1;
                                string fileName2 = saveFileDialog4.FileName;
                                ref string local2 = ref fileName2;
                                int num3 = buildTables2.SaveVer1(ref local2) ? 1 : 0;
                                saveFileDialog4.FileName = fileName2;
                                flag1 = num3 != 0;
                                break;
                            case 3:
                                flag1 = BuilderApp.BuildTables.SaveXML5(saveFileDialog1.FileName);
                                break;
                            case 4:
                                flag1 = BuilderApp.BuildTables.SaveXML1(saveFileDialog1.FileName);
                                break;
                        }
                        if (flag1)
                        {
                            SetStatusMessage("The file was successfully created", "");
                            TablesNeedToBeSaved = false;
                            break;
                        }
                        SetStatusMessage("There was an error saving the file. The tables were not saved", "");
                        MessageBox("File save error", "There was an error saving the file. The parse tables are NOT saved", WindowMessage.Style.Error);
                        break;
                    }
                    SetStatusMessage("File save was canceled.", "");
                    break;
                case DialogResult.Cancel:
                    SetStatusMessage("File save was canceled", "");
                    flag1 = false;
                    break;
            }
            return flag1;
        }

        public static bool OpenGrammarNoDialog(string Path)
        {
            if (CanDiscardGrammarPrompt())
                OpenGrammar(Path);
            bool flag;
            return flag;
        }

        public static bool OpenGrammarWithDialog()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            bool flag = false;
            if (CanDiscardGrammarPrompt())
            {
                OpenFileDialog openFileDialog2 = openFileDialog1;
                openFileDialog2.AddExtension = true;
                openFileDialog2.InitialDirectory = ProgramSettings["Editor", "Load Folder"].Value;
                openFileDialog2.DefaultExt = "grm";
                openFileDialog2.Filter = "All Grammar Files|*.grm;*.cgt;*.egt|Grammars|*.grm|Enhanced Grammar Table file (v5) (*.egt)|*.egt|Compiled Grammar Table file (v1)(*.cgt)|*.cgt|Text Files|*.txt;*.text";
                openFileDialog2.Title = "Load File";
                switch (openFileDialog1.ShowDialog())
                {
                    case DialogResult.OK:
                        flag = OpenGrammar(openFileDialog1.FileName);
                        Settings.Key programSetting = ProgramSettings["Editor", "Load Folder"];
                        OpenFileDialog openFileDialog3 = openFileDialog1;
                        string fileName = openFileDialog3.FileName;
                        string path = FileUtility.GetPath(ref fileName);
                        openFileDialog3.FileName = fileName;
                        programSetting.Value = path;
                        break;
                    case DialogResult.Cancel:
                        SetStatusMessage("File open was canceled", "");
                        flag = false;
                        break;
                }
            }
            return flag;
        }

        public static bool NewGrammarWizard()
        {
            bool flag = false;
            if (CanDiscardGrammarPrompt())
            {
                WindowNewGrammar windowNewGrammar = new WindowNewGrammar();
                if (windowNewGrammar.ShowDialog() == DialogResult.OK)
                    NewGrammar(ref windowNewGrammar.Grammar);
            }
            return flag;
        }

        public static bool NewGrammarBlank()
        {
            if (!CanDiscardGrammarPrompt())
                return false;
            string Text = "";
            NewGrammar(ref Text);
            return true;
        }

        private static void NewGrammar(ref string Text)
        {
            frmGrammar.Grammar = Text;
            GrammarFileName = "";
            GrammarModified = false;
            Reset();
            UpdateMainWindowTitle();
        }

        public static void ShowAbout()
        {
            int num = (int)new WindowAbout().ShowDialog((IWin32Window)frmMain);
        }

        public static bool YACCImportWizard()
        {
            bool flag = false;
            if (CanDiscardGrammarPrompt())
            {
                WindowImportYACC windowImportYacc = new WindowImportYACC();
                if (windowImportYacc.ShowDialog() == DialogResult.OK)
                    frmGrammar.Grammar = windowImportYacc.Grammar;
            }
            return flag;
        }

        public static bool OpenTestWithDialog()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            bool flag = false;
            if (CanDiscardTestPrompt())
            {
                OpenFileDialog openFileDialog2 = openFileDialog1;
                openFileDialog2.AddExtension = true;
                openFileDialog2.InitialDirectory = ProgramSettings["Test", "Load Folder"].Value;
                openFileDialog2.Filter = "Text Files (*.txt)|*.txt;*.text|All files (*.*)|*.*";
                openFileDialog2.Title = "Load Test File";
                openFileDialog2.FilterIndex = 2;
                switch (openFileDialog1.ShowDialog())
                {
                    case DialogResult.OK:
                        ShowTest();
                        flag = frmTestGrammar.OpenFile(openFileDialog1.FileName);
                        break;
                    case DialogResult.Cancel:
                        SetStatusMessage("File open was canceled", "");
                        flag = false;
                        break;
                }
                if (flag)
                {
                    Settings.Key programSetting = ProgramSettings["Test", "Load Folder"];
                    OpenFileDialog openFileDialog3 = openFileDialog1;
                    string fileName = openFileDialog3.FileName;
                    string path = FileUtility.GetPath(ref fileName);
                    openFileDialog3.FileName = fileName;
                    programSetting.Value = path;
                    OpenTest(openFileDialog1.FileName);
                }
            }
            return flag;
        }

        public static bool OpenTestNoDialog(string FilePath)
        {
            if (CanDiscardTestPrompt())
                return OpenTest(FilePath);
            return false;
        }

        public static bool OpenTest(string FilePath)
        {
            ShowTest();
            bool flag = frmTestGrammar.OpenFile(FilePath);
            if (flag)
            {
                TestFileName = FilePath;
                TestModified = false;
                TestMRU.Add(FilePath);
                frmMain.RefreshControls();
                frmTestGrammar.RefreshControls();
            }
            return flag;
        }

        public static bool CanDiscardTestPrompt()
        {
            if (!(TestModified & Operators.CompareString(ProgramSettings["Test", "Notify Save Changes"].Value.ToUpper(), "TRUE", true) == 0))
                return true;
            WindowMessage.Result result = MessageBox("Discard test data?", "The test data was modified since the last time you saved it.", WindowMessage.Style.Warning, "Discard", "Cancel", "Always show this warning", true);
            ProgramSettings["Test", "Notify Save Changes"].Value = Conversions.ToString(result.Checked);
            return result.Value;
        }

        public static bool CanEditTestPrompt()
        {
            bool flag = true;
            if (TestParseIsActive)
            {
                if (Operators.CompareString(ProgramSettings["Test", "Warn Edit Reset"].Value.ToUpper(), "TRUE", true) == 0)
                {
                    WindowMessage.Result result = MessageBox("Reset?", "By editting the source, you will stop the current test.", WindowMessage.Style.Warning, "Proceed", "Cancel", "Always show this warning", true);
                    ProgramSettings["Test", "Warn Edit Reset"].Value = Conversions.ToString(result.Checked);
                    flag = result.Value;
                }
                else
                    flag = true;
                if (flag)
                    TestAbort();
            }
            return flag;
        }

        public static bool SaveGrammarDialog()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            SaveFileDialog saveFileDialog2 = saveFileDialog1;
            saveFileDialog2.AddExtension = true;
            saveFileDialog2.DefaultExt = "grm";
            saveFileDialog2.Filter = "Grammar|*.grm|Text File|*.txt";
            saveFileDialog2.Title = "Save Grammar...";
            saveFileDialog2.OverwritePrompt = true;
            if (Operators.CompareString(GrammarFileName, "", true) == 0)
            {
                saveFileDialog2.FileName = "";
                saveFileDialog2.InitialDirectory = ProgramSettings["Editor", "Save Folder"].Value;
            }
            else
            {
                saveFileDialog2.InitialDirectory = FileUtility.GetPath(ref GrammarFileName);
                saveFileDialog2.FileName = FileUtility.GetFileName(ref GrammarFileName);
            }
            bool flag;
            switch (saveFileDialog1.ShowDialog())
            {
                case DialogResult.OK:
                    flag = SaveGrammar(saveFileDialog1.FileName);
                    Settings.Key programSetting = ProgramSettings["Editor", "Save Folder"];
                    SaveFileDialog saveFileDialog3 = saveFileDialog1;
                    string fileName = saveFileDialog3.FileName;
                    string path = FileUtility.GetPath(ref fileName);
                    saveFileDialog3.FileName = fileName;
                    programSetting.Value = path;
                    break;
                case DialogResult.Cancel:
                    SetStatusMessage("File save was canceled", "");
                    flag = false;
                    break;
            }
            return flag;
        }

        public static void SaveLogDialog()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            SaveFileDialog saveFileDialog2 = saveFileDialog1;
            saveFileDialog2.AddExtension = true;
            saveFileDialog2.DefaultExt = "log";
            saveFileDialog2.Filter = "Log files|*.log|Text Files|*.txt";
            saveFileDialog2.Title = "Save Log";
            saveFileDialog2.FileName = FileUtility.GetFileNameBase(ref GrammarFileName) + ".log";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (BuilderApp.SaveLog(saveFileDialog1.FileName))
                        return;
                    SetStatusMessage("Log was successfully saved", "");
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Information.Err().Clear();
                    SetStatusMessage("There was an error saving the log", "");
                    MessageBox("Save Error", "There was an error saving the log", WindowMessage.Style.Error);
                    ProjectData.ClearProjectError();
                }
            }
            else
                SetStatusMessage("File save was canceled", "");
        }

        public static bool OpenCommandLine(string FilePath)
        {
            if (FilePath.Length < 2)
                return false;
            if (FilePath[0] == '"')
                FilePath = FilePath.Substring(1, checked(FilePath.Length - 2));
            return OpenGrammar(FilePath);
        }

        private static bool OpenGrammar(string FilePath)
        {
            string Message = "";
            SetStatusMessage("Loading " + FileUtility.GetFileName(ref FilePath) + " ...", "");
            bool flag;
            try
            {
                switch (GetFileType(FilePath))
                {
                    case FileType.CGT:
                    case FileType.EGT:
                        flag = BuilderApp.LoadTables(FilePath);
                        if (flag)
                        {
                            string str = "! The grammar table file was successfully loaded.\r\n!\r\n! The file only contains the Symbol Table, Production Table, LALR State Table\r\n! DFA State Table and other related objects; not the original grammar.\r\n!\r\n! The Defined Character Sets are blank since these are used for the\r\n! construction of the Deterministic Finite Automata (used by the\r\n! tokenizer) and not saved to the file (although it does contain\r\n! character sets created at runtime).\r\n!\r\n! In addition, the configurations normally displayed in the LALR State\r\n! Table are not present. These are created at compile-time to create\r\n! the parser's LALR table and are also not saved to the file.";
                            frmGrammar.Grammar = str;
                            SetApplicationState(States.CGTLoaded);
                            break;
                        }
                        Message = "Compiled Grammar Table file could not be opened";
                        break;
                    case FileType.Grammar:
                        flag = frmGrammar.OpenFile(FilePath);
                        if (flag)
                        {
                            Reset();
                            SetApplicationState(States.GrammarReady);
                            break;
                        }
                        Message = "Grammar file could not be opened";
                        break;
                    case FileType.Unsupported:
                        flag = false;
                        Message = "Unsupported file type: ." + FileUtility.GetExtension(ref FilePath);
                        break;
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Message = ex.Message;
                flag = false;
                ProjectData.ClearProjectError();
            }
            if (flag)
            {
                FileMRU.Add(FilePath);
                GrammarFileName = FilePath;
                GrammarModified = false;
                UpdateMainWindowTitle();
                frmMain.RefreshControls();
                SetStatusMessage("The file was successfully loaded", "");
            }
            else
            {
                SetStatusMessage("File open error: " + Message, "");
                MessageBox("File open error", Message, WindowMessage.Style.Error);
            }
            return flag;
        }

        public static bool CanDiscardGrammarPrompt()
        {
            if (GrammarModified)
                return MessageBox("Grammar Modified", "The grammar was modified since the last time you saved it.\r\n\r\nDo you want to discard changes?", WindowMessage.Style.Warning, "Discard", "Cancel").Value;
            return true;
        }

        private static bool SaveGrammar(string FilePath)
        {
            string Left = ProgramSettings["Grammar File Encoding"].Value;
            bool flag = Operators.CompareString(Left, "UTF-8", true) != 0 ? (Operators.CompareString(Left, "UTF-16", true) != 0 ? frmGrammar.SaveFile(FilePath) : frmGrammar.SaveFile(FilePath, Encoding.Unicode)) : frmGrammar.SaveFile(FilePath, Encoding.UTF8);
            if (flag)
            {
                SetStatusMessage("The grammar was saved at " + CurrentTime(), "");
                FileMRU.Add(FilePath);
                GrammarFileName = FilePath;
                GrammarModified = false;
                UpdateMainWindowTitle();
                frmMain.RefreshControls();
            }
            else
            {
                SetStatusMessage("There was an error saving the file", "");
                MessageBox("File Save Error", "There was an error saving the file", WindowMessage.Style.Error);
            }
            return flag;
        }

        public static bool SaveGrammarDialogIfNeeded()
        {
            return Operators.CompareString(GrammarFileName, "", true) != 0 ? SaveGrammar(GrammarFileName) : SaveGrammarDialog();
        }

        public static void SetStatusMessage(string Title, string Description = "")
        {
            string Text = Operators.CompareString(Description, "", true) == 0 ? Title : Title + " " + Description;
            m_StatusMessage = Text;
            frmMain.SetStatusMessage(Text);
        }

        public static void TestInitializeParser()
        {
            TestGrammar.TestParser.Tables = (ParseTables)BuilderApp.BuildTables;
        }

        private static void TestStart()
        {
            TestInitializeParser();
            Settings programSettings = ProgramSettings;
            TestGrammar.RecoverLimit = checked((long)Math.Round(Conversion.Val(programSettings["Test", "Auto Recover Limit"].Value)));
            TestGrammar.ReportNoise = Operators.CompareString(programSettings["Test", "Report Whitespace"].Value.ToUpper(), "TRUE", true) == 0;
            TestGrammar.ReportParser = Operators.CompareString(programSettings["Test", "Report Parser"].Value.ToUpper(), "TRUE", true) == 0;
            frmTestGrammar.Clear();
            TestCurrentReportIndex = -1;
            TestGrammar.Start((TextReader)new StringReader(frmTestGrammar.TestString));
            TestParseIsActive = true;
        }

        private static void TestDrawResults()
        {
            bool flag = false;
            string str = "";
            frmTestGrammar.BeginPopulateReport();
            int num1 = checked(TestCurrentReportIndex + 1);
            int num2 = checked(TestGrammar.Report.Count - 1);
            int index = num1;
            int integer1;
            int integer2;
            int textLength;
            while (index <= num2)
            {
                TestParseItem testParseItem = TestGrammar.Report[index];
                frmTestGrammar.AddReportItem(testParseItem);
                if (Operators.CompareString(testParseItem.LineNumber, "", true) != 0)
                {
                    integer1 = Conversions.ToInteger(testParseItem.ColumnNumber);
                    integer2 = Conversions.ToInteger(testParseItem.LineNumber);
                    textLength = testParseItem.TextLength;
                    flag = true;
                }
                switch (testParseItem.Action)
                {
                    case TestParseAction.LexicalError:
                    case TestParseAction.SyntaxError:
                    case TestParseAction.CriticalError:
                    case TestParseAction.CommentError:
                        str = "There was a parse error.";
                        break;
                }
                checked { ++index; }
            }
            TestCurrentReportIndex = checked(TestGrammar.Report.Count - 1);
            frmTestGrammar.EndPopulateReport();
            if (flag)
            {
                frmTestGrammar.HighLightText((long)integer2, (long)integer1, (long)textLength);
                frmTestGrammar.SetMessage("Line: " + Conversions.ToString(integer2) + ", Column: " + Conversions.ToString(integer1));
            }
            else
                frmTestGrammar.ClearHighLight();
            if (TestGrammar.Accepted)
            {
                frmTestGrammar.SetMessage("Creating the parse tree...");
                frmTestGrammar.LoadReductionTree(TestGrammar.ParseTree);
                frmTestGrammar.SetMessage("The test string was successfully parsed.");
            }
            else if (Operators.CompareString(str, "", true) != 0)
                frmTestGrammar.SetMessage(str);
            if (!(TestGrammar.Complete | TestGrammar.Accepted))
                return;
            TestParseIsActive = false;
        }

        public static void TestAbort()
        {
            TestGrammar.Pause = true;
            TestGrammar.TestParser.Restart();
            TestDrawResults();
            TestParseIsActive = false;
            frmTestGrammar.SetMessage("Parse aborted");
            AllRefreshControls();
        }

        public static void TestPause()
        {
            TestGrammar.Pause = true;
        }

        internal static void TestParseAll()
        {
            if (!TestParseIsActive)
                TestStart();
            TestGrammar.Pause = false;
            AllRefreshControls();
            while (!(TestGrammar.Complete | TestGrammar.Pause))
            {
                TestGrammar.DoParseStep();
                frmTestGrammar.SetMessage("Parsing line: " + Conversions.ToString(TestGrammar.TestParser.CurrentPosition().Line));
            }
            TestDrawResults();
            if (TestGrammar.Complete)
                ShowTestCompleteWindow();
            AllRefreshControls();
        }

        internal static void TestStep()
        {
            if (!TestParseIsActive)
                TestStart();
            AllRefreshControls();
            TestGrammar.DoParseStep();
            TestDrawResults();
            if (TestGrammar.Complete)
                ShowTestCompleteWindow();
            AllRefreshControls();
        }

        private static void ShowTestCompleteWindow()
        {
            if (Operators.CompareString(ProgramSettings["Test", "Notify Test Done"].Value.ToUpper(), "TRUE", true) != 0)
                return;
            WindowMessage.Result result = !TestGrammar.Accepted ? MessageBox("Test Failed", "There was an lexical or syntax error in the test text.", WindowMessage.Style.Fail, "Ok", "", "Always show this message", true) : MessageBox("Test Success", "The Test Window successfully parsed the text.", WindowMessage.Style.Success, "Ok", "", "Always show this message", true);
            ProgramSettings["Test", "Notify Test Done"].Value = Conversions.ToString(result.Checked);
        }

        public static bool TestWindowOpen()
        {
            Form frmTestGrammar = (Form)frmTestGrammar;
            int num = IsWindowVisible(ref frmTestGrammar) ? 1 : 0;
            frmTestGrammar = (WindowTest)frmTestGrammar;
            return num != 0;
        }

        public static void RemoveClose(Form TheForm)
        {
            DeleteMenu(GetSystemMenu(TheForm.Handle, false), 61536, 0);
        }

        public static string CurrentTime()
        {
            return Microsoft.VisualBasic.Strings.Format((object)DateAndTime.Now, "hh:mm:ss tt");
        }

        public static bool CancelResetPrompt()
        {
            bool flag;
            switch (ApplicationState)
            {
                case States.GrammarReady:
                    GrammarModified = true;
                    UpdateMainWindowTitle();
                    flag = false;
                    break;
                case States.GrammarWorking:
                case States.LALRWorking:
                case States.DFAWorking:
                case States.Importing:
                    flag = true;
                    break;
                case States.GrammarFailed:
                case States.LALRFailed:
                case States.DFAFailed:
                    Reset();
                    GrammarModified = true;
                    UpdateMainWindowTitle();
                    flag = false;
                    break;
                case States.CGTLoaded:
                    flag = true;
                    break;
                default:
                    if (Operators.CompareString(ProgramSettings["Warn Edit Reset"].Value.ToUpper(), "TRUE", true) == 0)
                    {
                        WindowMessage.Result result = MessageBox("Reset?", "By editting the grammar, you will reset the project.\r\n\r\nDo you want to proceed?", WindowMessage.Style.Warning, "Yes", "No", "Always show this warning", true);
                        ProgramSettings["Warn Edit Reset"].Value = Conversions.ToString(result.Checked);
                        flag = !result.Value;
                    }
                    else
                        flag = false;
                    if (!flag)
                    {
                        Reset();
                        GrammarModified = true;
                        UpdateMainWindowTitle();
                    }
                    break;
            }
            return flag;
        }

        public static void DrawEtchTop(Graphics g, Rectangle Client)
        {
            Pen pen = new Pen(SystemColors.ControlDark);
            g.DrawLine(pen, Client.Left, Client.Top, checked(Client.Right - 1), Client.Top);
        }

        public static void DrawEtchTop(
          Graphics g,
          Rectangle Client,
          Color LightColor,
          Color ShadowColor)
        {
            Pen pen1 = new Pen(LightColor);
            Pen pen2 = new Pen(ShadowColor);
            g.DrawLine(pen2, Client.Left, Client.Top, checked(Client.Right - 1), Client.Top);
            g.DrawLine(pen1, Client.Left, checked(Client.Top + 1), checked(Client.Right - 1), checked(Client.Top + 1));
        }

        public static string ReformatText(string Source)
        {
            string[] strArray = Source.Split('\r');
            int num1 = 0;
            string str = "";
            long num2 = (long)checked(((IEnumerable<string>)strArray).Count<string>() - 1);
            long num3 = 0;
            while (num3 <= num2)
            {
                if (Operators.CompareString(strArray[checked((int)num3)].Trim(), "", true) == 0)
                {
                    str += "\r\n\r\n";
                }
                else
                {
                    str = str + strArray[checked((int)num3)].Trim() + " ";
                    num1 = 0;
                }
                checked { ++num3; }
            }
            return str;
        }

        public static void PrintGrammar()
        {
            string str = Operators.CompareString(ProgramSettings["Page Setup", "Scale"].Value, "cm", true) != 0 ? Conversions.ToString(100) : Conversions.ToString(39.37);
            int top = checked((int)Math.Round(unchecked(Conversion.Val(ProgramSettings["Page Setup", "Margin Top"].Value) * Conversions.ToDouble(str))));
            int left = checked((int)Math.Round(unchecked(Conversion.Val(ProgramSettings["Page Setup", "Margin Left"].Value) * Conversions.ToDouble(str))));
            int right = checked((int)Math.Round(unchecked(Conversion.Val(ProgramSettings["Page Setup", "Margin Right"].Value) * Conversions.ToDouble(str))));
            int bottom = checked((int)Math.Round(unchecked(Conversion.Val(ProgramSettings["Page Setup", "Margin Bottom"].Value) * Conversions.ToDouble(str))));
            TextPrinter.LineNumbering lineNumbering = Operators.CompareString(ProgramSettings["Page Setup", "Line Numbers"].Value.ToUpper(), "TRUE", true) != 0 ? TextPrinter.LineNumbering.None : TextPrinter.LineNumbering.Right;
            try
            {
                Printer.Document.DefaultPageSettings.Margins = new Margins(left, right, top, bottom);
                Printer.Font = GetSettingFont("Page Setup");
                Printer.LineNumbers = lineNumbering;
                Printer.LineNumberStart = 0;
                Printer.DocumentName = FileUtility.GetFileNameBase(ref GrammarFileName);
                TextPrinter printer = Printer;
                WindowGrammar frmGrammar = frmGrammar;
                string grammar = frmGrammar.Grammar;
                ref string local = ref grammar;
                printer.Print(ref local);
                frmGrammar.Grammar = grammar;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                MessageBox("Error Printing", Information.Err().Description, WindowMessage.Style.Error);
                SetStatusMessage("There was an error printing the document", "");
                Information.Err().Clear();
                ProjectData.ClearProjectError();
            }
        }

        public static Font GetFont(string Name, int Size, bool Bold, int MinSize = 6, int MaxSize = 32)
        {
            FontStyle style = !Bold ? FontStyle.Regular : FontStyle.Bold;
            if (Size < MinSize | Size < 4)
                Size = MinSize;
            else if (Size > MaxSize)
                Size = MaxSize;
            Font font;
            try
            {
                int index = 0;
                FontFamily family1 = FontFamily.GenericMonospace;
                bool flag = false;
                while (index < ((IEnumerable<FontFamily>)FontFamily.Families).Count<FontFamily>() & !flag)
                {
                    FontFamily family2 = FontFamily.Families[index];
                    if (Operators.CompareString(family2.Name.ToUpper(), Name.ToUpper(), true) == 0)
                    {
                        family1 = family2;
                        flag = true;
                    }
                    checked { ++index; }
                }
                font = !family1.IsStyleAvailable(style) ? new Font(family1, (float)Size) : new Font(family1, (float)Size, style);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Information.Err().Clear();
                font = new Font(FontFamily.GenericMonospace, (float)Size);
                ProjectData.ClearProjectError();
            }
            return font;
        }

        public static Font GetSettingFont(string Section)
        {
            bool Bold = Operators.CompareString(ProgramSettings[Section, "FontBold"].Value.ToUpper(), "TRUE", true) == 0;
            int Size = checked((int)Math.Round(Conversion.Val(ProgramSettings[Section, "FontSize"].Value)));
            return GetFont(ProgramSettings[Section, "FontName"].Value, Size, Bold, 6, 32);
        }

        public static bool LaunchWebsite(string URL)
        {
            bool flag;
            try
            {
                Process.Start(URL);
                flag = true;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                flag = false;
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        public static bool LaunchFile(string FileName)
        {
            bool flag;
            try
            {
                Process.Start(FileName);
                flag = true;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                flag = false;
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        public static bool SaveTestResults()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            SaveFileDialog saveFileDialog2 = saveFileDialog1;
            saveFileDialog2.AddExtension = true;
            saveFileDialog2.DefaultExt = "txt";
            saveFileDialog2.Filter = "Text Files|*.txt;*.text|All files|*.*";
            saveFileDialog2.Title = "Save Test File";
            bool flag;
            switch (saveFileDialog1.ShowDialog())
            {
                case DialogResult.OK:
                    flag = TestGrammar.SaveParseResults(saveFileDialog1.FileName, true);
                    if (flag)
                    {
                        SetStatusMessage("The test file was successfully saved", "");
                        break;
                    }
                    SetStatusMessage("There was an error saving the file", "");
                    MessageBox("File Error", "There was an error saving the file", WindowMessage.Style.Error);
                    break;
                case DialogResult.Cancel:
                    SetStatusMessage("Test file save was canceled", "");
                    flag = false;
                    break;
            }
            return flag;
        }

        private static bool SaveTestFile(string FilePath)
        {
            bool flag = frmTestGrammar.SaveFile(FilePath);
            if (flag)
            {
                SetStatusMessage("Test file was saved at " + CurrentTime(), "");
                TestFileName = FilePath;
                TestModified = false;
                frmTestGrammar.RefreshControls();
            }
            else
            {
                SetStatusMessage("There was an error saving the test file", "");
                MessageBox("File Save Error", "There was an error saving the test file", WindowMessage.Style.Error);
            }
            return flag;
        }

        public static bool SaveTestFileDialog()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            SaveFileDialog saveFileDialog2 = saveFileDialog1;
            saveFileDialog2.AddExtension = true;
            saveFileDialog2.DefaultExt = "txt";
            saveFileDialog2.Filter = "Text Files|*.txt;*.text|All files|*.*";
            saveFileDialog2.Title = "Save Test File...";
            if (Operators.CompareString(TestFileName, "", true) == 0)
            {
                saveFileDialog2.FileName = "";
                saveFileDialog2.InitialDirectory = ProgramSettings["Test", "Save Folder"].Value;
            }
            else
            {
                saveFileDialog2.InitialDirectory = FileUtility.GetPath(ref TestFileName);
                saveFileDialog2.FileName = FileUtility.GetFileName(ref TestFileName);
            }
            bool flag;
            switch (saveFileDialog1.ShowDialog())
            {
                case DialogResult.OK:
                    flag = SaveTestFile(saveFileDialog1.FileName);
                    TestFileName = saveFileDialog1.FileName;
                    Settings.Key programSetting = ProgramSettings["Test", "Save Folder"];
                    SaveFileDialog saveFileDialog3 = saveFileDialog1;
                    string fileName = saveFileDialog3.FileName;
                    string path = FileUtility.GetPath(ref fileName);
                    saveFileDialog3.FileName = fileName;
                    programSetting.Value = path;
                    break;
                case DialogResult.Cancel:
                    SetStatusMessage("Test file save was canceled", "");
                    flag = false;
                    break;
            }
            return flag;
        }

        public static bool SaveTestFileDialogIfNeeded()
        {
            return Operators.CompareString(TestFileName, "", true) != 0 ? SaveTestFile(TestFileName) : SaveTestFileDialog();
        }

        public static WindowMessage.Result MessageBox(string Title, string Message)
        {
            return MessageBox(Title, Message, WindowMessage.Style.Information, "Ok", "", "", false);
        }

        public static WindowMessage.Result MessageBox(
          string Title,
          string Message,
          WindowMessage.Style Icon)
        {
            return MessageBox(Title, Message, Icon, "Ok", "", "", false);
        }

        public static WindowMessage.Result MessageBox(
          string Title,
          string Message,
          WindowMessage.Style Icon,
          string TrueButton)
        {
            return MessageBox(Title, Message, Icon, TrueButton, "", "", false);
        }

        public static WindowMessage.Result MessageBox(
          string Title,
          string Message,
          WindowMessage.Style Icon,
          string TrueButton,
          string FalseButton)
        {
            return MessageBox(Title, Message, Icon, TrueButton, FalseButton, "", false);
        }

        public static WindowMessage.Result MessageBox(
          string Title,
          string Message,
          WindowMessage.Style Icon,
          string TrueButton,
          string FalseButton,
          string CheckText,
          bool CheckValue)
        {
            WindowMessage windowMessage = new WindowMessage();
            windowMessage.Show(Title, Message, Icon, TrueButton, FalseButton, CheckText, CheckValue);
            return windowMessage.MessageResult;
        }

        public static Color GetColor(string Section, string Key)
        {
            return Color.FromArgb(Colors.ToColorRGB((object)Section, (object)Key));
        }

        public interface IUpdateForm
        {
            void UpdateData();

            void RefreshControls();
        }

        public interface IClipboardForm
        {
            void Delete();

            void Paste();

            void Copy();

            void Cut();

            void SelectAll();

            bool CanUseClipboard();

            void Undo();

            void Redo();

            bool CanUndoRedo();
        }

        public interface IStyleForm
        {
            void RefreshStyles();
        }


        public enum FileType
        {
            CGT = 1,
            EGT = 2,
            Grammar = 3,
            Unsupported = 4,
        }


        public enum States
        {
            Boot,
            GrammarReady,
            GrammarWorking,
            GrammarFailed,
            LALRReady,
            LALRWorking,
            LALRFailed,
            DFAReady,
            DFAWorking,
            DFAFailed,
            TablesReady,
            TablesSaved,
            CGTLoaded,
            Importing,
        }


        public enum TableSaveFilterIndex
        {
            EGT = 1,
            CGT = 2,
            XML5 = 3,
            XML1 = 4,
        }
    }
}