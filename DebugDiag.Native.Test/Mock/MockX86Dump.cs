using System;
using System.Collections.Generic;
using System.Linq;
using DebugDiag.DotNet;
using DebugDiag.Native.Test.Fixtures;

namespace DebugDiag.Native.Test.Mock
{
    internal class MockX86Dump : IDumpContext
    {
        public NetScriptManager Manager { get; private set; }
        public NetDbgObj Debugger { get; private set; }
        public NetProgress Progress { get; private set; }
        public string Filename { get { return "MockX86Dump.dmp";  } }
        public bool Is32Bit { get { return true; } }

        public string Execute(string cmd)
        {
            if (!InputOutputMap.ContainsKey(cmd))
                throw new Exception("Mock command not found: " + cmd);
            return InputOutputMap[cmd]; 
        }

        /// <summary>
        /// Adds a generated fixture to the dump context.
        /// </summary>
        /// <param name="g">The constructed generator that represents the fixture.</param>
        public void AddFixture(Generator g)
        {
            foreach (var f in g.Where(f => f.Key != null && f.Value != null))
            {
                InputOutputMap[f.Key] = f.Value; // Register this fixture's output.
            }
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
            InputOutputMap["ln poi(0x49becc)"] = ""; // Nothing returned.
            InputOutputMap["dt 0 DebugDiag_Native_Test_App!VirtualTypeDeriv"] = X86.VirtualTypeDerivDt;
            InputOutputMap["dt 0 VirtualTypeDeriv"] = X86.VirtualTypeDerivDt;
            InputOutputMap["dt 0x49beb8 DebugDiag_Native_Test_App!VirtualTypeDeriv "] = X86.VirtualTypeDerivInst;
            InputOutputMap["dt 0x49beb8 DebugDiag_Native_Test_App!VirtualTypeDeriv"] = X86.VirtualTypeDerivInst;
            InputOutputMap["dp /c1 0x49beb8 L1"] = "0049beb8  0114cc84";
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
            InputOutputMap["dt 0 PODType"] = X86.DtPodType;
            InputOutputMap["dt 0x49becc PODType"] = X86.DtPodTypeInst;
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
            #region Pointer
            InputOutputMap["dp /c1 0x5bd3e0 L1"] = X86.Dp;
            InputOutputMap["dp /c1 0x5bd3e4 L1"] = X86.Dp1;
            InputOutputMap["dp /c1 0x5bd3e8 L1"] = X86.Dp2;
            InputOutputMap["dp /c1 0x0 L1"] = X86.DpInvalid;
            #endregion
            #region Vector
            InputOutputMap["dt 0 " + X86.PtrVector] = X86.PtrVectorDt;
            InputOutputMap[String.Format("dt {0} {1}", X86.PtrVectorAddr, X86.PtrVector)] = X86.PtrVectorDtInst;
            InputOutputMap["?? sizeof(PODType *)"] = "unsigned int 4";
            InputOutputMap["dt 0x5bd0e0 PODType"] = X86.PtrVectorElem1;
            InputOutputMap["dt 0x5bd308 PODType"] = X86.PtrVectorElem2;
            InputOutputMap["dt 0x5bd398 PODType"] = X86.PtrVectorElem3;
            InputOutputMap["dt 0x5bd0e0 DebugDiag_Native_Test_App!PODType"] = X86.PtrVectorElem1;
            InputOutputMap["dt 0x5bd308 DebugDiag_Native_Test_App!PODType"] = X86.PtrVectorElem2;
            InputOutputMap["dt 0x5bd398 DebugDiag_Native_Test_App!PODType"] = X86.PtrVectorElem3;

            #endregion
            #region List

            InputOutputMap["dt 0 std::_List_node<PODType,void *>"] = X86.DtListNode;
            InputOutputMap[String.Format("dt 0 {0}", X86.List)] = X86.DtList;
            InputOutputMap[String.Format("dt {0} {1}", X86.ListAddr, X86.List)] = X86.DtListInst;
            InputOutputMap["dt 0x28d720 std::_List_node<PODType,void *>"] = X86.DtListHead;
            InputOutputMap["dt 0x28daf0 std::_List_node<PODType,void *>"] = X86.DtListNode1;
            InputOutputMap["dt 0x28daf8 PODType"] = X86.DtListElem1;
            InputOutputMap["dt 0x28db40 std::_List_node<PODType,void *>"] = X86.DtListNode2;
            InputOutputMap["dt 0x28db48 PODType"] = X86.DtListElem2;
            InputOutputMap["dt 0x28db90 std::_List_node<PODType,void *>"] = X86.DtListNode3;
            InputOutputMap["dt 0x28db98 PODType"] = X86.DtListElem3;

            #endregion
        }

        private static readonly IDictionary<string, string> InputOutputMap = new Dictionary<string, string>();
    }
}
