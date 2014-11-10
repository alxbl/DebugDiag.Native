using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DebugDiag.Native.Windbg
{
    class SizeOf : Command
    {
        private static readonly Regex OutputFormat = new Regex(@"^unsigned int ([^\n\r]+)");
        private readonly string _command;
        private ulong _size;

        public ulong Size
        {
            get
            {
                if (!Executed) Execute();
                return _size;
            }

            private set { _size = value; }
        }

        public SizeOf(string type)
        {
            _command = String.Format("?? sizeof({0})", type);
        }

        protected override string BuildCommand()
        {
            return _command;
        }

        protected override void Parse(string output)
        {
            if (string.IsNullOrWhiteSpace(output) || output.StartsWith("Unexpected token"))
                throw new CommandException(String.Format("`{0}` returned '{1}'", _command, output));

            var m = OutputFormat.Match(output);
            Debug.Assert(m.Groups.Count == 2, "SizeOf should always match on output format.");
            Size = Native.StringAddrToUlong(m.Groups[1].Value);
        }
    }
}
