using System;
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
        private static readonly IDumpContext _context = new MockX86Dump();
        private static Vector _vector;
        [TestInitialize]
        public void Setup()
        {
            TypeParser.RegisterUserType(Vector.Syntax, new Vector(X86.PtrVector));
            Native.Initialize(_context);
            _vector = NativeType.AtAddress(X86.PtrVectorAddr, X86.PtrVector) as Vector;
            Assert.IsNotNull(_vector);
        }

        [TestMethod]
        public void TestVectorTypeParse()
        {
            var v = _vector;
            Assert.IsNotNull(v);
            Assert.IsNotNull(v.ElementType);
            Assert.AreEqual(X86.PtrVectorElementType, v.ElementType.TypeName);
            Assert.IsInstanceOfType(v.ElementType, typeof (Pointer));
        }

        [TestMethod]
        public void TestVectorSize()
        {
            var v = _vector;
            Assert.AreEqual(3UL, v.Size);
        }

        [TestMethod]
        public void TestVectoryEmpty()
        {
            
        }

        [TestMethod]
        public void TestVectorEnumerate()
        {
            var count = (ulong)_vector.Count();
            Assert.AreEqual(_vector.Size, count);
        }

        [TestMethod]
        public void TestVectorCapacity()
        {
            Assert.AreEqual(3UL, _vector.Capacity);
        }
    }
}
