using System;
using System.Text.RegularExpressions;
using DebugDiag.DotNet;
using DebugDiag.Native.Type;

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
            // Register built-in user types. (Could be registered by reflection.)
            if (!_typesRegistered)
            {
                TypeParser.RegisterUserType(Vector.Syntax, new Vector(""));
                TypeParser.RegisterUserType(List.Syntax, new List(""));
                TypeParser.RegisterUserType(Map.Syntax, new Map(""));
                TypeParser.RegisterUserType(Set.Syntax, new Set(""));
                //TypeParser.RegisterUserType();
                _typesRegistered = true;
            }
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

        private static bool _typesRegistered;

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
        /// Converts a string address into the corresponding unsigned long.
        /// 
        /// This method can handle decimal addresses prefixed by 0n, as well as hexadecimal addresses. 
        /// Like Windbg, if there is no prefix, the address is assumed to be hexadecimal
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
                // Coerce negative values into ulong.
                return parse.StartsWith("-") ? (ulong)Convert.ToInt64(parse) : Convert.ToUInt64(parse, isHex ? 16 : 10);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception x)
            {
                throw new ArgumentException(String.Format("Cannot parse address `{0}`:\n{1}", addr, x), x);
            }
        }

        /// <summary>
        /// Parses a windbg primitive value type and attempts to extract the raw value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong? ParseWindbgPrimitive(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (value.Equals("(null)")) return 0;
            // HACK: This method will be removed once primitives are parsed as objects and no longer rely on RawMemory for their value.
            // In the mean time, split the value at the first space and take everything before that to prevent the "parser" from breaking
            // due to compound type support.
            try
            {
                return StringAddrToUlong(value.Split(' ')[0]); 
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
