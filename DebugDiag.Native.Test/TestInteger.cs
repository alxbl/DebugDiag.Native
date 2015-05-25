using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestInteger
    {
        private static readonly MockX86Dump Context = new MockX86Dump();
 
        [ClassInitialize]
        public static void Setup(TestContext ctx)
        {
            Native.Initialize(Context);
        }

        [TestMethod]
        public void TestAtAddressInteger()
        {
            var gInt = new Fixtures.Generators.Integer(0xa0aa0a00, 42);
            Context.AddFixture(gInt);

            var i = NativeType.AtAddress(gInt.Address, gInt.GetTypeName());
            Assert.IsTrue(i is Integer);
            Assert.AreEqual(gInt.Value, i);
        }

        [TestMethod]
        public void TestCasts()
        {
            var gInt = new Fixtures.Generators.Integer(0x11221133, 750);
            Context.AddFixture(gInt);

            var i = NativeType.AtAddress(gInt.Address, gInt.GetTypeName());
            Assert.IsTrue(i is Integer);
            Assert.AreEqual(gInt.Value, i);

            // ReSharper disable RedundantCast
            Assert.AreEqual((float) gInt.Value, (float)i);
            Assert.AreEqual((double) gInt.Value, (double)i);
            Assert.AreEqual((int)gInt.Value, (int)i);
            Assert.AreEqual(gInt.Value != 0, (bool)i);
        }
    }
}
