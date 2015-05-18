using System.Collections.Generic;
using System.Text;

namespace DebugDiag.Native.Test.Fixtures.Generators
{
    /// <summary>
    /// Generates an std::set fixture.
    /// </summary>
    public class Set : Generator
    {
        private readonly Generator _childGenerator;
        private readonly int _count;

        /// <summary>
        /// Creates a set of a specific element.
        /// </summary>
        /// <param name="addr">The address at which to dump the set.</param>
        /// <param name="count">The number of elements in the set fixture.</param>
        /// <param name="child">Generator for the set elements.</param>
        public Set(ulong addr, int count, Generator child)
        {
            _childGenerator = child;
            _count = count;
            Address = addr;
        }

        public override string GetTypeName()
        {
            return string.Format("std::set<{0},std::less<{0}>,std::allocator<{0}> >", _childGenerator.GetTypeName());
        }

        public override KeyValuePair<string, string> GetTypeInfo()
        {
            var k = string.Format("dt 0 {0}", GetTypeName());
            var v = string.Format(@"   +0x000 _Myproxy         : Ptr32 std::_Container_proxy
   +0x004 _Myhead          : Ptr32 std::_Tree_node<{0},void *>
   +0x008 _Mysize          : Uint4B", _childGenerator.GetTypeName());

            return new KeyValuePair<string, string>(k, v);
        }

        public override IEnumerable<KeyValuePair<string, string>> GenerateInternal()
        {
            // Root of the set (k, v, and kv are re-used)
            var k = string.Format("dt 0x{0:x} {1}", Address, GetTypeName());
            var v = string.Format(@"   +0x000 _Myproxy         : 0xbaadf00d std::_Container_proxy
   +0x004 _Myhead          : 0xbaadf00d std::_Tree_node<{0},void *>
   +0x008 _Mysize          : {1}", _childGenerator.GetTypeName(), _count);
            var kv = new KeyValuePair<string, string>(k, v);
            yield return kv;

            // Generate children type info.
            var children = new StringBuilder();
            children.AppendFormat("Size={0}\r\n", _count);
            yield return _childGenerator.GetTypeInfo();

            // Generate children.
            for (ulong i = 0; i < (ulong)_count; ++i)
            {
                var addr = 0xbb0 + i;
                _childGenerator.Address = addr;
                children.AppendFormat("0x{0:x}\r\n", addr);
                foreach (var fixture in _childGenerator.Generate(false))
                {
                    yield return fixture;
                }
            }

            // Generate the !set output
            k = string.Format("!set 0x{0:x}", Address);
            v = children.ToString();
            kv = new KeyValuePair<string, string>(k, v);
            yield return kv;
        }
    }
}

