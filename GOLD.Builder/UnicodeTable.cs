using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Reflection;

namespace GOLD.Builder
{

    internal sealed class UnicodeTable
    {
        private static UnicodeMapTable m_UpperCaseTable = new UnicodeMapTable();
        private static UnicodeMapTable m_LowerCaseTable = new UnicodeMapTable();
        private static UnicodeMapTable m_Win1252Table = new UnicodeMapTable();

        public static void Setup()
        {
            UnicodeTable.LoadMapping();
        }

        private static void AddCase(int UppercaseCode, int LowercaseCode, string Name)
        {
            UnicodeTable.m_LowerCaseTable.Add(LowercaseCode, UppercaseCode);
            UnicodeTable.m_UpperCaseTable.Add(UppercaseCode, LowercaseCode);
        }

        private static void AddWin1252(int CharCode, int Mapping)
        {
            UnicodeTable.m_Win1252Table.Add(CharCode, Mapping);
            UnicodeTable.m_Win1252Table.Add(Mapping, CharCode);
        }

        public static bool IsWin1252(ref int CharCode)
        {
            return (uint)UnicodeTable.m_Win1252Table.Contains(CharCode) > 0U || CharCode >= 32 & CharCode <= 126 | CharCode >= 160 & CharCode <= (int)byte.MaxValue;
        }

        public static string GetPath(ref string FullPath)
        {
            short num = checked((short)FullPath.LastIndexOf("\\"));
            if (num == (short)-1)
                return "";
            return FullPath.Substring(0, (int)num);
        }

        private static void LoadMapping()
        {
            SimpleDB.Reader reader = new SimpleDB.Reader();
            try
            {
                string location = Assembly.GetExecutingAssembly().Location;
                reader.Open(GetPath(ref location) + "\\mapping.dat");
                if (Operators.CompareString(reader.Header(), "GOLD Character Mapping", true) != 0)
                {
                    BuilderApp.Log.Add(SysLogSection.Internal, SysLogAlert.Critical, "The file 'mapping.dat' is invalid.");
                }
                else
                {
                    while (!reader.EndOfFile())
                    {
                        reader.GetNextRecord();
                        string str = reader.RetrieveString();
                        int num1 = reader.RetrieveInt16();
                        int num2 = reader.RetrieveInt16();
                        string Left = str;
                        if (Operators.CompareString(Left, "C", true) == 0)
                            UnicodeTable.AddCase(num1, num2, "");
                        else if (Operators.CompareString(Left, "W", true) == 0)
                            UnicodeTable.AddWin1252(num1, num2);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                BuilderApp.Log.Add(SysLogSection.Internal, SysLogAlert.Critical, exception.Message);
                ProjectData.ClearProjectError();
            }
        }

        public static int ToUpperCase(ref int CharCode)
        {
            int Index = UnicodeTable.m_LowerCaseTable.IndexOf(CharCode);
            if (Index == -1)
                return CharCode;
            return UnicodeTable.m_LowerCaseTable.get_Item(Index).Map;
        }

        public static int ToLowerCase(ref int CharCode)
        {
            int Index = UnicodeTable.m_UpperCaseTable.IndexOf(CharCode);
            if (Index == -1)
                return CharCode;
            return UnicodeTable.m_UpperCaseTable.get_Item(Index).Map;
        }

        public static int ToWin1252(ref int CharCode)
        {
            int Index = UnicodeTable.m_Win1252Table.IndexOf(CharCode);
            if (Index == -1)
                return CharCode;
            return UnicodeTable.m_Win1252Table.get_Item(Index).Map;
        }
    }
}