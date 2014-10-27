using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DebugDiag.DotNet;
using DebugDiag.Native.Test.Fixtures;

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
            #region VirtualType
            InputOutputMap["ln poi(0x49beb8)"] = X86.VtableLnPoi;
            InputOutputMap["ln poi(49beb8)"] = X86.VtableLnPoi;
            InputOutputMap["dt 0 DebugDiag_Native_Test_App!VirtualTypeDeriv"] = X86.VirtualTypeDerivDt;
            InputOutputMap["dt 0 VirtualTypeDeriv"] = X86.VirtualTypeDerivDt;
            InputOutputMap["dt 0x49beb8 DebugDiag_Native_Test_App!VirtualTypeDeriv "] = X86.VirtualTypeDerivInst;
            InputOutputMap["dt 0x49beb8 DebugDiag_Native_Test_App!VirtualTypeDeriv"] = X86.VirtualTypeDerivInst;
            #endregion

            #region Edge Cases
            InputOutputMap["dt 0x49bebc Int4B"] = "Symbol Int4B not found.";
            InputOutputMap["dt 0 Int4B"] = "Symbol Int4B not found.";
            InputOutputMap["dt 0 nt!InvalidDoNotExist"] = X86.InvalidTypeDt;
            InputOutputMap["dt 0 InvalidDoNotExist"] = X86.InvalidTypeUnqualifiedDt;
            #endregion

            #region PEB
            InputOutputMap["dt 0 nt!_PEB"] = X86.DtPeb;
            InputOutputMap["dt 0x7efde000 nt!_PEB"] = X86.DtPebInst;
            InputOutputMap["dt 0x7efde000 ntdll!_PEB"] = X86.DtPebInst;
            InputOutputMap["dt 7efde000 nt!_PEB"] = X86.DtPebInst;
            #endregion

            #region POD
            InputOutputMap["dt 0 DebugDiag_Native_Test_App!PODType"] = X86.DtPodType;
            InputOutputMap["dt 0 PODType"] = X86.DtPodType;
            InputOutputMap["dt 0x49becc DebugDiag_Native_Test_App!PODType"] = X86.DtPodTypeInst;
            #endregion

            #region Multiple Vtables
            InputOutputMap["dt 0 MultiVtable"] = X86.MultiVtableDt;
            InputOutputMap["dt 0x88ccff22 MultiVtable"] = X86.MultiVtableDtInst;
            #endregion

            #region Static Type
            InputOutputMap["dt 0 HasAStaticField"] = X86.StaticDt;
            InputOutputMap["dt 0x29cc00 HasAStaticField"] = X86.StaticDtInst;
            InputOutputMap["dt 0x29cc00 DebugDiag_Native_Test_App!HasAStaticField"] = X86.StaticDtInst;
            InputOutputMap["dt 0x29cc00 HasAStaticField"] = X86.StaticDtInst;
            #endregion

            #region DrillDown
            InputOutputMap["dt 0x29cc00 VirtualTypeDeriv"] = X86.VirtualTypeDerivInst;
            InputOutputMap["dt 0x29cc00 DebugDiag_Native_Test_App!VirtualTypeDeriv"] = X86.VirtualTypeDerivInst;
            InputOutputMap["dt 0x29cc14 PODType"] = X86.StaticDtDrillPod;
            #endregion

            #region Map
            #endregion

            #region Vector
            InputOutputMap["dt 0 " + X86.PtrVector] = X86.PtrVectorDt;
            InputOutputMap[String.Format("dt {0} {1}", X86.PtrVectorAddr, X86.PtrVector)] = X86.PtrVectorDtInst;
            InputOutputMap["?? sizeof(PODType *)"] = "unsigned int 4";
            #endregion
        }

        private static readonly IDictionary<string, string> InputOutputMap = new Dictionary<string, string>();
    }
}
