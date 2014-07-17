using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    /// <summary>
    /// Summary description for TestNative
    /// </summary>
    [TestClass]
    public class TestNative
    {
        [TestMethod]
        public void TestAddressFormat()
        {
            var validFormats = new string[]
                               {
                                   "0x32003200",
                                   "32003200",
                                   "1234567a",
                                   "aabbccdd",
                                   "ee000000",
                                   "0x6400640064006400",
                                   "6400640064006400",
                                   "64006400`64006400",
                                   "0x64006400`64006400"
                               };
            var invalidFormats = new string[]
                                 {
                                     "0x",
                                     "'''InvalidSymbols",
                                     "ghijklmno",
                                     "0xgggggggg",
                                     "-1",
                                     "null"
                                 };
            foreach (var addr in validFormats) Assert.IsTrue(Native.AddressFormat.IsMatch(addr));

            foreach (var addr in invalidFormats) Assert.IsFalse(Native.AddressFormat.IsMatch(addr));
        }
    }
}
