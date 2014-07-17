using System;
using DebugDiag.Native.Test.Fixtures;
using DebugDiag.Native.Test.Mock;
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
        public void TestPreloadType()
        {
            Assert.IsTrue(NativeType.Preload("DebugDiag_Native_Test_App!PODType"));
        }

        [TestMethod]
        public void TestPreloadUnknownType()
        {
            Assert.IsFalse(NativeType.Preload(X86.InvalidType));
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
        public void TestAtAddressNoVtableAsString()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TestAtAddressNoVtableAsULong()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TestAtAddressWithRightType()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TestAtAddressWithWrongType()
        {
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAtAddressNull()
        {
            var t = NativeType.AtAddress(0, null);
        }
    }
}
