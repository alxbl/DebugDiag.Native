using System.Linq;
using DebugDiag.Native.Test.Fixtures;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestVector
    {
        private static readonly IDumpContext Context = new MockX86Dump();
        private static Vector _vector;

        [TestInitialize]
        public void Setup()
        {
            Native.Initialize(Context);
            _vector = NativeType.AtAddress(X86.PtrVectorAddr, X86.PtrVector) as Vector;
            Assert.IsNotNull(_vector);
        }

        [TestMethod]
        public void TestVectorTypeParse()
        {
            var v = _vector;
            Assert.IsNotNull(v);
            Assert.IsNotNull(v.ValueType);
            Assert.AreEqual(X86.PtrVectorElementType, v.ValueType.TypeName);
            Assert.IsInstanceOfType(v.ValueType, typeof (Pointer));
        }

        [TestMethod]
        public void TestVectorSize()
        {
            var v = _vector;
            Assert.AreEqual(3UL, v.Size);
        }

        [TestMethod]
        public void TestVectorEmpty()
        {
            Assert.Fail("Not implemented");
        }

        [TestMethod]
        public void TestVectorEnumerate()
        {
            var count = (ulong)_vector.Count();
            Assert.AreEqual(3UL, count);
        }
        
        [TestMethod]
        public void TestVectorEnumerateDynamic()
        {
            dynamic v = NativeType.AtAddress(X86.PtrVectorAddr, X86.PtrVector);
            Assert.IsNotNull(v);
            
            ulong count = 0;
            // Support for LINQ on dynamic types is not implemented yet. Usually we will iterate with a foreach.
            foreach (var e in v)
            {
                Assert.IsNotNull(e);
                count++;
            }
            Assert.AreEqual(3UL, count);
        }

        [TestMethod]
        public void TestVectorCapacity()
        {
            Assert.AreEqual(3UL, _vector.Capacity);
        }
    }
}
