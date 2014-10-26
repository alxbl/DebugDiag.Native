using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Windbg
{
    [Serializable]
    public class TypeDoesNotExistException : Exception
    {

        public TypeDoesNotExistException(string message) : base(message)
        {
        }

        public TypeDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
