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
        #region NativeType overrides

        public override NativeType GetField(string field)
        {
            throw new InvalidOperationException("Cannot call GetField() on a primitive type.");
        }

        public override NativeType GetField(ulong offset)
        {
            throw new InvalidOperationException("Cannot call GetField() on a primitive type.");
        }
        #endregion
        public ulong Value { get; internal set; }
    }
}
