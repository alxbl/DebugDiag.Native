using System;

namespace DebugDiag.Native.Windbg
{
    public class CommandException : Exception
    {
        public CommandException(string msg)
            : base(msg)
        {
        }

    public CommandException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}