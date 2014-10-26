using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Windbg;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestWindbg
    {
        private static readonly IDumpContext Context = new MockX86Dump();

        [TestInitialize]
        public void Setup()
        {
            Native.Initialize(Context);
        }

        #region Command

        [TestMethod]
        public void TestCommandOutput()
        {
            var c = new DumpType("HasAStaticField");
            c.Execute();
            Assert.IsTrue(c.Executed, "Command should mark Executed to true on execution.");
            Assert.IsNotNull(c.Output);
            Assert.IsTrue(c.Output.Contains("+0x000"));
        }

        [TestMethod]
        public void TestCommandOutputLazyExecute()
        {
            var c = new DumpType("HasAStaticField");
            Assert.IsFalse(c.Executed);
            var output = c.Output; // This causes the command to execute.
            Assert.IsTrue(c.Executed);
            Assert.IsNotNull(c.Output);
            Assert.IsTrue(c.Output.Contains("+0x000"));
        }

        #endregion
        #region DumpType

        [TestMethod]
        public void TestDumpTypeEnumerator()
        {
            var dt = new DumpType("HasAStaticField");
            dt.Execute();
            Assert.IsNotNull(dt.GetEnumerator());
            // TODO: Check that the `dt` parsing was successful.
        }

        #endregion
    }
}
