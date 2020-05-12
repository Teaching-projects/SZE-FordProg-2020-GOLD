using System;

namespace GOLD.Engine
{
    public class ParserException : Exception
    {
        public string Method;

        internal ParserException(string message) : base(message)
        {
            Method = "";
        }

        internal ParserException(string message, Exception inner, string method) : base(message, inner)
        {
            Method = method;
        }
    }
}