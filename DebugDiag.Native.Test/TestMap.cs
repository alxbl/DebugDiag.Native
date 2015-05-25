using System.Linq;
using DebugDiag.Native.Test.Mock;
using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestMap
    {
        private const int Size = 3;
        private static readonly MockX86Dump Context = new MockX86Dump();
        private static Map _map;
        private static Map _emptyMap;

        [ClassInitialize]
        public static void Setup(TestContext ctx)
        {
            Native.Initialize(Context);

            var gSet = new Fixtures.Generators.Map(0x6666, Size, new Fixtures.Generators.PodType(0, 1));
            Context.AddFixture(gSet);
            _map = NativeType.AtAddress(gSet.Address, gSet.GetTypeName()) as Map;
            var gEmpty = new Fixtures.Generators.Map(0xefefef, 0, new Fixtures.Generators.PodType(0, 0));
            Context.AddFixture(gEmpty);
            _emptyMap = NativeType.AtAddress(gEmpty.Address, gEmpty.GetTypeName()) as Map;

            Assert.IsNotNull(_map);
            Assert.IsNotNull(_emptyMap);
        }

        [TestMethod]
        public void TestMapTypeParse()
        {
            var s = _map;
            Assert.IsNotNull(s.ValueType);
            Assert.IsInstanceOfType(s.ValueType, typeof(NativeType));
            Assert.AreEqual("PODType", s.ValueType.TypeName);
        }

        [TestMethod]
        public void TestMapEnumerate()
        {
            var s = _map;
            var i = 0UL;
            foreach (var e in s)
            {
                Assert.IsNotNull(e);
                Assert.IsInstanceOfType(e, typeof(Pair));
                Assert.IsTrue(e.IsInstance);
                var kv = e as Pair;
                Assert.IsNotNull(kv);
                
                Assert.IsTrue(kv.First is Integer);
                Assert.AreEqual("PODType", kv.Second.TypeName);
                Assert.AreEqual(i + 1, kv.Second.GetField("Offset1"));
                i++;
            }
            Assert.AreEqual((ulong)Size, i);
        }

        [TestMethod]
        public void TestMapEnumerateDynamic()
        {
            dynamic l = NativeType.AtAddress(_map.Address, _map.TypeName);
            Assert.IsNotNull(l);

            ulong count = 0;
            // Support for LINQ on dynamic types is not implemented yet. Usually we will iterate with a foreach.
            foreach (var e in l)
            {
                Assert.IsNotNull(e);
                count++;
            }
            Assert.AreEqual((ulong)Size, count);
        }

        [TestMethod]
        public void TestMapSize()
        {
            Assert.AreEqual((ulong)Size, _map.Size);
        }

        [TestMethod]
        public void TestEmptySet()
        {
            Assert.AreEqual(0UL, _emptyMap.Size);
            int i = _emptyMap.Count(); // foreach sanity check (shouldn't throw)
            Assert.AreEqual(0, i);
        }

        [TestMethod]
        public void TestParseMap()
        {
            // FIXME: This test fails because the fixtures don't not exist.
            // TODO: Implement a Map fixture generator.
            var maps = new[]
                       {
                           "std::map<int,Foo *,std::less<int>,std::allocator<std::pair<int const ,Foo *> > >",
                           "std::map<int,int,std::less<int>,std::allocator<std::pair<int const ,int> > >",
                           "std::map<int,Foo,std::less<int>,std::allocator<std::pair<int const ,Foo> > >",
                           "std::map<Foo,int,std::less<Foo>,std::allocator<std::pair<Foo const ,int> > >",
                           "std::map<int,std::basic_string<char,std::char_traits<char>,std::allocator<char> >,std::less<int>,std::allocator<std::pair<int const ,std::basic_string<char,std::char_traits<char>,std::allocator<char> > > > >"
                       };
            foreach (var m in maps)
            {
                var t = Parser.Parse(m);
                Assert.IsInstanceOfType(t, typeof(Map), "Could not parse " + m);
            }
        }

        [TestMethod]
        public void TestDeepCopyIntegrity()
        {
            // Use reflection to check if all fields are deep copied properly.
            Assert.Inconclusive("Not implemented");
        }
    }
}
