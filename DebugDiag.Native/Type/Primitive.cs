using System;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a primitive value type.
    /// </summary>
    public abstract class Primitive : NativeType
    {
        #region Constructor
        protected Primitive(string typename) :
            base(typename)
        {
        }

        internal Primitive(NativeType other)
            : base(other)
        {
        }
        #endregion

        internal static NativeType CreatePrimitive(string typename, DumpType.Line? dt, bool isInstance)
        {
            NativeType type;
            if (String.Syntax.IsMatch(typename))
            {
                type = new String(typename);
            }
            else
            {
                type = (!isInstance || !dt.HasValue) 
                    ? new Integer(typename, 0) 
                    : new Integer(typename, ParseWindbgPrimitive(dt.Value.Detail).GetValueOrDefault());
            }

            return type;
        }

        /// <summary>
        /// Parses a windbg primitive value type and attempts to extract the raw value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static ulong? ParseWindbgPrimitive(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (value.Equals("(null)")) return 0;
            // HACK: This method will be removed once primitives are parsed as objects and no longer rely on RawMemory for their value.
            // In the mean time, split the value at the first space and take everything before that to prevent the "parser" from breaking
            // due to compound type support.
            try
            {
                return Native.StringAddrToUlong(value.Split(' ')[0]);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
