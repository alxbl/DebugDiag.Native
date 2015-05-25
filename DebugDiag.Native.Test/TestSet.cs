using System.Linq;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestSet
    {
        private const int Size = 3;
        private static readonly MockX86Dump Context = new MockX86Dump();
        private static Set _set;
        private static Set _emptySet;

        [ClassInitialize]
        public static void Setup(TestContext ctx)
        {
            Native.Initialize(Context);

            var gSet = new Fixtures.Generators.Set(0x6666, Size, new Fixtures.Generators.PodType(0, 1));
            Context.AddFixture(gSet);
            _set = NativeType.AtAddress(gSet.Address, gSet.GetTypeName()) as Set;
            var gEmpty = new Fixtures.Generators.Set(0xefefef, 0, new Fixtures.Generators.PodType(0, 0));
            Context.AddFixture(gEmpty);
            _emptySet = NativeType.AtAddress(gEmpty.Address, gEmpty.GetTypeName()) as Set;

            Assert.IsNotNull(_set);
            Assert.IsNotNull(_emptySet);
        }

        [TestMethod]
        public void TestSetTypeParse()
        {
            var s = _set;
            Assert.IsNotNull(s.ValueType);
            Assert.IsInstanceOfType(s.ValueType, typeof(NativeType));
            Assert.AreEqual("PODType", s.ValueType.TypeName);
        }

        [TestMethod]
        public void TestSetEnumerate()
        {
            var s = _set;
            var i = 0UL;
            foreach (var e in s)
            {
                Assert.IsNotNull(e);
                Assert.IsInstanceOfType(e, typeof(NativeType));
                Assert.IsTrue(e.IsInstance);
                Assert.AreEqual("PODType", e.TypeName);
                Assert.AreEqual(i + 1, e.GetField("Offset1"));
                i++;
            }
            Assert.AreEqual((ulong)Size, i);
        }

        [TestMethod]
        public void TestSetEnumerateDynamic()
        {
            dynamic l = NativeType.AtAddress(_set.Address, _set.TypeName);
            Assert.IsNotNull(l);

            ulong count = 0;
            // Support for LINQ on dynamic types is not implemented yet. Usually we will iterate with a foreach.
            foreach (var e in l)
            {
                Assert.IsNotNull(e);
                count++;
            }
            Assert.AreEqual((ulong)Size, count);
        }

        [TestMethod]
        public void TestSetSize()
        {
            Assert.AreEqual((ulong)Size, _set.Size);
        }

        [TestMethod]
        public void TestEmptySet()
        {
            Assert.AreEqual(0UL, _emptySet.Size);
            int i = _emptySet.Count(); // foreach sanity check (shouldn't throw)
            Assert.AreEqual(0, i);
        }
    }
}
