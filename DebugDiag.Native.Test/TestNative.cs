using System;
using System.Diagnostics;
using DebugDiag.Native.Test.Fixtures;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Windbg;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    /// <summary>
    /// Summary description for TestNative
    /// </summary>
    [TestClass]
    public class TestNative
    {
        [ClassInitialize]
        public static void SetUp(TestContext ctx)
        {
            Native.Initialize(new MockX86Dump());
        }

        [TestMethod]
        public void TestAddressFormat()
        {
            var validFormats = new string[]
                               {
                                   "1234",
                                   "0x49beb8",
                                   "0x32003200",
                                   "32003200",
                                   "1234567a",
                                   "aabbccdd",
                                   "ee000000",
                                   "0x6400640064006400",
                                   "6400640064006400",
                                   "64006400`64006400",
                                   "0x64006400`64006400",
                                   "0n123",
                                   "a",

                               };
            var invalidFormats = new string[]
                                 {
                                     "0x",
                                     "0n",
                                     "'''InvalidSymbols",
                                     "ghijklmno",
                                     "0xgggggggg",
                                     "-1",
                                     "null",
                                     "0n123a",
                                 };
            foreach (var addr in validFormats) Assert.IsTrue(Native.AddressFormat.IsMatch(addr), "Adddress {0} should be valid.", addr);

            foreach (var addr in invalidFormats) Assert.IsFalse(Native.AddressFormat.IsMatch(addr), "Adddress {0} should be invalid", addr);
        }

        [TestMethod]
        public void TestGetFieldByName()
        {
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            var field = t.GetField("POD");
            Assert.IsTrue(field.IsPrimitive);
            Assert.AreEqual(X86.VtableAddrULong + t.GetOffset("POD"), field.Address);
            Assert.AreEqual(8UL, field.GetIntValue());
        }

        [TestMethod]
        public void TestGetIntValueField()
        {
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            Assert.AreEqual(0UL, t.GetIntValue("MoreOffset"));
            Assert.AreEqual(8UL, t.GetIntValue(0x004));
        }

        [TestMethod]
        public void TestDynamicFieldAccess()
        {
            dynamic t = NativeType.AtAddress(X86.VtableAddrULong);
            dynamic field = t.POD;
            Assert.IsTrue(field.IsPrimitive);
            Assert.AreEqual(8UL, field.GetIntValue());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInvalidDynamicFieldAccess()
        {
            dynamic t = NativeType.AtAddress(X86.VtableAddrULong);
            dynamic field = t.DoesNotExist;
        }

        [TestMethod]
        public void TestGetPrimitiveWhenInstance()
        {
            // WARN: This will return the raw memory at the type's root.
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            //var field = t.GetField(0x14);
            Assert.IsFalse(t.IsPrimitive);
            Assert.AreEqual(0x0114cc84UL, t.GetIntValue()); // vtable address.
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetFieldInvalidOffset()
        {
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            t.GetField(0x10000); // This offset does not belong to that object.
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetFieldInvalidName()
        {
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            t.GetField("DoesNotExist"); // This field does not belong to that object.
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetInstanceWhenPrimitive()
        {
            // Getting a field on a primitive is an invalid operation.
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            var field = t.GetField(0x4);
            Assert.IsTrue(field.IsInstance);
            Assert.IsTrue(field.IsPrimitive);
            Assert.AreEqual(8UL, field.GetIntValue());
            field.GetField(0x0);
        }

        [TestMethod]
        public void TestGetZeroOffsetWithVtable()
        {
            // A virtual type has its vtable at offset 0x000.
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            Assert.IsTrue(t.HasVtable);
            var field = t.GetField(0x0);
            Assert.IsTrue(field.IsPrimitive);
            Assert.IsTrue(field.IsInstance);
            Assert.AreEqual("Ptr32", field.QualifiedName);
            Assert.AreEqual(0x0114cc84UL, field.GetIntValue());
            // TODO: It should be possible to inspect the vtable.
        }

        [TestMethod]
        public void TestGetZeroOffsetPod()
        {
            // A POD's first member is located at offset 0x000 and can be anything.
            var t = NativeType.AtAddress(X86.PodTypeAddr, "DebugDiag_Native_Test_App!PODType");
            Assert.AreEqual("DebugDiag_Native_Test_App!PODType", t.QualifiedName);
            var field = t.GetField(0x0);
            Assert.IsTrue(field.IsPrimitive);
            Assert.IsTrue(field.IsInstance);
            Assert.AreEqual(42UL, field.GetIntValue());
        }

        [TestMethod]
        public void TestStringToULongLeadingZerosHex()
        {
            Assert.AreEqual(0x12345UL, Native.StringAddrToUlong("00012345"));
        }

        [TestMethod]
        public void TestStringToULongLeadingZerosDecimal()
        {
            Assert.AreEqual(12345UL, Native.StringAddrToUlong("0n00012345"));
        }

        [TestMethod]
        public void TestStringToULong0X()
        {
            Assert.AreEqual(0x12345UL, Native.StringAddrToUlong("0x00012345"));
        }

        [TestMethod]
        public void TestStringToULong0N()
        {
            Assert.AreEqual(12345UL, Native.StringAddrToUlong("0n12345"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestStringToULongInvalidAddr()
        {
            Native.StringAddrToUlong("sdlkfjsdlfd");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestStringToULongEmptyAddr0N()
        {
            Native.StringAddrToUlong("0n");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestStringToULongEmptyAddr0X()
        {
            Native.StringAddrToUlong("0x");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestStringToULongEmptyAddr()
        {
            Native.StringAddrToUlong("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestStringToULongNullAddr()
        {
            Native.StringAddrToUlong(null);
        }

        [TestMethod]
        public void TestStringToULongWindbgNull()
        {
            Assert.AreEqual(0UL, Native.ParseWindbgPrimitive("(null)"));
        }
    }
}
