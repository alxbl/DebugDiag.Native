using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Windbg
{
    /// <summary>
    /// A helper class that abstracts dumping out a string from a dump.
    /// This class is only available internally since strings are treated as primitives and handled fully by DebugDiag.Native.
    /// </summary>
    internal class DumpString : Command
    {
        private readonly ulong _addr;
        private readonly bool _isWide, _dereference;
        private string _cache;

        /// <summary>
        /// Creates a DumpString command to be run against the dump context.
        /// 
        /// When dereference is true, the address passed must be the address of a pointer to the string.
        /// This is useful when dumping STL strings which are too big to fit in their internal buffer.
        /// </summary>
        /// <param name="addr">The address at which the first character of the string is located.</param>
        /// <param name="isWide">Whether the string is wide.</param>
        /// <param name="dereference">Whether it is necessary to dereference the pointer.</param>
        public DumpString(ulong addr, bool isWide, bool dereference)
        {
            _addr = addr;
            _isWide = isWide;
            _dereference = dereference;
        }

        protected override string BuildCommand()
        {
            if (string.IsNullOrEmpty(_cache))
            {
                // Using .printf ensures that we print the string escape codes as well.
                // Narrow: .printf "%ma", <addr> 
                // Wide:   .printf "%mu", <addr>
                _cache = _dereference ? String.Format(".printf \"{0}\", poi(0x{1:x})", _isWide ? "%mu" : "%ma", _addr) // poi(address)
                    : String.Format(".printf \"{0}\", 0x{1:x}", _isWide ? "%mu" : "%ma", _addr);
            }
            return _cache;
        }

        protected override void Parse(string output)
        {
            // No parsing to do.
        }
    }
}
