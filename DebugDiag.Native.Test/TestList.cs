using System;
using DebugDiag.Native.Test.Fixtures;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestList
    {
        private static readonly IDumpContext _context = new MockX86Dump();
        private static List _list;
        [TestInitialize]
        public void Setup()
        {
            TypeParser.RegisterUserType(Vector.Syntax, new Vector(X86.PtrVector));
            Native.Initialize(_context);
            _list = NativeType.AtAddress(X86.PtrVectorAddr, X86.PtrVector) as List;
            Assert.IsNotNull(_list);
        }

        [TestMethod]
        public void TestListTypeParse()
        {
        }
    }
}
