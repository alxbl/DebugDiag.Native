using System;
using DebugDiag.Native.Windbg;

namespace DebugDiag.Native.Type
{
    /// <summary>
    /// Represents a primitive value type.
    /// </summary>
    public class Primitive : NativeType
    {
        private ulong _value;
        
        public Primitive(string typename, ulong value) :
            base(typename)
        {
            _value = value;
        }

        #region NativeType overrides
        
        public override NativeType GetField(string field)
        {
            throw new InvalidOperationException("Cannot call GetField() on a primitive type.");
        }

        public override NativeType GetField(ulong offset)
        {
            throw new InvalidOperationException("Cannot call GetField() on a primitive type.");
        }

        public override ulong GetIntValue()
        {
            return _value;
        }

        #endregion

        internal static NativeType CreatePrimitive(string typename, DumpType.Line? dt, bool isInstance)
        {
            if (!isInstance || !dt.HasValue) return new Primitive(typename, 0); // No need to do extra parsing for non-instances.

            NativeType type;
            if (typename.Equals("std::string") || typename.Equals("std::wstring"))
                type = new String(typename, 0); // TODO: Handle string properly.
            else
            type = new Primitive(typename, ParseWindbgPrimitive(dt.Value.Detail).GetValueOrDefault());

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
