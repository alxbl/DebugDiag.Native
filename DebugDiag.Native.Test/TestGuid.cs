using System;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestGuid
    {
        private static readonly MockX86Dump Context = new MockX86Dump();


        [TestInitialize]
        public void Setup()
        {
            Native.Initialize(Context);

        }

        [TestMethod]
        public void TestAtAddressGuid()
        {
            const ulong addr = 0x1000;
            var expected = new System.Guid("73ABE945-9114-4673-9C9F-8B207B3FF4C3");
            var guid = new Fixtures.Generators.Guid(addr, expected);
            Context.AddFixture(guid);

            var t = NativeType.AtAddress(addr, guid.GetTypeName());
            Assert.IsInstanceOfType(t, typeof(Type.Guid));
            Assert.IsTrue(t is Primitive);
            Assert.IsTrue(t.IsInstance);
            Assert.IsFalse(t.IsStatic);
            Assert.AreEqual(string.Format("{{{0}}}", expected), t.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetFieldGuid()
        {
            // The Guid object abstracts away all of the GUIDs internals, and doesn't allow GetField().
            const ulong addr = 0x1001;
            var expected = new System.Guid("73ABE945-9114-4673-9C9F-8B207B3FF4C4");
            var guid = new Fixtures.Generators.Guid(addr, expected);
            Context.AddFixture(guid);

            var t = NativeType.AtAddress(addr, guid.GetTypeName());
            t.GetField("Data1");
        }

        [TestMethod]
        public void TestGetStringValueGuid()
        {
            const ulong addr = 0x1002;
            var expected = new System.Guid("73ABE945-9114-4673-9C9F-8B207B3FF4C5");
            var guid = new Fixtures.Generators.Guid(addr, expected);
            Context.AddFixture(guid);

            var t = NativeType.AtAddress(addr, guid.GetTypeName());
            Assert.AreEqual(string.Format("{{{0}}}", expected), (string)t);
        }

        [TestMethod]
        [ExpectedException((typeof(InvalidCastException)))]
        public void TestGetIntValueGuid()
        {
            const ulong addr = 0x1003;
            var expected = new System.Guid("73ABE945-9114-4673-9C9F-8B207B3FF4C6");
            var guid = new Fixtures.Generators.Guid(addr, expected);
            Context.AddFixture(guid);

            var t = NativeType.AtAddress(addr, guid.GetTypeName());
            ulong asUlong = t;
        }

        [TestMethod]
        public void TestGetInlineGuid()
        {
            // Place-holder test
            // Optimization: Do not run the dt command if the GUID can be obtained in-line when doing type discovery.
            Assert.Inconclusive();
        }
    }
}
