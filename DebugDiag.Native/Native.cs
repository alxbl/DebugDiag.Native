using System;
using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DebugDiag.DotNet;

namespace DebugDiag.Native
{
    /// <summary>
    /// Makes working with native dumps a bit less painful.
    /// </summary>
    public static class Native
    {
        public static IDumpContext Context { get; private set; }
    
        /// <summary>
        /// Initializes the native library with the dump context.
        /// This can be called multiple times to change the context.
        /// </summary>
        /// <param name="context">The dump context that the native library will use.</param>
        public static void Initialize(IDumpContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Provides access to the DebugDiag script manager.
        /// </summary>
        public static NetScriptManager Manager
        {
            get
            {
                return Context.Manager;
            }
        }

        /// <summary>
        /// Provides access to the underlying debugger.
        /// </summary>
        public static NetDbgObj Debugger
        {
            get
            {
                return Context.Debugger;
            }
        }

        /// <summary>
        /// Provides access to the DebugDiag progress report.
        /// </summary>
        public static NetProgress Progress
        {
            get
            {
                return Context.Progress;
            }
        }

        /// <summary>
        /// Regular expression to match an address format. Currently only supports decimal or hexadecimal addresses.
        /// 
        /// Decimal format:
        ///   - Starts with 0n
        ///   - Contains one or more digit 0..9
        ///   - Leading zero is ok, because 0n is explicit.
        /// Hexadecimal format:
        ///   - Optional start with 0x
        ///   - Contains only hexadecimal digits 0..f
        ///   - Optional ` to divide octets
        ///   - Leading 0 is ok, because default is hexadecimal.
        /// </summary>
        public static readonly Regex AddressFormat = new Regex("(^0n[0-9]+$)|(^(0x)?([0-9a-fA-F]{0,7}|[0-9a-fA-F]{8}`?[0-9a-fA-F]{7})[0-9a-fA-F]{1}$)");

        public enum PrimitiveType
        {
            /// <summary>
            /// An object instance (not a primitive)
            /// </summary>
            Object,
            // Actual primitives
            Char,
            UChar,
            Int2B,
            Uint2B,
            Int4B,
            Uint4B,
            Int8B,
            Uint8B,
            Ptr32,
            Ptr64
        }

        /// <summary>
        /// Converts the string representation of a primitive type into an enumeration.
        /// </summary>
        /// <param name="v">The string representation of the type.</param>
        /// <returns>A PrimitiveType enumeration representing that type</returns>
        public static PrimitiveType TypeFromString(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) throw new ArgumentException("Type cannot be empty");

            // TODO: Handle Ptr32 Ptr32 (Object)
            if (v.Equals("Char")) return PrimitiveType.Char;
            if (v.Equals("UChar")) return PrimitiveType.UChar;
            if (v.Equals("Int2B")) return PrimitiveType.Int2B;
            if (v.Equals("Uint2B")) return PrimitiveType.Uint2B;
            if (v.Equals("Int4B")) return PrimitiveType.Int4B;
            if (v.Equals("Uint4B")) return PrimitiveType.Uint4B;
            if (v.Equals("Int8B")) return PrimitiveType.Int8B;
            if (v.Equals("Uint8B")) return PrimitiveType.Uint8B;
            if (v.StartsWith("Ptr32")) return PrimitiveType.Ptr32;
            if (v.StartsWith("Ptr64")) return PrimitiveType.Ptr64;
            return PrimitiveType.Object;
        }

        /// <summary>
        /// Converts a string address into the corresponding unsigned long.
        /// 
        /// This method can handle decimal addresses prefixed by 0n, as well as hexadecimal addresses. 
        /// Like Windbg, if there is no prefix, the address is assumed to be hexadecimal.
        /// </summary>
        /// <param name="addr">The string address.</param>
        /// <returns>The ulong representation of the address.</returns>
        /// <exception cref="ArgumentException">Thrown if the conversion fails.</exception>
        public static ulong StringAddrToUlong(string addr)
        {
            if (string.IsNullOrEmpty(addr)) throw new ArgumentException("Cannot parse null address.");

            string parse = addr;
            bool isHex = true;

            // Strip 0n or 0x
            if (addr.StartsWith("0n") && addr.Length > 2)
            {
                parse = addr.Substring(2);
                isHex = false;
            }
            else if (addr.StartsWith("0x") && addr.Length > 2)
            {
                parse = addr.Substring(2);
            }

            try
            {
                return Convert.ToUInt64(parse, isHex ? 16 : 10);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception x)
            {
                throw new ArgumentException(String.Format("Cannot parse address `{0}`. See inner exception.", addr), x);
            }
        }

        /// <summary>
        /// Parses a windbg primitive value type and attempts to extract the raw value.
        /// TODO: Private?
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong? ParseWindbgPrimitive(string value)
        {
            // TODO: Handle complex array/pointer types in conversion.
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (value.StartsWith("0x") || value.StartsWith("0n")) return StringAddrToUlong(value);
            if (value.Equals("(null)")) return 0;
            return null;
        }
    }
}
