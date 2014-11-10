using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DebugDiag.Native.Windbg
{
    /// <summary>
    /// Dumps the raw memory at a specified location in the size specific to the current dump architecture.
    /// On 32 bit dumps, this will dump the specified amount of dwords from the base address.
    /// On 64 bit dumps, this will dump the specified amount of qwords from the base address.
    /// </summary>
    public class Dp : Command, IEnumerable<ulong>
    {
        private readonly string _command;
        private readonly ulong _baseAddr;
        private readonly int _len;

        private ulong[] _memory;

        public Dp(ulong addr, int len)
        {
            _baseAddr = addr;
            _len = len;
            _command = String.Format("dp /c{1} 0x{0:x} L{1}", addr, len);
        }

        public ulong BytesAt(int offset)
        {
            if (!Executed) Execute();
            return _memory != null ? _memory[offset] : 0;
        }

        protected override string BuildCommand()
        {
            return _command;
        }

        protected override void Parse(string output)
        {
            output = output.TrimEnd();
            if (output.Contains("?")) throw new ArgumentException(String.Format("Invalid memory location at 0x{0:x}", _baseAddr));
            _memory = output.Split(' ').Where(m => !string.IsNullOrWhiteSpace(m)).Skip(1).Select(Native.StringAddrToUlong).ToArray();

            Debug.Assert(_memory.Length == _len, "`dp` parsed unexpected amount of data.");
        }

        #region IEnumerable

        public IEnumerator<ulong> GetEnumerator()
        {
            return ((IEnumerable<ulong>)_memory).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
