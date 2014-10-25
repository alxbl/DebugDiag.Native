using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a primitive value type.
    /// </summary>
    public class Primitive : NativeType
    {
        public ulong Value { get; internal set; }
    }
}
