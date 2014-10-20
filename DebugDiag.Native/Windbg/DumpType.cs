using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DebugDiag.Native.Windbg
{
    /// <summary>
    /// A helper class to quickly parse the output of Windbg's "dt" command.
    /// </summary>
    public class DumpType : IEnumerable<DumpType.Line>
    {
        private static readonly Regex LineFormat = new Regex(@" +(\+0x|=)([0-9a-fA-F]+) ([A-Za-z_][A-Za-z0-9_]+) +: +([^ \n\r]*)");
        /// <summary>
        /// Represents one line of output.
        /// </summary>
        public struct Line
        {
            public ulong Offset { get; internal set; }
            public string Name { get; internal set; }
            public string Value { get; internal set; }
            public string Type { get; internal set; }

            /// <summary>
            /// Whether this line represents a static field.
            /// </summary>
            public bool IsStatic { get; internal set; }

            /// <summary>
            /// Whether this is part of a bitfield's bit breakdown.
            /// </summary>
            public bool IsBits { get; internal set; }
        }

        /// <summary>
        /// The fully qualified typename. This is only populated when a type is not dumped with its fully qualified typename.
        /// 
        /// If the type was dumped with its fully qualified name, this property will be null.
        /// 
        /// e.g. dt nt!_PEB will report ntdll!_PEB as its full typename.
        /// </summary>
        public string TypeName { get; private set; }
        public bool RecursionDetected { get; private set; } // TODO: Allow for internal recursion when parsing dt.
        public bool IsVirtualType { get; private set; }
        private readonly IList<Line> _lines = new List<Line>();

        /// <summary>
        /// Disallow Construction
        /// </summary>
        private DumpType()
        {
            
        }

        /// <summary>
        /// Parses the Windbg output into an easy to access object.
        /// </summary>
        /// <param name="content">The output from the windbg "dt" command, as-is.</param>
        /// <returns>An instance of DumpType that can be enumerated for the offsets.</returns>
        public static DumpType Parse(string content)
        {
            var output = new DumpType {RecursionDetected = false}; // RecursionDetected is always false for now.

            var first = true;
            var checkVtable = true;
            
            foreach (var l in content.TrimEnd().Split('\n')) // Remove trailing whitespace.
            {
                if (first)
                {
                    // Check if we have a fully qualified type name.
                    output.TypeName = !l.Contains(":") ? l : null; 
                    first = false;
                    if (output.TypeName != null) continue; // We found a type name, skip this line.
                }

                // Determine whether this type is virtual (i.e. its first offset is a vtable)
                if (checkVtable)
                {
                    output.IsVirtualType = l.Contains("__VFN_table");
                    checkVtable = false;
                }

                var m = LineFormat.Matches(l);
                Debug.Assert(m.Count == 1, "LineFormat should match exactly once per line.");

                var groups = m[0].Groups;
                Debug.Assert(groups.Count == 5, "Could not match all required offset information");
                var offset = Convert.ToUInt64(groups[2].Value, 16);
                var fieldName = groups[3].Value;
                var value = groups[4].Value;
                output._lines.Add(new Line()
                           {
                               Offset = offset,
                               IsBits = value.Contains("0y"),
                               IsStatic = groups[1].Value == "=", // Windbg prefixes static fields with = instead of +
                               Name = fieldName,
                               Value = value.TrimEnd()
                           });
            }
            return output;
        }

        public IEnumerable<Line> GetOutput()
        {
            return _lines;
        }

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
    }
}
