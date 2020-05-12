using System;
using System.IO;

namespace GOLD.Engine
{
    internal class EGTReader
    {
        private const byte kRecordContentMulti = 77; // M

        private BinaryReader reader;
        private int entriesRead;

        private string header;
        public string Header
        {
            get
            {
                return header;
            }
        }

        private int entryCount;
        public short EntryCount
        {
            get 
            { 
                return (short)entryCount; 
            }
        }
        public bool IsReadComplete 
        { 
            get 
            { 
                return entriesRead >= entryCount; 
            } 
        }

        public bool IsEndOfFile
        {
            get
            {
                return reader.BaseStream.Position == reader.BaseStream.Length;
            }
        }

        public void Open(BinaryReader reader)
        {
            this.reader = reader;
            entryCount = 0;
            entriesRead = 0;
            header = RawReadCString();
        }

        public void Open(string path)
        {
            Open(new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)));
        }

        public void Dispose()
        {
            if (reader != null)
                reader.Dispose();
        }

        public Entry RetrieveEntry()
        {
            Entry result = new Entry();
            if (IsReadComplete)
            {
                result.Type = EntryType.Empty;
                result.Value = "";
            }
            else
            {
                entriesRead++;
                EntryType type = (EntryType)reader.ReadByte();
                result.Type = type;

                switch (type)
                {
                    case EntryType.Boolean:
                        byte b = reader.ReadByte();
                        result.Value = b == 1;
                        break;
                    case EntryType.Empty:
                        result.Value = "";
                        break;
                    case EntryType.UInt16:
                        result.Value = RawReadUInt16();
                        break;
                    case EntryType.String:
                        result.Value = RawReadCString();
                        break;
                    case EntryType.Byte:
                        result.Value = reader.ReadByte();
                        break;
                    default:
                        result.Type = EntryType.Error;
                        result.Value = "";
                        break;
                }
            }
            return result;
        }

        private ushort RawReadUInt16()
        {
            int b0 = reader.ReadByte();    //Least significant byte first
            int b1 = reader.ReadByte();

            int result = (b1 << 8) + b0;
            return (ushort)result;
        }

        private string RawReadCString()
        {
            string str = "";
            bool done = false;
            while (!done)
            {
                ushort char16 = RawReadUInt16();
                if (char16 == 0)
                    done = true;
                else
                    str += (char)char16;
            }
            return str;
        }

        public string RetrieveString()
        {
            Entry e = RetrieveEntry();
            if (e.Type == EntryType.String)
                return (string)e.Value;

            throw new IOException(e.Type, reader);
        }

        public short RetrieveInt16()
        {
            Entry e = RetrieveEntry();
            if (e.Type == EntryType.UInt16)
                return Convert.ToInt16(e.Value);

            throw new IOException(e.Type, reader);
        }

        public bool RetrieveBoolean()
        {
            Entry e = RetrieveEntry();
            if (e.Type == EntryType.Boolean)
                return (bool)e.Value;

            throw new IOException(e.Type, reader);
        }

        public byte RetrieveByte()
        {
            Entry e = RetrieveEntry();
            if (e.Type == EntryType.Byte)
                return (byte)e.Value;

            throw new IOException(e.Type, reader);
        }

        public bool GetNextRecord()
        {
            bool success;

            while (entriesRead < entryCount)
                RetrieveEntry();

            if (reader.ReadByte() == kRecordContentMulti)
            {
                entryCount = RawReadUInt16();
                entriesRead = 0;
                success = true;
            }
            else
                success = false;
            return success;
        }     
    }
}