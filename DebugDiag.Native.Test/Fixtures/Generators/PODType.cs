﻿using System;
using System.Collections.Generic;

namespace DebugDiag.Native.Test.Fixtures.Generators
{
    internal class PodType : Generator
    {
        private readonly ulong _addr;
        private int _value;

        public PodType(ulong addr, int value)
        {
            _addr = addr;
            _value = value;
        }

        public override string GetTypeName()
        {
            return "PODType";
        }

        public override KeyValuePair<string, string> GetTypeInfo()
        {
            return new KeyValuePair<string, string>("dt 0 PODType", @"   +0x000 Offset1          : Int4B
   +0x004 Offset2          : Int4B
   +0x008 Offset3          : Int4B");
        }

        public override IEnumerator<KeyValuePair<string, string>> Generate()
        {
            var kv = new KeyValuePair<string, string>(String.Format("dt {0:x} PODType", Address),
                String.Format(@"   +0x000 Offset1          : 0n{0}
   +0x004 Offset2          : 0n{0}
   +0x008 Offset3          : 0n{0}", _value++));
            Address += 0x8;
            yield return kv;
        }
    }
}
