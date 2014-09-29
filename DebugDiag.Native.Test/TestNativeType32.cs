using System;
using DebugDiag.Native.Test.Fixtures;
using DebugDiag.Native.Test.Mock;
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
        public void TestGetPrimitiveString()
        {
            Assert.Fail("Not Implemented");
        }
    }
}
