using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace DebugDiag.Native.Windbg
{
    /// <summary>
    /// A helper class to quickly parse the output of Windbg's `dt` command.
    /// </summary>
    public class DumpType : Command, IEnumerable<DumpType.Line>
    {
        #region Constructors

        /// <summary>
        /// Dumps type information about a type.
        /// </summary>
        /// <param name="type">The typename to dump.</param>
        public DumpType(string type)
            : this()
        {
            IsInstance = false;
            _command = String.Format("dt 0 {0}", type);
            _type = type;
        }

        /// <summary>
        /// Dumps information about a type instances.
        /// </summary>
        /// <param name="type">The typename to dump.</param>
        /// <param name="addr">The base address of the instance.</param>
        public DumpType(string type, ulong addr)
            : this()
        {
            IsInstance = true;
            _command = String.Format("dt 0x{0:x} {1}", addr, type);
            _type = type;
        }

        private DumpType()
        {
            IsRecursive = false; // TODO: Allow for internal recursion when parsing dt if it ever becomes a performance issue.
        }

        #endregion
        #region Public API

        /// <summary>
        /// The fully qualified typename. This is only populated when a type is not dumped with its fully qualified typename.
        /// 
        /// If the type was dumped with its fully qualified name, this property will be null.
        /// 
        /// e.g. dt nt!_PEB will report ntdll!_PEB as its full typename.
        /// </summary>
        public string TypeName { get; private set; }
        public bool IsRecursive { get; private set; }
        public bool IsVirtualType { get; private set; }
        public bool IsInstance { get; private set; }

        #region IEnumerable

        public IEnumerator<Line> GetEnumerator()
        {
            return _lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
        #endregion
        #region Private

        private static readonly Regex LineFormat = new Regex(@"\s*(\+0x|=)([0-9a-fA-F]+) ([A-Za-z_][A-Za-z0-9_]+) +: +([^\n\r]*)");
        private static readonly Regex BitfieldFormat = new Regex(@"Pos (\d+), (\d+) Bits?");
        private readonly IList<Line> _lines = new List<Line>();
        private readonly string _command; // The windbg command to run.
        private readonly string _type; // cache requested type for error messages;

        /// <summary>
        /// Represents one line of output.
        /// </summary>
        public struct Line
        {
            public ulong Offset { get; internal set; }
            public string Name { get; internal set; }
            public string Detail { get; internal set; }
            /// <summary>
            /// Whether this line represents a static field.
            /// </summary>
            public bool IsStatic { get; internal set; }

            /// <summary>
            /// Whether this is part of a bit field's bit breakdown.
            /// </summary>
            public bool IsBits { get; internal set; }
        }

        #endregion
        #region Command Implementation

        protected override string BuildCommand()
        {
            return _command;
        }

        /// <summary>
        /// Parses the Windbg output into an easy to access object.
        /// </summary>
        /// <param name="content">The output from the windbg "dt" command, as-is.</param>
        /// <returns>An instance of DumpType that can be enumerated for the offsets.</returns>
        protected override void Parse(string content)
        {
            var notFound = String.Format("Symbol {0} not found.", _type);
            if (string.IsNullOrEmpty(content) || content.Contains(notFound)) 
                throw new TypeDoesNotExistException(notFound);

            var first = true;
            var checkVtable = true;

            foreach (var line in content.TrimEnd().Split('\n').Select(l => l.TrimEnd()))
            {
                if (first)
                {
                    // Check if we have a fully qualified type name.
                    TypeName = line.Contains("!") ? line : null;
                    first = false;
                    if (TypeName != null) continue; // We found a type name, skip this line.
                }

                // Determine whether this type is virtual (i.e. its first offset is a vtable)
                if (checkVtable)
                {
                    IsVirtualType = line.Contains("__VFN_table");
                    checkVtable = false;
                }

                var m = LineFormat.Matches(line);

                // In some cases, `dt` will output formatting for the type. Skip those lines,
                // they can be parsed externally if the caller really wants them.
                // e.g. ntdll!_GUID will always output the formatted GUID. 
                if (m.Count != 1) continue; 

                var groups = m[0].Groups;
                Debug.Assert(groups.Count == 5, "Could not match all required offset information");
                var offset = Convert.ToUInt64(groups[2].Value, 16);
                var fieldName = groups[3].Value;
                var value = groups[4].Value;
                _lines.Add(new Line
                           {
                               Offset = offset,
                               IsBits = BitfieldFormat.IsMatch(value) || value.Contains("0y"), // TODO: Extract bitfield info.
                               IsStatic = groups[1].Value == "=", // Windbg prefixes static fields with = instead of +
                               Name = fieldName,
                               Detail = value.TrimEnd()
                           });
            }
        }

        #endregion
    }
}
