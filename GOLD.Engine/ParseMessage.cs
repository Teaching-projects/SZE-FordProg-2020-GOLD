namespace GOLD.Engine
{
    public enum ParseMessage
    {
        TokenRead,          // A new token is read
        Reduction,          // A production is reduced
        Accept,             // Grammar complete
        NotLoadedError,     // The tables are not loaded
        LexicalError,       // Token not recognized
        SyntaxError,        // Token is not expected
        GroupError,         // Reached the end of the file inside a block
        InternalError,      // Something is wrong, very wrong
    }
}