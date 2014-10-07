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
            InputOutputMap["ln poi(0x49beb8)"] = Fixtures.X86.VtableLnPoi;
            InputOutputMap["ln poi(49beb8)"] = Fixtures.X86.VtableLnPoi;
            InputOutputMap["dt DebugDiag_Native_Test_App!VirtualTypeDeriv"] = Fixtures.X86.VirtualTypeDerivDt;
            InputOutputMap["dt DebugDiag_Native_Test_App!VirtualTypeDeriv 0x49beb8"] = Fixtures.X86.VirtualTypeDerivInst;
            InputOutputMap["dt Int4B 0x49bebc"] = "Symbol Int4B not found.";
            InputOutputMap["dt Int4B"] = "Symbol Int4B not found.";
            InputOutputMap["dt nt!_PEB"] = Fixtures.X86.DtPeb;
            InputOutputMap["dt nt!_PEB 0x7efde000"] = Fixtures.X86.DtPebInst;
            InputOutputMap["dt nt!_PEB 7efde000"] = Fixtures.X86.DtPebInst;
            InputOutputMap["dt nt!InvalidDoNotExist"] = Fixtures.X86.InvalidTypeDt;
            InputOutputMap["dt DebugDiag_Native_Test_App!PODType"] = Fixtures.X86.DtPodType;
            InputOutputMap["dt DebugDiag_Native_Test_App!PODType 0x49becc"] = Fixtures.X86.DtPodTypeInst;
            InputOutputMap["dt HasAStaticField"] = Fixtures.X86.StaticDt;
            InputOutputMap["dt DebugDiag_Native_Test_App!HasAStaticField 0x29cc00"] = Fixtures.X86.StaticDtInst;
            InputOutputMap["dt HasAStaticField 0x29cc00"] = Fixtures.X86.StaticDtInst;
            InputOutputMap["dt MultiVtable"] = Fixtures.X86.MultiVtableDt;
            InputOutputMap["dt MultiVtable 0x88ccff22"] = Fixtures.X86.MultiVtableDtInst;
            InputOutputMap["dt HasAStaticField 0x29cc00"] = Fixtures.X86.StaticDtInst;
            InputOutputMap["dt DebugDiag_Native_Test_App!VirtualType 0x29cc00"] = Fixtures.X86.StaticDtDrillSubType;
            InputOutputMap["dt DebugDiag_Native_Test_App!PODType 0x29cc14"] = Fixtures.X86.StaticDtDrillPod;
            //InputOutputMap[""] =;
        }

        private static readonly IDictionary<string, string> InputOutputMap = new Dictionary<string, string>();
    }
}
