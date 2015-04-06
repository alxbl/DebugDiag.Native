using System;
using System.IO;
using System.Text.RegularExpressions;
using DebugDiag.DotNet;
using DebugDiag.Native.Type;
using DebugDiag.Native.Windbg;

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
                Parser.RegisterUserType(Vector.Syntax, typeof(Vector));
                Parser.RegisterUserType(List.Syntax, typeof(List));
                Parser.RegisterUserType(Map.Syntax, typeof(Map));
                Parser.RegisterUserType(Set.Syntax, typeof(Set));
                //Parser.RegisterUserType();
                _typesRegistered = true;
            }

            // Load native extensions into the dump context.
            new Load(Context.Is32Bit ? "NDbgExt.dll" : "NDbgExt64.dll").Execute();
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
                if (parse.Contains("`")) parse = parse.Remove(parse.IndexOf('`'), 1); // Handle long.

                // Coerce negative values into ulong.
                return parse.StartsWith("-") ? (ulong)Convert.ToInt64(parse) : Convert.ToUInt64(parse, isHex ? 16 : 10);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception x)
            {
                throw new ArgumentException(string.Format("Cannot parse address `{0}`:\n{1}", addr, x), x);
            }
        }
    }
}
