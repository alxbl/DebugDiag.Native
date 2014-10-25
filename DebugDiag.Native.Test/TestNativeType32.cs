using System;
using DebugDiag.Native.Test.Fixtures;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using DebugDiag.Native.Windbg;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    /// <summary>
    /// 32 bit native type tests.
    /// </summary>
    [TestClass]
    public class TestNativeType32
    {
        private static readonly IDumpContext Context = new MockX86Dump();
        
        // Initialize the right context on every test to make sure tests run against the right "dump"
        [TestInitialize]
        public void SetUp()
        {
            Native.Initialize(Context);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeDoesNotExistException))]
        public void TestPreloadUnknownType()
        {
            NativeType.Preload(X86.InvalidType);
        }

        [TestMethod]
        public void TestAtAddressVtableAsString()
        {
            var t = NativeType.AtAddress(X86.VtableAddr);
            Assert.AreEqual("VirtualTypeDeriv", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App", t.ModuleName);
            Assert.AreEqual("DebugDiag_Native_Test_App!VirtualTypeDeriv", t.QualifiedName);
        }

        [TestMethod]
        public void TestAtAddressVtableAsULong()
        {
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            Assert.AreEqual("VirtualTypeDeriv", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App", t.ModuleName);
            Assert.AreEqual("DebugDiag_Native_Test_App!VirtualTypeDeriv", t.QualifiedName);
        }

        [TestMethod]
        public void TestAtAddressVtableWithoutVtable()
        {
            // This type does not have a vtable, we shouldn't be able to find it.
            var t = NativeType.AtAddress(X86.PodTypeAddr);
            Assert.IsNull(t);
        }

        [TestMethod]
        public void TestGetAddress()
        {
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            Assert.AreEqual(X86.VtableAddrULong, t.Address);
        }

        [TestMethod]
        public void TestAtAddressNoVtableAsString()
        {
            var t = NativeType.AtAddress(X86.PodTypeAddr, "DebugDiag_Native_Test_App!PODType");
            Assert.AreEqual(X86.PodTypeAddrULong, t.Address);
            Assert.AreEqual("PODType", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App!PODType", t.QualifiedName);
        }

        [TestMethod]
        public void TestAtAddressNoVtableAsULong()
        {
            var t = NativeType.AtAddress(X86.PodTypeAddrULong, "DebugDiag_Native_Test_App!PODType");
            Assert.AreEqual(X86.PodTypeAddrULong, t.Address);
            Assert.AreEqual("PODType", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App!PODType", t.QualifiedName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAtAddressNull()
        {
            var t = NativeType.AtAddress(0, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAtAddressInvalid()
        {
            var t = NativeType.AtAddress("notAnAddress");
        }

        [TestMethod]
        public void TestParseTypeWithBitfield()
        {
            var t = NativeType.AtAddress(X86.PebAddr, "nt!_PEB");
            var field = t.GetField(0x3); // PEB->BitField
            Assert.AreEqual(0x8UL, field.GetIntValue());
        }

        [TestMethod]
        public void TestParseTypeWithMultipleVtables()
        {
            // Nested types with vtables will have more than one __VFN_table member.
            var t = NativeType.AtAddress(X86.MultiVtableAddr, "MultiVtable"); // No fixture for vtable discovery.
            Assert.IsNotNull(t);
        }

        [TestMethod]
        public void TestParseCorruptedVtableType()
        {
            Assert.Fail("Not implemented");
        }

        [TestMethod]
        public void TestParseTypeWithStaticField()
        {
            var t = NativeType.AtAddress(X86.StaticDtAddr, "HasAStaticField");
            var f = t.GetField("IAmSoStatic");
            Assert.IsTrue(f is Primitive);
            Assert.IsTrue(f.IsStatic);
            Assert.AreEqual(3UL, f.GetIntValue());
        }

        [TestMethod]
        public void TestStaticInDifferentModules()
        {
            Assert.Fail("Not Implemented");
            // A static member can have different values in different modules.
            // This is a disgusting case, but it needs to be handled properly.
            // The idea is that internally we want to always use the fully qualified type.
        }

        [TestMethod]
        public void TestDrillDownSubtypes()
        {
            // Retrieve the HasAStaticField object.
            var t = NativeType.AtAddress(X86.StaticDtAddr, "HasAStaticField");

            // Drill into its VirtualType instance.
            var virtualType = t.GetField("subType");
            Assert.IsFalse(virtualType is Primitive);
            Assert.IsFalse(virtualType.IsStatic);
            Assert.IsTrue(virtualType.IsInstance);
            Assert.AreEqual("VirtualTypeDeriv", virtualType.TypeName);
            
            // Drill into the VirtualType's PODType instance. 
            var podType = virtualType.GetField("PODObject");
            Assert.IsFalse(podType is Primitive);
            Assert.IsFalse(podType.IsStatic);
            Assert.IsTrue(podType.IsInstance);
            Assert.AreEqual("PODType", podType.TypeName);

            // Finally, get the PODType's Offset1 value.
            var offset1 = podType.GetField(0x000);
            Assert.AreNotSame(podType, offset1);
            // This will work because PODType has no vtable.
            Assert.AreEqual(42UL, offset1.GetIntValue()); 
        }

        [TestMethod]
        public void TestDrillDownSubtypesDynamic()
        {
            // Retrieve the HasAStaticField object.
            dynamic t = NativeType.AtAddress(X86.StaticDtAddr, "HasAStaticField");

            // Drill into its VirtualType instance (easily) thanks to 
            Assert.AreEqual(42UL, t.subType.PODObject.Offset1.GetIntValue());
        }

        [TestMethod]
        public void TestGetPrimitiveString()
        {
            Assert.Fail("Not Implemented");
        }

        [TestMethod]
        public void TestCastPrimitiveToInteger()
        {
            Assert.Fail("Not implemented");
        }

        [TestMethod]
        public void TestCastPrimitiveToString()
        {
            Assert.Fail("Not implemented");
        }

        [TestMethod]
        public void TestCastNonPrimitiveToInteger()
        {
            Assert.Fail("Not implemented");
        }

        [TestMethod]
        public void TestCastNonPrimitiveToString()
        {
            Assert.Fail("Not implemented");
        }
    }
}
