using System;

namespace DebugDiag.Native.Windbg
{
    [Serializable]
    public class TypeDoesNotExistException : Exception
    {

        public TypeDoesNotExistException(string message)
            : base(message)
        {
        }

        public TypeDoesNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
