using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GOLD.Builder
{

    internal sealed class BuilderUtility
    {
        public const short SW_SHOWNORMAL = 1;
        public const string kDoubleQuote = "\"";
        public const char MidDotChar = '\x0095';

        [DebuggerNonUserCode]
        static BuilderUtility()
        {
        }

        [DllImport("shell32.dll", EntryPoint = "ShellExecuteA", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int ShellExecute(
          int hwnd,
          [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpOperation,
          [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFile,
          [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpParameters,
          [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpDirectory,
          int nShowCmd);

        public static string CreateFileName(ref string Label, string AdditionalChars = "")
        {
            string str1 = "";
            string str2 = "\\/:*?<>|\"" + AdditionalChars;
            short num1 = checked((short)(Label.Length - 1));
            short num2 = 0;
            while ((int)num2 <= (int)num1)
            {
                string str3 = Conversions.ToString(Label[(int)num2]);
                if (str2.IndexOf(str3) == -1)
                    str1 += str3;
                checked { ++num2; }
            }
            return str1;
        }

        public static void OpenWebsite(int hwndOwner, string URL)
        {
            int hwnd = hwndOwner;
            string str1 = "Open";
            ref string local1 = ref str1;
            ref string local2 = ref URL;
            string str2 = Conversions.ToString(0);
            ref string local3 = ref str2;
            string str3 = Conversions.ToString(0);
            ref string local4 = ref str3;
            BuilderUtility.ShellExecute(hwnd, ref local1, ref local2, ref local3, ref local4, 1);
        }

        public static string HTMLText(string Text, bool EmptyToNBSP = true, bool SpaceToNBSP = false)
        {
            string str = "";
            if (Operators.CompareString(Text, "", true) == 0)
            {
                str = Conversions.ToString(Interaction.IIf(EmptyToNBSP, (object)"&nbsp;", (object)""));
            }
            else
            {
                short num1 = checked((short)(Text.Length - 1));
                short num2 = 0;
                while ((int)num2 <= (int)num1)
                {
                    str += BuilderUtility.HTMLChar((int)Text[(int)num2], SpaceToNBSP);
                    checked { ++num2; }
                }
            }
            return str;
        }

        public static string HTMLChar(int CharCode, bool SpaceToNBSP = false)
        {
            string str;
            switch (CharCode)
            {
                case 10:
                    str = "<br/>";
                    break;
                case 13:
                    str = "";
                    break;
                case 32:
                    str = Conversions.ToString(Interaction.IIf(SpaceToNBSP, (object)"&nbsp;", (object)" "));
                    break;
                case 34:
                    str = "&quot;";
                    break;
                case 38:
                    str = "&amp;";
                    break;
                case 60:
                    str = "&lt;";
                    break;
                case 62:
                    str = "&gt;";
                    break;
                case 149:
                    str = "&middot;";
                    break;
                case 160:
                    str = "&nbsp;";
                    break;
                default:
                    str = CharCode <= (int)byte.MaxValue ? Conversions.ToString(Strings.ChrW(CharCode)) : "&#" + Conversions.ToString(CharCode) + ";";
                    break;
            }
            return str;
        }

        public static string TimeDiffString(DateTime StartTime, DateTime EndTime)
        {
            return Conversions.ToString(Conversion.Int((double)DateAndTime.DateDiff(DateInterval.Minute, StartTime, EndTime, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1) / 60.0)) + " Hours " + Conversions.ToString(Conversion.Int(DateAndTime.DateDiff(DateInterval.Minute, StartTime, EndTime, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1) % 60L)) + " Minutes " + Conversions.ToString(Conversion.Int(DateAndTime.DateDiff(DateInterval.Second, StartTime, EndTime, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1) % 60L)) + " Seconds";
        }

        public static string RemoveNull(string Text)
        {
            string str = "";
            short num1 = checked((short)Strings.Len(Text));
            short num2 = 1;
            while ((int)num2 <= (int)num1)
            {
                string Left = Strings.Mid(Text, (int)num2, 1);
                if (Operators.CompareString(Left, "\0", true) != 0)
                    str += Left;
                checked { ++num2; }
            }
            return str;
        }

        public static string DisplayText(
          CharacterSet CharSet,
          bool ReplaceSpaceChar = true,
          int MaxSize = 1024,
          string OversizeMessage = "",
          short BreakWidth = -1)
        {
            string str = "";
            NumberRangeList numberRangeList = CharSet.RangeList();
            bool flag = true;
            int index1 = 0;
            while (index1 < numberRangeList.Count & flag)
            {
                if (!BuilderUtility.IsDisplayRange(numberRangeList[index1].First, numberRangeList[index1].Last))
                    flag = false;
                checked { ++index1; }
            }
            if (flag)
            {
                int num = checked(CharSet.Count() - 1);
                int index2 = 0;
                while (index2 <= num)
                {
                    str += BuilderUtility.DisplayChar(CharSet[index2], true);
                    checked { ++index2; }
                }
            }
            else
                str = BuilderUtility.DisplayRangeListText(CharSet.RangeList());
            return str;
        }

        private static bool IsDisplayRange(int First, int Last)
        {
            return First >= 32 & First <= (int)sbyte.MaxValue & Last >= 32 & Last <= (int)sbyte.MaxValue | First >= 160 & First <= (int)byte.MaxValue & Last >= 160 & Last <= (int)byte.MaxValue;
        }

        public static string DisplayRangeListText(NumberRangeList Ranges)
        {
            string str = BuilderUtility.DisplayRangeText(Ranges[0]);
            int num = checked(Ranges.Count - 1);
            int index = 1;
            while (index <= num)
            {
                NumberRange numberRange = Ranges[index];
                str = str + ", " + BuilderUtility.DisplayRangeText(Ranges[index]);
                numberRange = (NumberRange)null;
                checked { ++index; }
            }
            return str;
        }

        public static string DisplayRangeText(NumberRange Range)
        {
            NumberRange numberRange = Range;
            return numberRange.First != numberRange.Last ? (checked(numberRange.Last - numberRange.First) != 1 ? "&" + BuilderUtility.DisplayCodeText(numberRange.First) + " .. &" + BuilderUtility.DisplayCodeText(numberRange.Last) : "&" + BuilderUtility.DisplayCodeText(numberRange.First) + ", &" + BuilderUtility.DisplayCodeText(numberRange.Last)) : "&" + BuilderUtility.DisplayCodeText(numberRange.First);
        }

        private static string DisplayCodeText(int Codepoint)
        {
            string str = Conversion.Hex(Codepoint);
            if (str.Length % 2 == 1)
                str = "0" + str;
            return str;
        }

        public static string DisplayText(
          string Text,
          bool ReplaceSpaceChar = true,
          int MaxSize = 1024,
          string OversizeMessage = "",
          short BreakWidth = -1)
        {
            string str1;
            if (Text.Length > MaxSize)
            {
                str1 = Operators.CompareString(OversizeMessage, "", true) != 0 ? OversizeMessage : "The text is too large to view: " + Conversions.ToString(Text.Length) + " characters";
            }
            else
            {
                short num1 = 0;
                str1 = "";
                int num2 = checked(Text.Length - 1);
                int index = 0;
                while (index <= num2)
                {
                    string str2 = BuilderUtility.DisplayChar((int)Text[index], ReplaceSpaceChar);
                    if (BreakWidth != (short)-1)
                    {
                        checked { num1 += unchecked((short)str2.Length); }
                        if ((int)num1 > (int)BreakWidth)
                        {
                            str1 += " ";
                            num1 = (short)0;
                        }
                    }
                    str1 += str2;
                    checked { ++index; }
                }
            }
            return str1;
        }

        private static string DisplayChar(int CharCode, bool ReplaceSpaceChar)
        {
            string str;
            switch (CharCode)
            {
                case 9:
                    str = "{HT}";
                    break;
                case 10:
                    str = "{LF}";
                    break;
                case 11:
                    str = "{VT}";
                    break;
                case 12:
                    str = "{FF}";
                    break;
                case 13:
                    str = "{CR}";
                    break;
                case 32:
                    str = Conversions.ToString(Interaction.IIf(ReplaceSpaceChar, (object)"{Space}", (object)" "));
                    break;
                case 160:
                    str = "{NBSP}";
                    break;
                case 8364:
                    str = "{Euro Sign}";
                    break;
                default:
                    str = !(CharCode >= 32 & CharCode <= 126 | CharCode >= 160 & CharCode <= (int)byte.MaxValue) ? "{#" + Conversions.ToString(CharCode) + "}" : Conversions.ToString(Strings.ChrW(CharCode));
                    break;
            }
            return str;
        }

        public static bool IsAlphaNumeric(string Text)
        {
            bool flag = false;
            int index = 0;
            while (index < Text.Length & !flag)
            {
                if (!LikeOperator.LikeString(Conversions.ToString(Text[index]), "[a-zA-Z0-9_]", CompareMethod.Text))
                    flag = true;
                checked { ++index; }
            }
            return !flag;
        }

        public static string ToIdentifier(
          string Label,
          BuilderUtility.IDTypeCase TypeCase = BuilderUtility.IDTypeCase.Propercase,
          string ConvertSpaceChar = "",
          bool RemoveDashes = false)
        {
            string Left = Label.Trim();
            string Source;
            if (Operators.CompareString(Left, "", true) == 0)
            {
                Source = "";
            }
            else
            {
                string Text = Conversions.ToString(Left[0]);
                string str1;
                if (BuilderUtility.IsAlphaNumeric(Text))
                {
                    Source = Text;
                }
                else
                {
                    char TheChar = Conversions.ToChar(Text);
                    string str2 = BuilderUtility.CharNameShort(ref TheChar);
                    str1 = Conversions.ToString(TheChar);
                    Source = str2;
                }
                short num1 = checked((short)(Left.Length - 1));
                short num2 = 1;
                while ((int)num2 <= (int)num1)
                {
                    string str2 = Conversions.ToString(Left[(int)num2]);
                    if (BuilderUtility.IsAlphaNumeric(str2) | Operators.CompareString(str2, "_", true) == 0)
                        Source += str2;
                    else if (!(Operators.CompareString(str2, "-", true) == 0 & RemoveDashes))
                    {
                        if (Operators.CompareString(str2, " ", true) == 0)
                        {
                            Source += ConvertSpaceChar;
                        }
                        else
                        {
                            string str3 = Source;
                            char TheChar = Conversions.ToChar(str2);
                            string str4 = BuilderUtility.CharNameShort(ref TheChar);
                            str1 = Conversions.ToString(TheChar);
                            Source = str3 + str4;
                        }
                    }
                    checked { ++num2; }
                }
            }
            switch (TypeCase)
            {
                case BuilderUtility.IDTypeCase.Uppercase:
                    return Source.ToUpper();
                case BuilderUtility.IDTypeCase.Lowercase:
                    return Source.ToLower();
                case BuilderUtility.IDTypeCase.Propercase:
                    return BuilderUtility.IdentifierCase(ref Source, true);
                case BuilderUtility.IDTypeCase.Camelcase:
                    return BuilderUtility.IdentifierCase(ref Source, false);
                default:
                    return Source;
            }
        }

        public static string CharNameShort(ref char TheChar)
        {
            string str;
            switch ((char)((int)TheChar - 32))
            {
                case char.MinValue:
                    str = "Space";
                    break;
                case '\x0001':
                    str = "Exclam";
                    break;
                case '\x0002':
                    str = "Quote";
                    break;
                case '\x0003':
                    str = "Num";
                    break;
                case '\x0004':
                    str = "Dollar";
                    break;
                case '\x0005':
                    str = "Percent";
                    break;
                case '\x0006':
                    str = "Amp";
                    break;
                case '\a':
                    str = "Apost";
                    break;
                case '\b':
                    str = "LParen";
                    break;
                case '\t':
                    str = "RParen";
                    break;
                case '\n':
                    str = "Times";
                    break;
                case '\v':
                    str = "Plus";
                    break;
                case '\f':
                    str = "Comma";
                    break;
                case '\r':
                    str = "Minus";
                    break;
                case '\x000E':
                    str = "Dot";
                    break;
                case '\x000F':
                    str = "Div";
                    break;
                case '\x001A':
                    str = "Colon";
                    break;
                case '\x001B':
                    str = "Semi";
                    break;
                case '\x001C':
                    str = "Lt";
                    break;
                case '\x001D':
                    str = "Eq";
                    break;
                case '\x001E':
                    str = "Gt";
                    break;
                case '\x001F':
                    str = "Question";
                    break;
                case ' ':
                    str = "At";
                    break;
                case ';':
                    str = "LBracket";
                    break;
                case '<':
                    str = "Backslash";
                    break;
                case '=':
                    str = "RBracket";
                    break;
                case '>':
                    str = "Caret";
                    break;
                case '?':
                    str = "UScore";
                    break;
                case '@':
                    str = "Accent";
                    break;
                case '[':
                    str = "LBrace";
                    break;
                case '\\':
                    str = "Pipe";
                    break;
                case ']':
                    str = "RBrace";
                    break;
                case '^':
                    str = "Tilde";
                    break;
                default:
                    str = Conversions.ToString(TheChar);
                    break;
            }
            return str;
        }

        public static string XMLText(string Text)
        {
            string str = "";
            int num1 = checked(Text.Length - 1);
            int index = 0;
            while (index <= num1)
            {
                string String = Conversions.ToString(Text[index]);
                string Left = String;
                if (Operators.CompareString(Left, "<", true) == 0)
                    str += "&lt;";
                else if (Operators.CompareString(Left, ">", true) == 0)
                    str += "&gt;";
                else if (Operators.CompareString(Left, "&", true) == 0)
                    str += "&amp;";
                else if (Operators.CompareString(Left, "\"", true) == 0)
                {
                    str += "&quot;";
                }
                else
                {
                    int num2 = Strings.AscW(String);
                    str = !(num2 >= 32 & num2 <= 126) ? str + "&#" + Conversions.ToString(num2) + ";" : str + String;
                }
                checked { ++index; }
            }
            return str;
        }

        private static string IdentifierCase(ref string Source, bool CapitializeFirst)
        {
            bool flag = CapitializeFirst;
            string str = "";
            short num1 = checked((short)(Source.Length - 1));
            short num2 = 0;
            while ((int)num2 <= (int)num1)
            {
                string Left = Conversions.ToString(Source[(int)num2]);
                if (Operators.CompareString(Left, " ", true) == 0)
                {
                    str += " ";
                    flag = true;
                }
                else if (flag)
                {
                    str += Left.ToUpper();
                    flag = false;
                }
                else
                    str += Left.ToLower();
                checked { ++num2; }
            }
            return str;
        }

        public static string CleanString(string Text, bool ReplaceSpaceChar = true)
        {
            bool flag1 = ReplaceSpaceChar;
            if (!flag1)
            {
                bool flag2 = false;
                long num = 1;
                while (!(num > (long)Strings.Len(Text) | flag2))
                {
                    if (Operators.CompareString(Strings.Mid(Text, checked((int)num), 1), " ", true) != 0)
                        flag2 = true;
                    checked { ++num; }
                }
                if (!flag2)
                    flag1 = true;
            }
            string str1 = "";
            long num1 = (long)Strings.Len(Text);
            long num2 = 1;
            while (num2 <= num1)
            {
                string String = Strings.Mid(Text, checked((int)num2), 1);
                long num3 = (long)Strings.AscW(String);
                string str2;
                switch (num3)
                {
                    case 9:
                        str2 = "{HT}";
                        break;
                    case 10:
                        str2 = "{LF}";
                        break;
                    case 11:
                        str2 = "{VT}";
                        break;
                    case 12:
                        str2 = "{FF}";
                        break;
                    case 13:
                        str2 = "{CR}";
                        break;
                    case 32:
                        str2 = !flag1 ? " " : "{Space}";
                        break;
                    case 160:
                        str2 = "{NBSP}";
                        break;
                    case 8364:
                        str2 = "{Euro Sign}";
                        break;
                    default:
                        str2 = !(num3 >= 32L & num3 <= 126L | num3 >= 160L & num3 <= (long)byte.MaxValue) ? "{#" + Conversions.ToString(num3) + "}" : String;
                        break;
                }
                str1 += str2;
                checked { ++num2; }
            }
            return str1;
        }


        public enum IDTypeCase
        {
            AsIs,
            Uppercase,
            Lowercase,
            Propercase,
            Camelcase,
        }
    }
}