using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DebugDiag.Native.Windbg
{
    /// <summary>
    /// A helper class to quickly parse the output of Windbg's "dt" command.
    /// </summary>
    public class DumpType
    {
        private static readonly Regex LineFormat = new Regex(@"+0x([0-9a-fA-F]+) ([A-Za-z_][A-Za-z0-9_]+)[ ]+: (.*)");
        /// <summary>
        /// Represents one line of output.
        /// </summary>
        public struct Line
        {
            public ulong Offset { get; private set; }
            public string Name { get; private set; }
            public string Value { get; private set; }
            public string Type { get; private set; }

            /// <summary>
            /// Whether this is part of a bitfield's bit breakdown.
            /// </summary>
            public bool IsBits { get; private set; }
        }

        public string TypeName { get; private set; }
        public bool RecursionDetected { get; private set; } // TODO: Allow for internal recursion when parsing dt.
        private readonly IList<Line> _lines = new List<Line>();

        /// <summary>
        /// Disallow Construction
        /// </summary>
        private DumpType()
        {
            
        }

        public static DumpType Parse(string content)
        {
            var output = new DumpType();
            
            bool first = true;
            foreach (var l in content.Split('\n'))
            {
                if (first && !l.Contains(":"))
                {
                    output.TypeName = l;
                    first = false;
                    continue;
                }
                first = false;
                var m = LineFormat.Matches(l);

                //var offset = m[0]
            }
            return null;
        }

        public IEnumerable<Line> GetOutput()
        {
            return _lines;
        }
    }
}
