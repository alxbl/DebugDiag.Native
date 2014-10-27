using DebugDiag.Native.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebugDiag.Native.Test
{
    [TestClass]
    public class TestMap
    {
        [TestMethod]
        public void TestParseMap()
        {
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
                var t = TypeParser.Parse(m);
                Assert.IsInstanceOfType(t, typeof(Map), "Could not parse " + m);
            }
        }
    }
}
