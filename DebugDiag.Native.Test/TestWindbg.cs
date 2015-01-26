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
            Assert.IsNotNull(output);
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
            Assert.IsFalse(string.IsNullOrEmpty(dt.Output));
        }
        #endregion

        #region Dp
        [TestMethod]
        public void TestDp32Bits()
        {
            Assert.Inconclusive("Not implemented.");
        }

        [TestMethod]
        public void TestDp64Bits()
        {
            Assert.Inconclusive("Not implemented.");
        }

        [TestMethod]
        [ExpectedException(typeof(CommandException))]
        public void TestDpInvalidLocation()
        {
            var dp = new Dp(0, 1);
            dp.Execute();
        }
        #endregion

        #region DumpString
        [TestMethod]
        public void TestDumpStringNarrow()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDumpStringWide()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDumpStringNarrowNull()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDumpStringWideNull()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDumpStringInvalidMemory()
        {
            // It's impossible for the computer to know that the memory is invalid.
            // In this scenario DumpString() will return trash until it finds a \0.
            Assert.Inconclusive();
        }
        #endregion
    }
}
