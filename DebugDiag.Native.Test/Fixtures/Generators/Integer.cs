using System;
using System.Collections.Generic;
using DebugDiag.DbgEng;

namespace DebugDiag.Native.Test.Fixtures.Generators
{
    public sealed class Integer : Generator
    {
        public ulong Value { get; private set; }


        public Integer(ulong addr, ulong value)
        {
            Address = addr;
            Value = value;
        }

        public override string GetTypeName()
        {
            return "int";
        }

        public override KeyValuePair<string, string> GetTypeInfo()
        {
            return new KeyValuePair<string, string>(null, null); // Primitive, no `dt` output.
        }

        public override IEnumerable<KeyValuePair<string, string>> GenerateInternal()
        {
            // as int
            var kv = new KeyValuePair<string, string>(string.Format("?? *((int*)0x{0:x})", Address), string.Format("int 0n{0}", Value));
            yield return kv;

            // as uint
            kv = new KeyValuePair<string, string>(string.Format("?? *((int*)0x{0:x})", Address), string.Format("unsigned int 0x{0:x}", Value));
            yield return kv;
            
            // as ulong
            kv = new KeyValuePair<string, string>(string.Format("?? *((unsigned long*)0x{0:x})", Address), string.Format("unsigned long 0x{0:x}", Value));
            yield return kv;

            // as long
            kv = new KeyValuePair<string, string>(string.Format("?? *((long*)0x{0:x})", Address), string.Format("long 0x{0:x}", Value));
            yield return kv;

            // as float
            kv = new KeyValuePair<string, string>(string.Format("?? *((float*)0x{0:x})", Address), string.Format("float {0}", (float)Value));
            yield return kv;

            // as double
            kv = new KeyValuePair<string, string>(string.Format("?? *((double*)0x{0:x})", Address), string.Format("float {0}", (double)Value));
            yield return kv;

            // as bool
            kv = new KeyValuePair<string, string>(string.Format("?? *((bool*)0x{0:x})", Address), string.Format("bool {0}", Value != 0));
            yield return kv;

        }

    }
}
