using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestPointer
    {
        [TestMethod]
        public void TestPointerSyntax()
        {
            var valid = new[] {"char *", "Ptr32 Char", "Ptr64 Char", "char **", "MyModule!Type *"};
            var invalid = new[] {"char", "****"};
            foreach (var s in valid) Assert.IsTrue(Pointer.Syntax.IsMatch(s), string.Format("Expected {0} to be valid pointer.", s));
            foreach (var s in invalid) Assert.IsFalse(Pointer.Syntax.IsMatch(s), string.Format("Expected {0} to be invalid pointer.", s));
        }

        [TestMethod]
        public void TestPointToPrimitive()
        {
            Assert.Inconclusive("Not implemented.");
        }

        [TestMethod]
        public void TestPointToNull()
        {
            Assert.Inconclusive("Not implemented.");
        }

        [TestMethod]
        public void TestPointToPointer()
        {
            Assert.Inconclusive("Not implemented.");
        }
    }
}
