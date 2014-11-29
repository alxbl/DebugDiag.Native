using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.Native.Test.Fixtures.Generators
{
    class List : Generator
    {
        private readonly Generator _childGenerator;
        private int _count;
        private ulong _addr;

        /// <summary>
        /// Creates a list of a specific element.
        /// </summary>
        /// <param name="addr">The address at which to dump the list.</param>
        /// <param name="count">The number of elements in the list fixture.</param>
        /// <param name="child">Generator for the list elements.</param>
        public List(ulong addr, int count, Generator child)
        {
            _childGenerator = child;
            _count = count;
            _addr = addr;
        }

        public override string GetTypeName()
        {
            return String.Format("std::list<{0},std::allocator<{0}> >", _childGenerator.GetTypeName());
        }

        public override KeyValuePair<string, string> GetTypeInfo()
        {
            var k = String.Format("dt 0 {0}", GetTypeName());
            var v = String.Format(@"+0x000 _Myproxy         : Ptr32 std::_Container_proxy
   +0x004 _Myhead          : Ptr32 std::_List_node<PODType,void *>
   +0x008 _Mysize          : Uint4B");
            return new KeyValuePair<string, string>(k,v);
        }

        public override IEnumerator<KeyValuePair<string, string>> Generate()
        {
            string k, v;
            var kv = new KeyValuePair<string, string>();
            
            // Root of the list
            k = String.Format("dt {0:x} {1}", Address, GetTypeName());
            v = String.Format("");
            kv = new KeyValuePair<string, string>(k, v);
            yield return kv;

            // Head of the list
            k = String.Format("dt");
            v = String.Format("");
            kv = new KeyValuePair<string, string>(k,v);
            yield return kv;
        }
    }
}
