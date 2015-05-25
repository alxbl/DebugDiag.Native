using System.Linq;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestList
    {
        private const int Size = 3;
        private static readonly MockX86Dump Context = new MockX86Dump();
        private static List _list;
        private static List _emptyList;

        [ClassInitialize]
        public static void Setup(TestContext ctx)
        {
            Native.Initialize(Context);

            var gList = new Fixtures.Generators.List(0xeeeee, Size, new Fixtures.Generators.PodType(0, 1));
            Context.AddFixture(gList);
            _list = NativeType.AtAddress(gList.Address, gList.GetTypeName()) as List;
            var gEmpty = new Fixtures.Generators.List(0xefefef, 0, new Fixtures.Generators.PodType(0, 0));
            Context.AddFixture(gEmpty);
            _emptyList = NativeType.AtAddress(gEmpty.Address, gEmpty.GetTypeName()) as List;

            Assert.IsNotNull(_list);
            Assert.IsNotNull(_emptyList);
        }

        [TestMethod]
        public void TestListTypeParse()
        {
            var l = _list;
            Assert.IsNotNull(l.ValueType);
            Assert.IsInstanceOfType(l.ValueType, typeof(NativeType));
            Assert.AreEqual("PODType", l.ValueType.TypeName);
        }

        [TestMethod]
        public void TestListEnumerate()
        {
            var l = _list;
            var i = 0UL;
            foreach (var e in l)
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
        public void TestListEnumerateDynamic()
        {
            dynamic l = NativeType.AtAddress(_list.Address, _list.TypeName);
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
        public void TestListSize()
        {
            Assert.AreEqual((ulong)Size, _list.Size);
        }

        [TestMethod]
        public void TestEmptyList()
        {
            Assert.AreEqual(0UL, _emptyList.Size);
            int i = _emptyList.Count(); // foreach sanity check (shouldn't throw)
            Assert.AreEqual(0, i);
        }
    }
}
