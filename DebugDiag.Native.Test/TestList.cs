using DebugDiag.Native.Test.Fixtures;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestList
    {
        private static readonly IDumpContext Context = new MockX86Dump();
        private static List _list;
        [TestInitialize]
        public void Setup()
        {
            Native.Initialize(Context);
            _list = NativeType.AtAddress(X86.ListAddr, X86.List) as List;
            Assert.IsNotNull(_list);
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
                Assert.AreEqual(i+1, e.GetIntValue("Offset1"));
                i++;
            }
            Assert.AreEqual(3UL, i);
        }

        [TestMethod]
        public void TestListEnumerateDynamic()
        {
            dynamic l = NativeType.AtAddress(X86.ListAddr, X86.List);
            Assert.IsNotNull(l);

            ulong count = 0;
            // Support for LINQ on dynamic types is not implemented yet. Usually we will iterate with a foreach.
            foreach (var e in l)
            {
                Assert.IsNotNull(e);
                count++;
            }
            Assert.AreEqual(3UL, count);
        }

        [TestMethod]
        public void TestListSize()
        {
            Assert.AreEqual(3UL, _list.Size);
        }

        [TestMethod]
        public void TestEmptyList()
        {
            Assert.Inconclusive("Not implemented.");
        }
    }
}
