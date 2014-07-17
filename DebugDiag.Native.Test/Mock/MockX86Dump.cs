using System.Collections.Generic;
using DebugDiag.DotNet;

namespace DebugDiag.Native.Test.Mock
{
    internal class MockX86Dump : IDumpContext
    {
        public NetScriptManager Manager { get; private set; }
        public NetDbgObj Debugger { get; private set; }
        public NetProgress Progress { get; private set; }
        
        public string Execute(string cmd)
        {
            return InputOutputMap.ContainsKey(cmd)
                ? InputOutputMap[cmd]
                : "";
        }

        public MockX86Dump()
        {
            Manager = null;
            Debugger = null;
            Progress = null;
        }

        /// <summary>
        /// Constructs the I/O map for mocking a dump context.
        /// </summary>
        static MockX86Dump()
        {
            InputOutputMap["ln poi(0x0049beb8)"] = Fixtures.X86.VtableLnPoi;
            InputOutputMap["ln poi(0049beb8)"] = Fixtures.X86.VtableLnPoi;
            InputOutputMap["dt DebugDiag_Native_Test_App!VirtualTypeDeriv"] = Fixtures.X86.VirtualTypeDerivDt;
            InputOutputMap["dt nt!_PEB 0x7efde000"] = Fixtures.X86.Peb;
            InputOutputMap["dt nt!_PEB 7efde000"] = Fixtures.X86.Peb;
            InputOutputMap["dt nt!InvalidDoNotExist"] = Fixtures.X86.InvalidTypeDt; 
            //InputOutputMap[""] =;
        }

        private static readonly IDictionary<string, string> InputOutputMap = new Dictionary<string, string>();
    }
}
