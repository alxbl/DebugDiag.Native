using System;
using DebugDiag.Native.Test.Fixtures;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using String = DebugDiag.Native.Test.Fixtures.Generators.String;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestString
    {
        private static readonly MockX86Dump Context = new MockX86Dump();


        [TestInitialize]
        public void Setup()
        {
            Native.Initialize(Context);

        }
        [TestMethod]
        public void TestStringSyntax()
        {
            var valid = new[]
                             {
                                 "[80] Char",
                                 "[3] Wchar",
                                 "std::basic_string<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t> >",
                                 "std::basic_string<char,std::char_traits<char>,std::allocator<char> >",
                                 "Ptr32 Char",
                                 "Ptr64 Char",
                                 "Ptr32 Wchar",
                                 "Ptr64 Wchar",
                                 "Wchar *",
                                 "Char *"
                             };

            var invalid = new[]
                          {
                              "[] Char",
                              "Char",
                              "Ptr32 Ptr32 Char",
                              "Wchar",
                              "string",
                              "std::string", // This is a typedef for std::basic_string<char,std::char_traits<char>,std::allocator<char> >
                              "std::wstring" // This is a typedef for  std::basic_string<wchar_t,std::char_traits<wchar_t>,std::allocator<wchar_t> >
                          };
            foreach (var v in valid)
                Assert.IsTrue(Type.String.Syntax.IsMatch(v), string.Format("Expected valid String type: {0}", v));

            foreach (var v in invalid)
                Assert.IsFalse(Type.String.Syntax.IsMatch(v), string.Format("Expected invalid String type: {0}", v));
        }

        [TestMethod]
        public void TestCharPtrValid()
        {
            const ulong addr = 0x30;
            const string expected = "Test String with\r\n Line breaks.";
            var str = new String(addr, String.StringFormat.Narrow, expected);
            Context.AddFixture(str);
            
            var t = NativeType.AtAddress(addr, str.GetTypeName());
            Assert.IsTrue(t.IsInstance);
            Assert.IsTrue(t is Primitive);
            Assert.IsTrue(t is Type.String);
            Assert.AreEqual(expected, t.GetStringValue());
        }

        [TestMethod]
        public void TestCharPtrNull()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestWCharPtrValid()
        {
            const ulong addr = 0x31;
            const string expected = "Test Wide String with\r\n Line breaks.";
            var str = new String(addr, String.StringFormat.Wide, expected);
            Context.AddFixture(str);

            var t = NativeType.AtAddress(addr, str.GetTypeName());
            Assert.IsTrue(t.IsInstance);
            Assert.IsTrue(t is Primitive);
            Assert.IsTrue(t is Type.String);
            Assert.AreEqual(expected, t.GetStringValue());
        }

        [TestMethod]
        public void TestWCharPtrNull()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestCharArray()
        {
            const ulong addr = 0x32;
            const string expected = "Test char array with\r\n Line breaks.";
            var str = new String(addr, String.StringFormat.ArrayNarrow, expected);
            Context.AddFixture(str);

            var t = NativeType.AtAddress(addr, str.GetTypeName());
            Assert.IsTrue(t.IsInstance);
            Assert.IsTrue(t is Primitive);
            Assert.IsTrue(t is Type.String);
            Assert.AreEqual(expected, t.GetStringValue());
        }

        [TestMethod]
        public void TestWCharArray()
        {
            const ulong addr = 0x33;
            const string expected = "Test wchar array with\r\n Line breaks.";
            var str = new String(addr, String.StringFormat.ArrayWide, expected);
            Context.AddFixture(str);

            var t = NativeType.AtAddress(addr, str.GetTypeName());
            Assert.IsTrue(t.IsInstance);
            Assert.IsTrue(t is Primitive);
            Assert.IsTrue(t is Type.String);
            Assert.AreEqual(expected, t.GetStringValue());
        }

        [TestMethod]
        public void TestStdWStringInBuf()
        {
            const ulong addr = 0x34;
            const string expected = "len<8";
            var str = new String(addr, String.StringFormat.StlWide, expected);
            Context.AddFixture(str);

            var t = NativeType.AtAddress(addr, str.GetTypeName());
            Assert.IsTrue(t.IsInstance);
            Assert.IsTrue(t is Primitive);
            Assert.IsTrue(t is Type.String);
            Assert.AreEqual(expected, t.GetStringValue());
        }

        [TestMethod]
        public void TestStdWStringPtr()
        {
            const ulong addr = 0x35;
            const string expected = "The length of this string must be longer than 8.";
            var str = new String(addr, String.StringFormat.StlWide, expected);
            Context.AddFixture(str);

            var t = NativeType.AtAddress(addr, str.GetTypeName());
            Assert.IsTrue(t.IsInstance);
            Assert.IsTrue(t is Primitive);
            Assert.IsTrue(t is Type.String);
            Assert.AreEqual(expected, t.GetStringValue());
        }

        [TestMethod]
        public void TestStdStringInBuf()
        {
            const ulong addr = 0x36;
            const string expected = "len<16";
            var str = new String(addr, String.StringFormat.StlNarrow, expected);
            Context.AddFixture(str);

            var t = NativeType.AtAddress(addr, str.GetTypeName());
            Assert.IsTrue(t.IsInstance);
            Assert.IsTrue(t is Primitive);
            Assert.IsTrue(t is Type.String);
            Assert.AreEqual(expected, t.GetStringValue());
        }

        [TestMethod]
        public void TestStdStringptr()
        {
            const ulong addr = 0x37;
            const string expected = "The length of this string must be longer than 16 characters.\r\nHello.";
            var str = new String(addr, String.StringFormat.StlNarrow, expected);
            Context.AddFixture(str);

            var t = NativeType.AtAddress(addr, str.GetTypeName());
            Assert.IsTrue(t.IsInstance);
            Assert.IsTrue(t is Primitive);
            Assert.IsTrue(t is Type.String);
            Assert.AreEqual(expected, t.GetStringValue());
        }
    }
}
