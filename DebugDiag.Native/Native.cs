using System;
using System.Text.RegularExpressions;
using DebugDiag.DotNet;

namespace DebugDiag.Native
{
    /// <summary>
    /// Makes working with native dumps a bit less painful.
    /// </summary>
    public static class Native
    {
        private static DumpContext _context = null;
        

        /// <summary>
        /// Initializes the native library with the dump context.
        /// This can be called multiple times to change the context.
        /// </summary>
        /// <param name="context">The dump context that the native library will use.</param>
        public static void Initialize(DumpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Provides access to the DebugDiag script manager.
        /// </summary>
        public static NetScriptManager Manager
        {
            get
            {
                return _context.Manager;
            }
        }

        /// <summary>
        /// Provides access to the underlying debugger.
        /// </summary>
        public static NetDbgObj Debugger
        {
            get
            {
                return _context.Debugger;
            }
        }

        /// <summary>
        /// Provides access to the DebugDiag progress report.
        /// </summary>
        public static NetProgress Progress
        {
            get
            {
                return _context.Progress;
            }
        }

        public static readonly Regex AddressFormat = new Regex("(0x)?([0-9a-fA-F]{8})|([0-9a-fA-F]{8}`?[0-9a-fA-F]{8})");
    }
}
