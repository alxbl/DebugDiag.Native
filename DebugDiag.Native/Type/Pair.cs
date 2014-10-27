using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Analog representation of an std::pair.
    /// </summary>
    public class Pair : NativeType
    {
        public NativeType First { get; internal set; }
        public NativeType Second { get; internal set; }
    }
}
