using System;
using System.Diagnostics;
using DebugDiag.Native.Test.Fixtures;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using DebugDiag.Native.Windbg;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    /// <summary>
    /// Summary description for TestNative
    /// </summary>
    [TestClass]
    public class TestNativeType
    {
        [ClassInitialize]
        public static void SetUp(TestContext ctx)
        {
            Native.Initialize(new MockX86Dump());
        }

        [TestMethod]
        [ExpectedException(typeof(CommandException))]
        public void TestPreloadInvalidTypeQualified()
        {
            var t = NativeType.Preload("nt!InvalidDoNotExist");
        }

        [TestMethod]
        [ExpectedException(typeof(CommandException))]
        public void TestPreloadInvalidTypeUnqualified()
        {
            var t = NativeType.Preload("InvalidDoNotExist");
        }

        [TestMethod]
        public void TestPreloadCompoundType()
        {
            // Manually preloading a compound type is very unnatural, since templates usually have many
            // default parameters that windbg will always output, and requires to be explicitly specified
            // in order to function properly.
            // Usually, you should prefer getting the parent type, and navigating to the compound type using
            // `GetField()` or dynamic accessors.

            var t = NativeType.Preload(X86.PtrVector);
            Assert.IsNotNull(t);
            Assert.AreEqual(X86.PtrVector, t.TypeName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestNavigateNonInstance()
        {
            var t = NativeType.Preload("VirtualTypeDeriv");
            var pod = t.GetField("POD");
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
            Assert.IsTrue(field is Primitive);
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
            Assert.IsTrue(field is Primitive);
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
            Assert.IsFalse(t is Primitive);
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
            Assert.IsTrue(field is Primitive);
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
            Assert.IsTrue(field is Pointer); // Change to Vtable once Vtable support is added.
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
            Assert.IsTrue(field is Primitive);
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

                [TestMethod]
        public void TestAtAddressVtableAsString()
        {
            var t = NativeType.AtAddress(X86.VtableAddr);
            Assert.AreEqual("VirtualTypeDeriv", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App", t.ModuleName);
            Assert.AreEqual("DebugDiag_Native_Test_App!VirtualTypeDeriv", t.QualifiedName);
        }

        [TestMethod]
        public void TestAtAddressVtableAsULong()
        {
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            Assert.AreEqual("VirtualTypeDeriv", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App", t.ModuleName);
            Assert.AreEqual("DebugDiag_Native_Test_App!VirtualTypeDeriv", t.QualifiedName);
        }

        [TestMethod]
        public void TestAtAddressVtableWithoutVtable()
        {
            // This type does not have a vtable, we shouldn't be able to find it.
            var t = NativeType.AtAddress(X86.PodTypeAddr);
            Assert.IsNull(t);
        }

        [TestMethod]
        public void TestGetAddress()
        {
            var t = NativeType.AtAddress(X86.VtableAddrULong);
            Assert.AreEqual(X86.VtableAddrULong, t.Address);
        }

        [TestMethod]
        public void TestAtAddressNoVtableAsString()
        {
            var t = NativeType.AtAddress(X86.PodTypeAddr, "DebugDiag_Native_Test_App!PODType");
            Assert.AreEqual(X86.PodTypeAddrULong, t.Address);
            Assert.AreEqual("PODType", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App!PODType", t.QualifiedName);
        }

        [TestMethod]
        public void TestAtAddressNoVtableAsULong()
        {
            var t = NativeType.AtAddress(X86.PodTypeAddrULong, "DebugDiag_Native_Test_App!PODType");
            Assert.AreEqual(X86.PodTypeAddrULong, t.Address);
            Assert.AreEqual("PODType", t.TypeName);
            Assert.AreEqual("DebugDiag_Native_Test_App!PODType", t.QualifiedName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAtAddressNull()
        {
            var t = NativeType.AtAddress(0, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAtAddressInvalid()
        {
            var t = NativeType.AtAddress("notAnAddress");
        }

        [TestMethod]
        public void TestParseTypeWithBitfield()
        {
            var t = NativeType.AtAddress(X86.PebAddr, "nt!_PEB");
            var field = t.GetField(0x3); // PEB->BitField
            Assert.AreEqual(0x8UL, field.GetIntValue());
        }

        [TestMethod]
        public void TestParseTypeWithMultipleVtables()
        {
            // Nested types with vtables will have more than one __VFN_table member.
            var t = NativeType.AtAddress(X86.MultiVtableAddr, "MultiVtable"); // No fixture for vtable discovery.
            Assert.IsNotNull(t);
        }

        [TestMethod]
        public void TestParseCorruptedVtableType()
        {
            Assert.Fail("Not implemented");
        }

        [TestMethod]
        public void TestParseTypeWithStaticField()
        {
            var t = NativeType.AtAddress(X86.StaticDtAddr, "HasAStaticField");
            var f = t.GetField("IAmSoStatic");
            Assert.IsTrue(f is Primitive);
            Assert.IsTrue(f.IsStatic);
            Assert.AreEqual(3UL, f.GetIntValue());
        }

        [TestMethod]
        public void TestStaticInDifferentModules()
        {
            Assert.Fail("Not Implemented");
            // A static member can have different values in different modules.
            // This is a disgusting case, but it needs to be handled properly.
            // The idea is that internally we want to always use the fully qualified type.
        }

        [TestMethod]
        public void TestDrillDownSubtypes()
        {
            // Retrieve the HasAStaticField object.
            var t = NativeType.AtAddress(X86.StaticDtAddr, "HasAStaticField");

            // Drill into its VirtualType instance.
            var virtualType = t.GetField("subType");
            Assert.IsFalse(virtualType is Primitive);
            Assert.IsFalse(virtualType.IsStatic);
            Assert.IsTrue(virtualType.IsInstance);
            Assert.AreEqual("VirtualTypeDeriv", virtualType.TypeName);
            
            // Drill into the VirtualType's PODType instance. 
            var podType = virtualType.GetField("PODObject");
            Assert.IsFalse(podType is Primitive);
            Assert.IsFalse(podType.IsStatic);
            Assert.IsTrue(podType.IsInstance);
            Assert.AreEqual("PODType", podType.TypeName);

            // Finally, get the PODType's Offset1 value.
            var offset1 = podType.GetField(0x000);
            Assert.AreNotSame(podType, offset1);
            // This will work because PODType has no vtable.
            Assert.AreEqual(42UL, offset1.GetIntValue()); 
        }

        [TestMethod]
        public void TestDrillDownSubtypesDynamic()
        {
            // Retrieve the HasAStaticField object.
            dynamic t = NativeType.AtAddress(X86.StaticDtAddr, "HasAStaticField");

            // Drill into its VirtualType instance (easily) thanks to 
            Assert.AreEqual(42UL, t.subType.PODObject.Offset1.GetIntValue());
        }

        [TestMethod]
        public void TestGetPrimitiveString()
        {
            Assert.Fail("Not Implemented");
        }

        [TestMethod]
        public void TestCastPrimitiveToInteger()
        {
            Assert.Fail("Not implemented");
        }

        [TestMethod]
        public void TestCastPrimitiveToString()
        {
            Assert.Fail("Not implemented");
        }

        [TestMethod]
        public void TestCastNonPrimitiveToInteger()
        {
            Assert.Fail("Not implemented");
        }

        [TestMethod]
        public void TestCastNonPrimitiveToString()
        {
            Assert.Fail("Not implemented");
        }
    }
}
