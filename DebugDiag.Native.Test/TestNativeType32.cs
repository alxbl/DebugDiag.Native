using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    /// <summary>
    /// 32 bit native type tests.
    /// </summary>
    [TestClass]
    public class TestNativeType32
    {
        [TestMethod]
        public void TestPreloadType()
        {
            NativeType.Preload("DebugDiag_Native_Test_App!PODType");
            Assert.IsTrue(NativeType.IsCached("DebugDiag_Native_Test_App!PODType"));
        }

        [TestMethod]
        public void TestAtAddressVtableAsString()
        {
            var t = NativeType.AtAddress(Fixtures32.VtableAddr);
            Assert.AreEqual("VirtualTypeDeriv", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App", t.ModuleName);
            Assert.AreEqual("DebugDiag_Native_Test_App!VirtualTypeDeriv", t.QualifiedName);
        }

        [TestMethod]
        public void TestAtAddressVtableAsULong()
        {
            var t = NativeType.AtAddress(Fixtures32.VtableAddrULong);
            Assert.AreEqual("VirtualTypeDeriv", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App", t.ModuleName);
            Assert.AreEqual("DebugDiag_Native_Test_App!VirtualTypeDeriv", t.QualifiedName);
        }

        [TestMethod]
        public void TestAtAddressNoVtableAsString()
        {

        }

        [TestMethod]
        public void TestAtAddressNoVtableAsULong()
        {

        }

        [TestMethod]
        public void TestAtAddressWithRightType()
        {
        }

        [TestMethod]
        public void TestAtAddressWithWrongType()
        {
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAtAddressNull()
        {
            var t = NativeType.AtAddress(0, null);
        }
    }
}
