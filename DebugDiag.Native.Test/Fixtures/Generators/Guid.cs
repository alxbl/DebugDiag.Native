using System.Collections.Generic;

namespace DebugDiag.Native.Test.Fixtures.Generators
{
    internal class Guid : Generator
    {
        private readonly System.Guid _value;

        public Guid(ulong addr, System.Guid value)
        {
            Address = addr;
            _value = value;
        }

        public override string GetTypeName()
        {
            return "ntdll!_GUID";
        }

        public override KeyValuePair<string, string> GetTypeInfo()
        {
            return new KeyValuePair<string, string>("dt 0 ntdll!_GUID", @"   +0x000 Data1            : Uint4B
   +0x004 Data2            : Uint2B
   +0x006 Data3            : Uint2B
   +0x008 Data4            : [8] UChar");
        }

        public override IEnumerable<KeyValuePair<string, string>> GenerateInternal()
        {
            var kv = new KeyValuePair<string, string>(string.Format("dt 0x{0:x} ntdll!_GUID", Address),
                string.Format(@" {{{0}}}
   +0x000 Data1            : 0xbaadf00d
   +0x004 Data2            : 0xbaadf00d
   +0x006 Data3            : 0xbaadf00d
   +0x008 Data4            : [8]  ""unused by DebugDiag.Native""", _value)); // Data1~4 are unused by library.
            yield return kv;
        }
    }
}

