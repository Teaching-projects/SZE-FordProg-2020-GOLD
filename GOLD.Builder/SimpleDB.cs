using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace GOLD.Builder
{

    internal sealed class SimpleDB
    {
        private const short kRecordContentMulti = 77;

        [DebuggerNonUserCode]
        static SimpleDB()
        {
        }


        public enum EntryType : byte
        {
            Error = 0,
            Boolean = 66, // 0x42
            Empty = 69, // 0x45
            UInt16 = 73, // 0x49
            String = 83, // 0x53
            Byte = 98, // 0x62
        }


        public class IOException : Exception
        {
            public IOException(string Message)
              : base(Message)
            {
            }

            public IOException(string Message, Exception Inner)
              : base(Message, Inner)
            {
            }

            public IOException(SimpleDB.EntryType Type, BinaryReader Reader)
              : base("Type mismatch in file. Read '" + Conversions.ToString(Strings.ChrW((int)Type)) + "' at " + Conversions.ToString(Reader.BaseStream.Position))
            {
            }
        }


        public class Entry
        {
            public SimpleDB.EntryType Type;
            public object Value;

            public Entry()
            {
                this.Type = SimpleDB.EntryType.Empty;
                this.Value = (object)"";
            }

            public Entry(SimpleDB.EntryType Type, object Value)
            {
                this.Type = Type;
                this.Value = RuntimeHelpers.GetObjectValue(Value);
            }
        }


        public class Reader
        {
            private const byte kRecordContentMulti = 77;
            private string m_FileHeader;
            private BinaryReader m_Reader;
            private int m_EntryCount;
            private int m_EntriesRead;

            [DebuggerNonUserCode]
            public Reader()
            {
            }

            public bool RecordComplete()
            {
                return this.m_EntriesRead >= this.m_EntryCount;
            }

            public void Close()
            {
                if (this.m_Reader == null)
                    return;
                this.m_Reader.Close();
                this.m_Reader = (BinaryReader)null;
            }

            public short EntryCount()
            {
                return checked((short)this.m_EntryCount);
            }

            public bool EndOfFile()
            {
                return this.m_Reader.BaseStream.Position == this.m_Reader.BaseStream.Length;
            }

            public string Header()
            {
                return this.m_FileHeader;
            }

            public void Open(BinaryReader Reader)
            {
                this.m_Reader = Reader;
                this.m_EntryCount = 0;
                this.m_EntriesRead = 0;
                this.m_FileHeader = this.RawReadCString();
            }

            public void Open(string Path)
            {
                try
                {
                    this.Open(new BinaryReader((Stream)File.Open(Path, FileMode.Open, FileAccess.Read)));
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    throw ex;
                }
            }

            public SimpleDB.Entry RetrieveEntry()
            {
                SimpleDB.Entry entry = new SimpleDB.Entry();
                if (this.RecordComplete())
                    throw new SimpleDB.IOException("Read past end of record at " + Conversions.ToString(this.m_Reader.BaseStream.Position));
                checked { ++this.m_EntriesRead; }
                byte num1 = this.m_Reader.ReadByte();
                entry.Type = (SimpleDB.EntryType)num1;
                switch (num1)
                {
                    case 66:
                        byte num2 = this.m_Reader.ReadByte();
                        entry.Value = (object)(num2 == (byte)1);
                        break;
                    case 69:
                        entry.Value = (object)"";
                        break;
                    case 73:
                        entry.Value = (object)this.RawReadUInt16();
                        break;
                    case 83:
                        entry.Value = (object)this.RawReadCString();
                        break;
                    case 98:
                        entry.Value = (object)this.m_Reader.ReadByte();
                        break;
                    default:
                        entry.Type = SimpleDB.EntryType.Error;
                        entry.Value = (object)"";
                        break;
                }
                return entry;
            }

            private ushort RawReadUInt16()
            {
                return checked((ushort)(((int)this.m_Reader.ReadByte() << 8) + (int)this.m_Reader.ReadByte()));
            }

            private string RawReadCString()
            {
                string str = "";
                bool flag = false;
                while (!flag)
                {
                    ushort num = this.RawReadUInt16();
                    if (num == (ushort)0)
                        flag = true;
                    else
                        str += Conversions.ToString(Strings.ChrW((int)num));
                }
                return str;
            }

            public string RetrieveString()
            {
                SimpleDB.Entry entry = this.RetrieveEntry();
                if (entry.Type == SimpleDB.EntryType.String)
                    return Conversions.ToString(entry.Value);
                throw new SimpleDB.IOException(entry.Type, this.m_Reader);
            }

            public int RetrieveInt16()
            {
                SimpleDB.Entry entry = this.RetrieveEntry();
                if (entry.Type == SimpleDB.EntryType.UInt16)
                    return Conversions.ToInteger(entry.Value);
                throw new SimpleDB.IOException(entry.Type, this.m_Reader);
            }

            public bool RetrieveBoolean()
            {
                SimpleDB.Entry entry = this.RetrieveEntry();
                if (entry.Type == SimpleDB.EntryType.Boolean)
                    return Conversions.ToBoolean(entry.Value);
                throw new SimpleDB.IOException(entry.Type, this.m_Reader);
            }

            public byte RetrieveByte()
            {
                SimpleDB.Entry entry = this.RetrieveEntry();
                if (entry.Type == SimpleDB.EntryType.Byte)
                    return Conversions.ToByte(entry.Value);
                throw new SimpleDB.IOException(entry.Type, this.m_Reader);
            }

            public bool GetNextRecord()
            {
                while (this.m_EntriesRead < this.m_EntryCount)
                    this.RetrieveEntry();
                bool flag;
                if (this.m_Reader.ReadByte() == (byte)77)
                {
                    this.m_EntryCount = (int)this.RawReadUInt16();
                    this.m_EntriesRead = 0;
                    flag = true;
                }
                else
                    flag = false;
                return flag;
            }

            ~Reader()
            {
                this.Close();
            }
        }


        public class EntryList : ArrayList
        {
            [DebuggerNonUserCode]
            public EntryList()
            {
            }

            public SimpleDB.Entry this[int Index]
            {
                get
                {
                    return (SimpleDB.Entry)base[Index];
                }
                set
                {
                    this[Index] = value;
                }
            }

            public int Add(SimpleDB.Entry Value)
            {
                return this.Add((object)Value);
            }
        }


        public class Writer
        {
            private FileStream m_File;
            private BinaryWriter m_Writer;
            private SimpleDB.EntryList m_CurrentRecord;
            private string m_ErrorDescription;

            public Writer()
            {
                this.m_CurrentRecord = new SimpleDB.EntryList();
            }

            public string ErrorDescription()
            {
                return this.m_ErrorDescription;
            }

            public void Close()
            {
                this.WriteRecord();
                this.m_File.Close();
            }

            public void Open(ref string Path, string Header)
            {
                try
                {
                    this.m_File = new FileStream(Path, FileMode.Create);
                    this.m_Writer = new BinaryWriter((Stream)this.m_File);
                    this.RawWriteCString(ref Header);
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    throw new SimpleDB.IOException("Could not open file", ex);
                }
            }

            public void StoreEmpty()
            {
                this.m_CurrentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.Empty, (object)""));
            }

            public void StoreBoolean(bool Value)
            {
                this.m_CurrentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.Boolean, (object)Value));
            }

            public void StoreInt16(int Value)
            {
                this.m_CurrentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.UInt16, (object)Value));
            }

            public void StoreByte(byte Value)
            {
                this.m_CurrentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.Byte, (object)Value));
            }

            public void StoreString(ref string Value)
            {
                this.m_CurrentRecord.Add(new SimpleDB.Entry(SimpleDB.EntryType.String, (object)Value));
            }

            private void RawWriteCString(ref string Text)
            {
                int num = checked(Text.Length - 1);
                int index = 0;
                while (index <= num)
                {
                    this.RawWriteInt16((int)Text[index]);
                    checked { ++index; }
                }
                this.RawWriteInt16(0);
            }

            private void RawWriteInt16(int Value)
            {
                byte num1 = checked((byte)(Value & (int)byte.MaxValue));
                byte num2 = checked((byte)(Value >> 8 & (int)byte.MaxValue));
                this.m_Writer.Write(num1);
                this.m_Writer.Write(num2);
            }

            private void RawWriteInt32(int Value)
            {
                byte num1 = checked((byte)(Value & (int)byte.MaxValue));
                byte num2 = checked((byte)(Value >> 8 & (int)byte.MaxValue));
                num2 = checked((byte)(Value >> 16 & (int)byte.MaxValue));
                byte num3 = checked((byte)(Value >> 24 & (int)byte.MaxValue));
                this.m_Writer.Write(num1);
                this.m_Writer.Write(num3);
                byte num4=0;
                this.m_Writer.Write(num4);
                byte num5=0;
                this.m_Writer.Write(num5);
            }

            private void RawWriteByte(byte Value)
            {
                this.m_Writer.Write(Value);
            }

            public void NewRecord()
            {
                this.WriteRecord();
            }

            private void WriteRecord()
            {
                if (this.m_CurrentRecord.Count < 1)
                    return;
                this.RawWriteByte((byte)77);
                this.RawWriteInt16(this.m_CurrentRecord.Count);
                int num = checked(this.m_CurrentRecord.Count - 1);
                int index = 0;
                while (index <= num)
                {
                    SimpleDB.Entry entry1 = this.m_CurrentRecord[index];
                    switch (entry1.Type)
                    {
                        case SimpleDB.EntryType.Boolean:
                            this.RawWriteByte((byte)66);
                            this.RawWriteByte(Conversions.ToByte(Interaction.IIf(Conversions.ToBoolean(entry1.Value), (object)1, (object)0)));
                            break;
                        case SimpleDB.EntryType.UInt16:
                            this.RawWriteByte((byte)73);
                            this.RawWriteInt16(Conversions.ToInteger(entry1.Value));
                            break;
                        case SimpleDB.EntryType.String:
                            this.RawWriteByte((byte)83);
                            SimpleDB.Entry entry2 = entry1;
                            string Text = Conversions.ToString(entry2.Value);
                            this.RawWriteCString(ref Text);
                            entry2.Value = (object)Text;
                            break;
                        case SimpleDB.EntryType.Byte:
                            this.RawWriteByte((byte)98);
                            this.RawWriteByte(Conversions.ToByte(entry1.Value));
                            break;
                        default:
                            this.RawWriteByte((byte)69);
                            break;
                    }
                    checked { ++index; }
                }
                this.m_CurrentRecord.Clear();
            }

            ~Writer()
            {
                this.Close();
            }
        }
    }
}