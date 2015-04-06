using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DebugDiag.Native.Windbg
{
    public class ForeachStl : Command
    {
        private readonly Type _type;
        private readonly ulong _addr;
        private readonly List<ulong> _elems = new List<ulong>(); 

        public enum Type
        {
            Map,
            Set,
            List,
            Tree
        }

        public ForeachStl(Type t, ulong addr)
        {
            _type = t;
            _addr = addr;
        }

        protected override string BuildCommand()
        {
            switch (_type)
            {
                case Type.Map:
                    return string.Format("!map 0x{0:x}", _addr);
                default:
                    throw new InvalidOperationException(string.Format("ForeachStl: Unsupported container type {0}", _type));
            }
        }

        protected override void Parse(string output)
        {
            var lines = output.Split('\n');
            Debug.Assert(lines.Length >= 1);
            //Size = ulong.Parse(lines[0].Split('=')[1]);
            foreach (var l in lines.Skip(1))
            {
                if (!string.IsNullOrWhiteSpace(l))
                    _elems.Add(Convert.ToUInt64(l.Trim('\r', ' ', '\n'), 16));
            }
        }

        public IEnumerable<ulong> GetElements()
        {
            if (!Executed) Execute();
            return _elems;
        }

        public ulong Size { get; private set; }
    }
}
