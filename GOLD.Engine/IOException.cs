using System;
using System.IO;

namespace GOLD.Engine
{
    public class IOException : Exception
    {
        public IOException(string message, Exception inner) : base(message, inner)
        {
        }

        public IOException(EntryType type, BinaryReader reader) : base("Type mismatch in file. Read '" + (char)type + "' at " + reader.BaseStream.Position.ToString())
        {
        }
    }
}
